using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class PatientFormVM
    {
        public int? Id { get; set; }

        [Display(Name = "Linked User")]
        public int? UserId { get; set; }

        [Display(Name = "First Name")]
        public string? FirstName { get; set; }

        [Display(Name = "Last Name")]
        public string? LastName { get; set; }

        [EmailAddress]
        public string? Email { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [DataType(DataType.Password)]
        public string? Password { get; set; }

        // Patient details
        public string? Gender { get; set; }

        [DataType(DataType.Date)]
        public DateTime? DOB { get; set; }

        public string? Address { get; set; }

        [Display(Name = "Emergency Contact")]
        public string? EmergencyContact { get; set; }

        [Display(Name = "Emergency Phone")]
        public string? EmergencyPhone { get; set; }

        [Display(Name = "Blood Type")]
        public string? BloodType { get; set; }

        public string? Allergies { get; set; }

        [Display(Name = "Medical History")]
        public string? MedicalHistory { get; set; }

        [Display(Name = "Current Medications")]
        public string? CurrentMedications { get; set; }

        [Display(Name = "Insurance Provider")]
        public string? InsuranceProvider { get; set; }

        [Display(Name = "Insurance Policy #")]
        public string? InsurancePolicyNumber { get; set; }
    }
}
