using System.ComponentModel.DataAnnotations;

namespace MountedGames.Logic.Models.Event
{
    public class EventRegistration
    {
        public int Id { get; set; }

        [Required]
        public int EventId { get; set; }

       
        [Required]
        public int UserId { get; set; }

       
        [Required]
        [StringLength(100)]
        public string RiderName { get; set; } = string.Empty;

        
        public DateTime? BirthDate { get; set; }

        
        [Required]
        public AgeCategory AgeCategory { get; set; } = AgeCategory.OFFEN;

        
        [Required]
        public string HorseName { get; set; } = string.Empty;

        
        public int StallSpaces { get; set; } = 0;

        
        public int CampingSpaces { get; set; } = 0;

       
        [Range(0, 5)]
        public int ChargingSpaces { get; set; } = 0;

        
        public string Comments { get; set; } = string.Empty;

       
        public DateTime RegistrationDate { get; set; } = DateTime.UtcNow;

      
        public virtual Event? Event { get; set; }
        public virtual User? User { get; set; }
    }
}
