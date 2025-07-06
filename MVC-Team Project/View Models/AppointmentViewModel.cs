using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int AvailabilityId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required]
        [MaxLength(20)]
        public string Status { get; set; } = "Scheduled";

        [MaxLength(1000)]
        public string? PatientNotes { get; set; }

        [MaxLength(1000)]
        public string? Symptoms { get; set; }

        [MaxLength(2000)]
        public string? Diagnosis { get; set; }

        [MaxLength(2000)]
        public string? Prescription { get; set; }

        [MaxLength(2000)]
        public string? DoctorNotes { get; set; }

        [MaxLength(500)]
        public string? CancellationReason { get; set; }

        public bool IsFollowUpRequired { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }

        public string? DoctorName { get; set; }

        public string? DoctorSpecialty { get; set; }

        public string? PatientName { get; set; }

        public string? PatientEmail { get; set; }
    }
}
