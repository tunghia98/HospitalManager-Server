using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("Invoice")]
public partial class Invoice
{
    [Key]
    public int InvoiceId { get; set; }

    public int PatientId { get; set; }

    public int? AppointmentId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime InvoiceDate { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal TotalAmount { get; set; }

    [StringLength(50)]
    public string Status { get; set; } = null!;

    [ForeignKey("PatientId")]
    [InverseProperty("Invoices")]
    public virtual Patient Patient { get; set; } = null!;

    [ForeignKey("AppointmentId")]
    [InverseProperty("Invoices")]
    public virtual Appointment Appointment { get; set; } = null!;
}
