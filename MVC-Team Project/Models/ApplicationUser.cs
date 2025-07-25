﻿#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace MVC_Team_Project.Models;

//[Index("Email", Name = "IX_Users_Email")]
[Index("IsActive", Name = "IX_Users_IsActive")]
//[Index("Username", Name = "IX_Users_Username")]
//[Index("Username", Name = "UQ__Users__536C85E49A39E348", IsUnique = true)]
//[Index("Email", Name = "UQ__Users__A9D1053454DD1EF6", IsUnique = true)]
public partial class ApplicationUser : IdentityUser<int>
{
    [Required]
    [StringLength(255)]
    public string FullName { get; set; }

    //[Required]
    //[StringLength(50)]
    //public string Username { get; set; }

    [Required]
    [StringLength(20)]
    //public string Role { get; set; }
    public virtual ICollection<IdentityUserRole<int>> UserRoles { get; set; } = new List<IdentityUserRole<int>>();

    //[Required]
    //[StringLength(255)]
    //public string Email { get; set; } 

    //[StringLength(20)]
    //public string Phone { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public bool IsActive { get; set; }

    public DateTime? LastLogin { get; set; }

    [StringLength(500)]
    public string ProfilePicture { get; set; }

    public bool EmailVerified { get; set; }

    [StringLength(255)]
    public string PasswordResetToken { get; set; }

    public DateTime? PasswordResetExpiry { get; set; }

    [InverseProperty("DeletedByNavigation")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("User")]
    public virtual ICollection<AuditLog> AuditLogs { get; set; } = new List<AuditLog>();

    [InverseProperty("User")]
    public virtual Doctor Doctor { get; set; }

    [InverseProperty("DeletedByNavigation")]
    public virtual ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();

    [InverseProperty("User")]
    public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    [InverseProperty("User")]
    public virtual Patient Patient { get; set; }
}