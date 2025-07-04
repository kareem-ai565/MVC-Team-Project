﻿#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace MVC_Team_Project.Models;

[Index("IsVerified", Name = "IX_Doctors_IsVerified")]
[Index("SpecialtyId", Name = "IX_Doctors_SpecialtyId")]
[Index("UserId", Name = "IX_Doctors_UserId")]
[Index("UserId", Name = "UQ__Doctors__1788CC4D8EE7758A", IsUnique = true)]
[Index("LicenseNumber", Name = "UQ__Doctors__E88901668E0AAA1D", IsUnique = true)]
public partial class Doctor
{
    [Key]
    public int Id { get; set; }

    public int UserId { get; set; }

    public int SpecialtyId { get; set; }

    [StringLength(2000)]
    public string Bio { get; set; }

    [StringLength(500)]
    public string ClinicAddress { get; set; }

    [Required]
    [StringLength(100)]
    public string LicenseNumber { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal ConsultationFee { get; set; }

    public int ExperienceYears { get; set; }

    [StringLength(1000)]
    public string Education { get; set; }

    [StringLength(1000)]
    public string Certifications { get; set; }

    public bool IsVerified { get; set; }

    public DateTime? VerificationDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Doctor")]
    public virtual ICollection<Availability> Availabilities { get; set; } = new List<Availability>();

    [InverseProperty("Doctor")]
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    [InverseProperty("Doctor")]
    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

    [ForeignKey("SpecialtyId")]
    [InverseProperty("Doctors")]
    public virtual Specialty Specialty { get; set; }

    [ForeignKey("UserId")]
    [InverseProperty("Doctor")]


  

    public virtual ApplicationUser User { get; set; }

}