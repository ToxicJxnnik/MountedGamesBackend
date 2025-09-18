using System.ComponentModel.DataAnnotations;

namespace MountedGames.Logic.Models.Event
{
    public class EventRegistrationRequest
    {
        [Required]
        public string RiderName { get; set; } = string.Empty;

        [Required]
        public DateTime BirthDate { get; set; }

        [Required]
        public AgeCategory AgeCategory { get; set; }

        [Required]
        public string HorseName { get; set; } = string.Empty;

        public int StallSpaces { get; set; } = 0;

        public int CampingSpaces { get; set; } = 0;

        public int ChargingSpaces { get; set; } = 0;

        public string Comments { get; set; } = string.Empty;
    }

    public enum AgeCategory
    {
        U14,
        U16,
        U18,
        OFFEN
    }
}
