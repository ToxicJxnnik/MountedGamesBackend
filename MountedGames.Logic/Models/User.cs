namespace MountedGames.Logic.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Role { get; set; } = "horseman"; // Default role
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string? PasswordHash { get; set; }
        public DateTime? DateOfBirth { get; set; }
        public string? Nationality { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Club { get; set; }
        public string? FinNumber { get; set; }
    }
}
