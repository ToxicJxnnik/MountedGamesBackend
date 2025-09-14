using Microsoft.IdentityModel.Tokens;
using MountedGames.Logic.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MountedGames.Logic.Services
{
    public class JwtService(IConfiguration configuration)
    {
        public string GenerateToken(User user)
        {
            // 1. Security Key erstellen (aus appsettings.json)
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
            );

            // 2. Signing Credentials (wie wird signiert)
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 3. Claims erstellen (Benutzer-Informationen)
            var claims = new[]
            {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),  // User ID
            new Claim(ClaimTypes.Email, user.Email),                   // Email
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"), // Name
            new Claim("firstName", user.FirstName),                    // Custom Claims
            new Claim("lastName", user.LastName)
        };

            // 4. Token erstellen
            var token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],           // Wer hat es erstellt
                audience: configuration["Jwt:Audience"],       // Für wen ist es bestimmt
                claims: claims,                                 // Benutzer-Daten
                expires: DateTime.Now.AddMinutes(
                    Convert.ToDouble(configuration["Jwt:ExpireMinutes"])
                ),                                              // Wann läuft es ab
                signingCredentials: credentials                 // Sicherheits-Signatur
            );

            // 5. Token zu String konvertieren
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public ClaimsPrincipal? ValidateToken(string token)
        {
            try
            {
                var key = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
                );

                var tokenHandler = new JwtSecurityTokenHandler();
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = key,
                    ClockSkew = TimeSpan.Zero  // Keine Toleranz für abgelaufene Token
                };

                var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
                return principal;
            }
            catch
            {
                return null; // Token ungültig
            }
        }
    }
}
