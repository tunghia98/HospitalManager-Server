using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("Patient")]
public partial class Patient
{
    [Key]
    public int PatientId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    public DateOnly? DateOfBirth { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [StringLength(10)]
    public string? Gender { get; set; }

    [StringLength(100)]
    public string? HealthInsurance { get; set; }

    // New foreign key for User
    public string? UserId { get; set; } // Assuming the User ID is of type string

    [InverseProperty("Patient")]
    public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

    [InverseProperty("Patient")]
    public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = new List<EmergencyContact>();

    [InverseProperty("Patient")]
    public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

    // Remove the Users collection or keep it if you want to establish a relationship
    // [InverseProperty("Patient")]
    // public virtual ICollection<User> Users { get; set; } = new List<User>();

    // Navigation property for User
    [ForeignKey("UserId")]
    public virtual IdentityUser? User { get; set; } // Assuming IdentityUser is the class for AspNetUsers
}
