using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
            
        public class AppointmentCompleteVM
        {
            public int Id { get; set; }
            public string? PatientName { get; set; }
            public string? DoctorName { get; set; }
            public DateTime AppointmentDate { get; set; }
            public TimeSpan StartTime { get; set; }
            public TimeSpan EndTime { get; set; }
            public string? Symptoms { get; set; }
            public decimal ConsultationFee { get; set; }

            [Required]
            public string Diagnosis { get; set; } = string.Empty;

            public string? DoctorNotes { get; set; }

            public string? Prescription { get; set; }

            public bool IsFollowUpRequired { get; set; }

            [DataType(DataType.Date)]
            public DateTime? FollowUpDate { get; set; }
        }



        




    
}
