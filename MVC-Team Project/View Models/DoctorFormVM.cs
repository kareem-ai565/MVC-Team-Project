using MVC_Team_Project.Models;
namespace MVC_Team_Project.View_Models

{
    public class DoctorFormVM
    {
        public Doctor Doctor { get; set; } = new Doctor();
        public IFormFile? ProfilePicture { get; set; }

        public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
        public List<Specialty> Specialties { get; set; } = new List<Specialty>();
    }
}