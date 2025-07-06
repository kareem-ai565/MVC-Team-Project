namespace MVC_Team_Project.View_Models
{
    public class DoctorsVM
    {
        public int Id { get; set; }

        public string FullName { get; set; }

        public string? ProfilePicture { get; set; }

        public string SpecialtyName { get; set; }

        public string Bio { get; set; }

        public string ClinicAddress { get; set; }

        public decimal ConsultationFee { get; set; }

        public int ExperienceYears { get; set; }

        public bool IsVerified { get; set; }

        public List<DoctorAvailabilityVM> Availabilities { get; set; } = new();
        public List<DoctorAppointmentVM> Appointments { get; set; } = new();
    }
}
