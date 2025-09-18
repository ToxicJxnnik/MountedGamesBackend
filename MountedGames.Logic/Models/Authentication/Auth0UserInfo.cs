using System.Text.Json.Serialization;

namespace MountedGames.Logic.Models.Authentication
{
    public class Auth0UserInfo
    {
        [JsonPropertyName("sub")]
        public string Sub { get; set; } = string.Empty;

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("email")]
        public string Email { get; set; } = string.Empty;

        [JsonPropertyName("given_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? GivenName { get; set; }

        [JsonPropertyName("family_name")]
        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public string? FamilyName { get; set; }

        [JsonPropertyName("picture")]
        public string Picture { get; set; } = string.Empty;

        [JsonPropertyName("email_verified")]
        public bool EmailVerified { get; set; }

        // Additional properties that might be returned
        [JsonPropertyName("nickname")]
        public string? Nickname { get; set; }

        [JsonPropertyName("locale")]
        public string? Locale { get; set; }
    }
}