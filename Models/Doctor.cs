using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("Doctor")]
public partial class Doctor
{
    [Key]
    public int DoctorId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(100)]
    public string? Specialization { get; set; }

    public string? ImageUrl { get; set; } 

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    public int? DepartmentId { get; set; }

    // New Foreign Key for User
    public string? UserId { get; set; } // assuming the type in AspNetUsers is string (Id)

    [InverseProperty("Doctor")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [ForeignKey("DepartmentId")]
    [InverseProperty("Doctors")]
    public virtual Department? Department { get; set; }

    [InverseProperty("Doctor")]
    public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

    // Remove or modify this line to avoid FK constraint with User
    // [InverseProperty("Doctor")]
    // public virtual ICollection<User> Users { get; set; } = new List<User>();

    // Navigation property for User
    [ForeignKey("UserId")]
    public virtual IdentityUser? User { get; set; } // Assuming IdentityUser is the class for AspNetUsers
}
