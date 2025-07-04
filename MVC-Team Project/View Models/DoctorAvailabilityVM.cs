namespace MVC_Team_Project.View_Models
{
    public class DoctorAvailabilityVM
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDuration { get; set; }
        public int MaxPatients { get; set; }
    }

}

