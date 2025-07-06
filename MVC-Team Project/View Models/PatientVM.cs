namespace MVC_Team_Project.View_Models
{
    public class PatientVM
    {
        public int Id { get; set; }
        public string UserId { get; set; }

        public string FullName { get; set; }
        public string ProfilePicture { get; set; }
        public string Gender { get; set; }
        public DateTime? DOB { get; set; }
        public string Address { get; set; }
        public string BloodType { get; set; }
        public string Allergies { get; set; }
        public string MedicalHistory { get; set; }
        public string CurrentMedications { get; set; }
        public string InsuranceProvider { get; set; }
        public string InsurancePolicyNumber { get; set; }
        public List<PatientAppointmentVM>? Appointments { get; set; }
    }

}
