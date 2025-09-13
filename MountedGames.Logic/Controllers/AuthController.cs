using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using MountedGames.Logic.Data;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Authentication;
using MountedGames.Logic.Services;

namespace MountedGames.Logic.Controllers
{
    [ApiController]
    public class AuthController(MountedGamesDbContext context, JwtService jwtService) : ControllerBase
    {
        [HttpPost]
        [Route("api/auth/register")]
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
                    LastName = request.LastName
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
        [Route("api/auth/login")]
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
    }
}