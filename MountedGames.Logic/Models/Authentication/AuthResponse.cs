namespace MountedGames.Logic.Models.Authentication
{
    public class AuthResponse
    {
        public UserDto User { get; set; } = new();
        public string Token { get; set; } = string.Empty;
    }
}
