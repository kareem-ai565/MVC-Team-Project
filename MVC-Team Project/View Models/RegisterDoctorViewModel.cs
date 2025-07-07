using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class RegisterDoctorViewModel
    {
        public int Id { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? ProfilePicturePath { get; set; }    


        [Required(ErrorMessage = "Full name is required")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone number is required")]
        [Phone(ErrorMessage = "Invalid phone number")]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Confirm password is required")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match")]
        public string ConfirmPassword { get; set; }

        public string Role { get; set; } = "Doctor";

        // Doctor-specific fields
        [Required(ErrorMessage = "License number is required")]
        public string LicenseNumber { get; set; }

        [Required(ErrorMessage = "Specialty is required")]
        public int? SpecialtyId { get; set; }
        public string? SpecialityName { get; set; }

        [Required(ErrorMessage = "Clinic address is required")]
        public string ClinicAddress { get; set; }

        [Required(ErrorMessage = "Bio is required")]
        public string Bio { get; set; }

        [Required(ErrorMessage = "Education is required")]
        public string Education { get; set; }

        [Required(ErrorMessage = "Certifications are required")]
        public string Certifications { get; set; }

        [Required(ErrorMessage = "Experience years is required")]
        public int? ExperienceYears { get; set; }

        [Required(ErrorMessage = "Consultation fee is required")]
        public decimal? ConsultationFee { get; set; }

        public bool IsVerified { get; set; }

    }
}
