namespace MountedGames.Logic.Models.Authentication
{
    public class SocialLoginRequest
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? Role { get; set; } = UserRoles.Horseman;
    }
}
