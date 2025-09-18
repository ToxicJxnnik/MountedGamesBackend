using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Authentication;

namespace MountedGames.Logic.Services
{
    public class Auth0Service(
        MountedGamesDbContext context,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        private readonly HttpClient _httpClient = httpClientFactory.CreateClient();

        public async Task<Auth0UserInfo?> ValidateAuth0Token(string accessToken)
        {
            try
            {
                // Get user info from Auth0 using the access token
                var request = new HttpRequestMessage(HttpMethod.Get, $"{configuration["Auth0:Domain"]}/userinfo");
                request.Headers.Authorization =
                    new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

                var response = await _httpClient.SendAsync(request);
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Auth0 API call failed: {response.StatusCode} - {await response.Content.ReadAsStringAsync()}");
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Auth0 response: {json}"); // Debug log

                var userInfo = JsonSerializer.Deserialize<Auth0UserInfo>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return userInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating Auth0 token: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                return null;
            }
        }

        public async Task<User> GetOrCreateUserFromAuth0(Auth0UserInfo auth0User)
        {
            // Check if user already exists by email
            var existingUser = await context.Users.FirstOrDefaultAsync(u => u.Email == auth0User.Email);

            if (existingUser != null)
            {
                // Update Auth0 info if this is a new social login for existing user
                if (string.IsNullOrEmpty(existingUser.Auth0UserId))
                {
                    existingUser.Auth0UserId = auth0User.Sub;
                    existingUser.SocialProvider = auth0User.Sub;
                    await context.SaveChangesAsync();
                }

                return existingUser;
            }

            // Extract first and last name with better fallback logic
            string firstName = "";
            string lastName = "";

            // Try to use given_name and family_name first
            if (!string.IsNullOrEmpty(auth0User.GivenName))
            {
                firstName = auth0User.GivenName;
            }

            if (!string.IsNullOrEmpty(auth0User.FamilyName))
            {
                lastName = auth0User.FamilyName;
            }

            // If we don't have both names, try to parse the full name
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                if (!string.IsNullOrEmpty(auth0User.Name))
                {
                    var nameParts = auth0User.Name.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (nameParts.Length > 0)
                    {
                        if (string.IsNullOrEmpty(firstName))
                            firstName = nameParts[0];

                        if (string.IsNullOrEmpty(lastName) && nameParts.Length > 1)
                            lastName = string.Join(" ", nameParts.Skip(1));
                    }
                }
            }

            // If we still don't have names, use nickname or email prefix as fallback
            if (string.IsNullOrEmpty(firstName))
            {
                firstName = !string.IsNullOrEmpty(auth0User.Nickname)
                    ? auth0User.Nickname
                    : auth0User.Email.Split('@')[0];
            }

            if (string.IsNullOrEmpty(lastName))
            {
                lastName = ""; // Leave empty if we can't determine it
            }

            Console.WriteLine($"Creating user: {firstName} {lastName} ({auth0User.Email})"); // Debug log

            // Create new user from Auth0 info
            var newUser = new User
            {
                Email = auth0User.Email,
                FirstName = firstName,
                LastName = lastName,
                Role = UserRoles.Horseman, // Default role
                Auth0UserId = auth0User.Sub,
                SocialProvider = auth0User.Sub,
                // No password hash - this is a social user
                PasswordHash = null
            };

            context.Users.Add(newUser);
            await context.SaveChangesAsync();

            return newUser;
        }
    }
}