using MVC_Team_Project.Models;

namespace MVC_Team_Project.View_Models
{
    public class SpecialtyForShowVM
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }

        // List of doctors in that specialty
        public List<DoctorsVM> Doctors { get; set; }
    }
}
