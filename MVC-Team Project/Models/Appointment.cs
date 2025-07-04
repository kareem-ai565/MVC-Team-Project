﻿#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MVC_Team_Project.Models;

[Index("AppointmentDate", Name = "IX_Appointments_AppointmentDate")]
[Index("AppointmentDate", "StartTime", Name = "IX_Appointments_DateTime")]
[Index("DoctorId", Name = "IX_Appointments_DoctorId")]
[Index("IsDeleted", Name = "IX_Appointments_IsDeleted")]
[Index("PatientId", Name = "IX_Appointments_PatientId")]
[Index("Status", Name = "IX_Appointments_Status")]
public partial class Appointment
{
    [Key]
    public int Id { get; set; }

    public int PatientId { get; set; }

    public int DoctorId { get; set; }

    public int? AvailabilityId { get; set; }

    [Column(TypeName = "date")]
    public DateTime AppointmentDate { get; set; }

    public TimeSpan StartTime { get; set; }

    public TimeSpan EndTime { get; set; }

    [Required]
    [StringLength(20)]
    public string Status { get; set; }

    [StringLength(1000)]
    public string PatientNotes { get; set; }

    [StringLength(1000)]
    public string Symptoms { get; set; }

    [StringLength(2000)]
    public string Diagnosis { get; set; }

    [StringLength(2000)]
    public string Prescription { get; set; }

    [StringLength(2000)]
    public string DoctorNotes { get; set; }

    [StringLength(500)]
    public string CancellationReason { get; set; }

    public bool IsFollowUpRequired { get; set; }

    [Column(TypeName = "date")]
    public DateTime? FollowUpDate { get; set; }

    public bool IsDeleted { get; set; }

    public DateTime? DeletedAt { get; set; }

    public int? DeletedBy { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [ForeignKey("AvailabilityId")]
    [InverseProperty("Appointments")]
    public virtual Availability Availability { get; set; }

    [ForeignKey("DeletedBy")]
    [InverseProperty("Appointments")]
    public virtual ApplicationUser DeletedByNavigation { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("Appointments")]
    public virtual Doctor Doctor { get; set; }

    [InverseProperty("Appointment")]
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    [ForeignKey("PatientId")]
    [InverseProperty("Appointments")]
    public virtual Patient Patient { get; set; }

    [InverseProperty("Appointment")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}