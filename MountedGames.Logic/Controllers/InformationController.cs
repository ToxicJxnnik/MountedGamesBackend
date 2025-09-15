using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;
using System.Security.Claims;

namespace MountedGames.Logic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class InformationController(MountedGamesDbContext context) : ControllerBase
    {
        [Authorize]
        [HttpPost]
        [Route("update")]
        public ActionResult Update([FromBody] InformationUpdateRequest request)
        {
            try
            {
                // Log the incoming request for debugging
                Console.WriteLine($"Received request: {System.Text.Json.JsonSerializer.Serialize(request)}");

                // Validate the request
                if (request == null)
                {
                    return BadRequest("Request body is null or invalid");
                }

                // Get user email from JWT token - try different claim types
                string? userEmail = null;

                // Try common email claim names
                var emailClaim = User.FindFirst("email") ??
                               User.FindFirst(ClaimTypes.Email) ??
                               User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress") ??
                               User.FindFirst("sub");

                if (emailClaim != null)
                {
                    userEmail = emailClaim.Value;
                }

                if (string.IsNullOrEmpty(userEmail))
                {
                    Console.WriteLine("Available claims:");
                    foreach (var claim in User.Claims)
                    {
                        Console.WriteLine($"  {claim.Type}: {claim.Value}");
                    }
                    return BadRequest("User email claim not found in JWT token");
                }

                Console.WriteLine($"Looking for user with email: {userEmail}");

                // Check if user exists
                var existingUser = context.Users.FirstOrDefault(u => u.Email == userEmail);
                if (existingUser == null)
                {
                    return BadRequest($"User with email {userEmail} doesn't exist");
                }

                Console.WriteLine($"Found user: {existingUser.FirstName} {existingUser.LastName}");

                // Update user properties - only update if values are provided
                if (!string.IsNullOrEmpty(request.FirstName))
                {
                    existingUser.FirstName = request.FirstName;
                    Console.WriteLine($"Updated FirstName to: {request.FirstName}");
                }

                if (!string.IsNullOrEmpty(request.LastName))
                {
                    existingUser.LastName = request.LastName;
                    Console.WriteLine($"Updated LastName to: {request.LastName}");
                }

                if (!string.IsNullOrEmpty(request.Club))
                {
                    existingUser.Club = request.Club;
                    Console.WriteLine($"Updated Club to: {request.Club}");
                }

                /*if (request.DateOfBirth.HasValue)
                {
                    existingUser.DateOfBirth = request.DateOfBirth.Value;
                    Console.WriteLine($"Updated DateOfBirth to: {request.DateOfBirth}");
                }*/

                if (!string.IsNullOrEmpty(request.PhoneNumber))
                {
                    existingUser.PhoneNumber = request.PhoneNumber;
                    Console.WriteLine($"Updated PhoneNumber to: {request.PhoneNumber}");
                }

                if (!string.IsNullOrEmpty(request.FinNumber))
                {
                    existingUser.FinNumber = request.FinNumber;
                    Console.WriteLine($"Updated FinNumber to: {request.FinNumber}");
                }

                if (!string.IsNullOrEmpty(request.Nationality))
                {
                    existingUser.Nationality = request.Nationality;
                    Console.WriteLine($"Updated Nationality to: {request.Nationality}");
                }

                // IMPORTANT: Save changes to database
                context.SaveChanges();
                Console.WriteLine("Changes saved successfully");

                return Ok(new { message = "User information updated successfully" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error in Update method: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    error = ex.Message,
                    stackTrace = ex.StackTrace,
                    details = "Check server console logs for more information"
                });
            }
        }
    }
}