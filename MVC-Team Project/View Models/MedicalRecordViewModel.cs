using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }

        [Required]
        public int DoctorId { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string RecordType { get; set; }

        [Required]
        [StringLength(2000)]
        public string Diagnosis { get; set; }

        [StringLength(2000)]
        public string Treatment { get; set; }

        [StringLength(2000)]
        public string Prescription { get; set; }

        [StringLength(2000)]
        public string TestResults { get; set; }

        [StringLength(2000)]
        public string Notes { get; set; }

        [StringLength(500)]
        public string AttachmentPath { get; set; }

        [DataType(DataType.Date)]
        public DateTime RecordDate { get; set; }

        public bool IsConfidential { get; set; }

        // Optional for display
        public string DoctorName { get; set; }
        public string PatientName { get; set; }
    }
}
