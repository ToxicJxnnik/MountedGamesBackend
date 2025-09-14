namespace MountedGames.Logic.Models
{
    public static class UserRoles
    {
        public const string Organizer = "organizer";
        public const string Horseman = "horseman";
        public const string Judge = "judge";
        public const string Admin = "admin";

        public static readonly string[] AllRoles = [ Organizer, Horseman, Judge, Admin ];

        public static bool IsValidRole(string role)
        {
            return AllRoles.Contains(role.ToLower());
        }
    }
}