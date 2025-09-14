using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Authentication;
using MountedGames.Logic.Services;

namespace MountedGames.Logic.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController(MountedGamesDbContext context, JwtService jwtService) : ControllerBase
    {
        [HttpPost]
        [Route("register")]
        public ActionResult<AuthResponse> Register(RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (existingUser != null)
                {
                    return BadRequest("User with this email already exists");
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);

                User user = new()
                {
                    Email = request.Email,
                    PasswordHash = hashedPassword,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Role = request.Role.ToLower()
                };

                context.Users.Add(user);

                context.SaveChanges();

                string token = jwtService.GenerateToken(user);

                AuthResponse response = new()
                {
                    User = new()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    },
                    Token = token
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        [HttpPost]
        [Route("login")]
        public ActionResult<AuthResponse> Login(LoginRequest request)
        {
            try
            {
                var user = context.Users.FirstOrDefault(u => u.Email == request.Email);
                if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
                {
                    return Unauthorized("Invalid email or password");
                }

                string token = jwtService.GenerateToken(user);
                AuthResponse response = new()
                {
                    User = new()
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email
                    },
                    Token = token
                };
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        // Test endpoint - only for authenticated users
        [Authorize]
        [HttpGet]
        [Route("profile")]
        public IActionResult GetProfile()
        {
            var userRole = User.FindFirst("role")?.Value;
            var userName = User.FindFirst("firstName")?.Value;

            return Ok(new
            {
                message = $"Hello {userName}!",
                role = userRole,
                userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
            });
        }

        // Test endpoint - only for organizers
        [Authorize(Roles = UserRoles.Organizer)]
        [HttpGet]
        [Route("organizer-only")]
        public IActionResult OrganizerOnly()
        {
            return Ok("This endpoint is only accessible by organizers");
        }

        // Test endpoint - only for admins
        [Authorize(Roles = UserRoles.Admin)]
        [HttpGet]
        [Route("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok("This endpoint is only accessible by admins");
        }

        // Test endpoint - multiple roles allowed
        [Authorize(Roles = $"{UserRoles.Organizer},{UserRoles.Judge}")]
        [HttpGet]
        [Route("organizer-or-judge")]
        public IActionResult OrganizerOrJudge()
        {
            return Ok("This endpoint is accessible by organizers and judges");
        }
    }
}