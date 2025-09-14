namespace MountedGames.Logic.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
