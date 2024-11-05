using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("Diagnosis")]
public partial class Diagnosis
{
    [Key]
    public int DiagnosisId { get; set; }

    public int AppointmentId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime DiagnosisDate { get; set; }

    [StringLength(255)]
    public string? Description { get; set; }

    [StringLength(255)]
    public string? Notes { get; set; }

    [ForeignKey("AppointmentId")]
    [InverseProperty("Diagnoses")]
    public virtual Appointment Appointment { get; set; } = null!;
}
