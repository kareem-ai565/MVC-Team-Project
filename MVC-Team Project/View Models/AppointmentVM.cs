using MVC_Team_Project.Models;
using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    // Main view model for displaying appointments in list/grid
    public class AppointmentVM
    {
        public int Id { get; set; }
        public string? PatientName { get; set; }
        public string? DoctorName { get; set; }
        public string? SpecialtyName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? PatientNotes { get; set; }
        public string? DoctorNotes { get; set; }
        public string? Prescription { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsFollowUpRequired { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // Form view model for creating/editing appointments
    public class AppointmentFormVM
    {
        public Appointment Appointment { get; set; } = new();
        public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();
        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();
        public IEnumerable<Availability> Availabilities { get; set; } = new List<Availability>();

        // For filtering available time slots
        public int? SelectedDoctorId { get; set; }
        public DateTime? SelectedDate { get; set; }
        public List<TimeSlot> AvailableTimeSlots { get; set; } = new();
    }

    // View model for creating a new appointment (simplified)
    public class CreateAppointmentVM
    {
        [Required]
        [Display(Name = "Patient")]
        public int PatientId { get; set; }

        [Required]
        [Display(Name = "Doctor")]
        public int DoctorId { get; set; }

        [Required]
        [Display(Name = "Appointment Date")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Display(Name = "Patient Notes")]
        [StringLength(1000)]
        public string? PatientNotes { get; set; }

        [Display(Name = "Symptoms")]
        [StringLength(1000)]
        public string? Symptoms { get; set; }

        // Dropdown lists
        public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();
        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();
        public List<TimeSlot> AvailableTimeSlots { get; set; } = new();
    }

    // View model for updating appointment details (for doctors/staff)
    public class UpdateAppointmentVM
    {
        public int Id { get; set; }

        [Display(Name = "Patient Name")]
        public string? PatientName { get; set; }

        [Display(Name = "Doctor Name")]
        public string? DoctorName { get; set; }

        [Display(Name = "Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [Display(Name = "Start Time")]
        public TimeSpan StartTime { get; set; }

        [Display(Name = "End Time")]
        public TimeSpan EndTime { get; set; }

        [Required]
        [Display(Name = "Status")]
        public string Status { get; set; } = "Scheduled";

        [Display(Name = "Symptoms")]
        [StringLength(1000)]
        public string? Symptoms { get; set; }

        [Display(Name = "Diagnosis")]
        [StringLength(2000)]
        public string? Diagnosis { get; set; }

        [Display(Name = "Prescription")]
        [StringLength(2000)]
        public string? Prescription { get; set; }

        [Display(Name = "Doctor Notes")]
        [StringLength(2000)]
        public string? DoctorNotes { get; set; }

        [Display(Name = "Patient Notes")]
        [StringLength(1000)]
        public string? PatientNotes { get; set; }

        [Display(Name = "Follow-up Required")]
        public bool IsFollowUpRequired { get; set; }

        [Display(Name = "Follow-up Date")]
        [DataType(DataType.Date)]
        public DateTime? FollowUpDate { get; set; }

        [Display(Name = "Cancellation Reason")]
        [StringLength(500)]
        public string? CancellationReason { get; set; }

        // Read-only properties
        public decimal ConsultationFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    // View model for appointment details
    public class AppointmentDetailsVM
    {
        public int Id { get; set; }
        public string? PatientName { get; set; }
        public string? PatientEmail { get; set; }
        public string? PatientPhone { get; set; }
        public string? DoctorName { get; set; }
        public string? SpecialtyName { get; set; }
        public DateTime AppointmentDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? Symptoms { get; set; }
        public string? Diagnosis { get; set; }
        public string? PatientNotes { get; set; }
        public string? DoctorNotes { get; set; }
        public string? Prescription { get; set; }
        public decimal ConsultationFee { get; set; }
        public bool IsFollowUpRequired { get; set; }
        public DateTime? FollowUpDate { get; set; }
        public string? CancellationReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? ClinicAddress { get; set; }

        // Collections
        public IEnumerable<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public IEnumerable<Payment> Payments { get; set; } = new List<Payment>();
    }

    // View model for filtering appointments
    public class AppointmentFilterVM
    {
        public string? Search { get; set; }
        public string? Status { get; set; }
        public int? DoctorId { get; set; }
        public int? PatientId { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Dropdown lists
        public IEnumerable<Doctor> Doctors { get; set; } = new List<Doctor>();
        public IEnumerable<Patient> Patients { get; set; } = new List<Patient>();
        public List<string> StatusList { get; set; } = new() { "Scheduled", "Confirmed", "In Progress", "Completed", "Cancelled", "No Show" };
    }

    // Helper class for time slots
    public class TimeSlot
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string DisplayText => $"{StartTime:hh\\:mm} - {EndTime:hh\\:mm}";
        public bool IsAvailable { get; set; } = true;
    }

    // View model for appointment statistics/dashboard
    public class AppointmentStatsVM
    {
        public int TotalAppointments { get; set; }
        public int TodaysAppointments { get; set; }
        public int UpcomingAppointments { get; set; }
        public int CompletedAppointments { get; set; }
        public int CancelledAppointments { get; set; }
        public int NoShowAppointments { get; set; }
        public decimal TotalRevenue { get; set; }
        public decimal TodaysRevenue { get; set; }

        // Recent appointments
        public IEnumerable<AppointmentVM> RecentAppointments { get; set; } = new List<AppointmentVM>();
        public IEnumerable<AppointmentVM> TodaysAppointmentsList { get; set; } = new List<AppointmentVM>();

        
    }
}
