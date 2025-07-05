using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
   
        public class AppointmentCreateVM
        {
            [Required]
            public int PatientId { get; set; }

            [Required]
            public int DoctorId { get; set; }

            [Required]
            [DataType(DataType.Date)]
            public DateTime AppointmentDate { get; set; }

            [Required]
            [DataType(DataType.Time)]
            public TimeSpan StartTime { get; set; }

            [Required]
            [DataType(DataType.Time)]
            public TimeSpan EndTime { get; set; }

            public string? Symptoms { get; set; }

            public string? PatientNotes { get; set; }

            [Required]
            [Range(0, double.MaxValue)]
            public decimal ConsultationFee { get; set; }
        }



        




    
}
