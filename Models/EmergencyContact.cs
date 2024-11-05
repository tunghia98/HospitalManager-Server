using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("EmergencyContact")]
public partial class EmergencyContact
{
    [Key]
    public int ContactId { get; set; }

    public int PatientId { get; set; }

    [StringLength(100)]
    public string Name { get; set; } = null!;

    [StringLength(50)]
    public string? Relationship { get; set; }

    [StringLength(15)]
    public string? PhoneNumber { get; set; }

    [StringLength(100)]
    public string? Email { get; set; }

    [ForeignKey("PatientId")]
    [InverseProperty("EmergencyContacts")]
    public virtual Patient Patient { get; set; } = null!;
}
