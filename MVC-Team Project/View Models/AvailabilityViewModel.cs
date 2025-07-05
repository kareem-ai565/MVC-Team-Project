using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class AvailabilityViewModel
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Available date is required.")]
        [DataType(DataType.Date)]
        public DateTime AvailableDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Slot duration is required.")]
        [Range(5, 120, ErrorMessage = "Slot duration must be between 5 and 120 minutes.")]
        public int SlotDuration { get; set; }

        [Required(ErrorMessage = "Max patients is required.")]
        [Range(1, 100, ErrorMessage = "Max patients must be between 1 and 100.")]
        public int MaxPatients { get; set; }

        public bool IsBooked { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Optional display
        public string? DoctorName { get; set; }
    }
}
