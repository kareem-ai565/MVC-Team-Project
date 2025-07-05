using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class AppointmentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient is required.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Appointment date is required.")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Start time is required.")]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required.")]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Status is required.")]
        [StringLength(20)]
        public string Status { get; set; }

        [StringLength(1000)]
        public string? PatientNotes { get; set; }

        [StringLength(1000)]
        public string? Symptoms { get; set; }

        [StringLength(2000)]
        public string? Diagnosis { get; set; }

        [StringLength(2000)]
        public string? Prescription { get; set; }

        [StringLength(2000)]
        public string? DoctorNotes { get; set; }

        [StringLength(500)]
        public string? CancellationReason { get; set; }

        public bool IsFollowUpRequired { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }

        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
    }
}
