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
                    return null;
                }

                var json = await response.Content.ReadAsStringAsync();
                var userInfo = JsonSerializer.Deserialize<Auth0UserInfo>(json, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return userInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error validating Auth0 token: {ex.Message}");
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

            // Create new user from Auth0 info
            var newUser = new User
            {
                Email = auth0User.Email,
                FirstName = auth0User.GivenName ?? auth0User.Name?.Split(' ').FirstOrDefault() ?? "",
                LastName = auth0User.FamilyName ?? auth0User.Name?.Split(' ').Skip(1).FirstOrDefault() ?? "",
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