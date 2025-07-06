namespace MVC_Team_Project.View_Models
{
    public class DoctorAvailabilityVM
    {
        public DateTime AvailableDate { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int SlotDuration { get; set; }
        public int MaxPatients { get; set; }
        public bool IsBooked { get; set; } // Optional for filtering/booked tag

    }

}

