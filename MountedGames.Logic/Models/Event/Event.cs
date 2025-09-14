using MountedGames.Logic.Enums;

namespace MountedGames.Logic.Models.Event
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = default!;
        public EventStatus Status { get; set; } = EventStatus.Planned;
        public User Owner { get; set; }
        public string? Location { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
