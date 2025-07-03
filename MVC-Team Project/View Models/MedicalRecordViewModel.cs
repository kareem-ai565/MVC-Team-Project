using System;
using System.ComponentModel.DataAnnotations;

namespace MVC_Team_Project.View_Models
{
    public class MedicalRecordViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Doctor is required.")]
        public int DoctorId { get; set; }

        [Required(ErrorMessage = "Please select a patient.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Record type is required.")]
        [StringLength(50, ErrorMessage = "Record type must not exceed 50 characters.")]
        public string RecordType { get; set; }

        [Required(ErrorMessage = "Diagnosis is required.")]
        [StringLength(2000, ErrorMessage = "Diagnosis must not exceed 2000 characters.")]
        public string Diagnosis { get; set; }

        [StringLength(2000, ErrorMessage = "Treatment must not exceed 2000 characters.")]
        public string Treatment { get; set; }

        [StringLength(2000, ErrorMessage = "Prescription must not exceed 2000 characters.")]
        public string Prescription { get; set; }

        [StringLength(2000, ErrorMessage = "Test results must not exceed 2000 characters.")]
        public string TestResults { get; set; }

        [StringLength(2000, ErrorMessage = "Notes must not exceed 2000 characters.")]
        public string Notes { get; set; }

        [StringLength(500, ErrorMessage = "Attachment path must not exceed 500 characters.")]
        public string? AttachmentPath { get; set; }

        [Required(ErrorMessage = "Record date is required.")]
        [DataType(DataType.Date, ErrorMessage = "Please enter a valid date.")]
        public DateTime RecordDate { get; set; }

        public bool IsConfidential { get; set; }

        // Optional for display
        public string? DoctorName { get; set; }
        public string? PatientName { get; set; }
    }
}
