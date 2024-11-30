using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using EHospital.DTO;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("Ticket")]
public partial class Ticket
{
    [Key]
    public Guid TicketId { get; set; } = Guid.NewGuid();

    public int PatientId { get; set; }

    public int? DoctorId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

    [ForeignKey("PatientId")]
    public virtual Patient Patient { get; set; } = null!;
    [ForeignKey("DoctorId")]
    public virtual Doctor? Doctor { get; set; }

    public virtual ICollection<Message> Messages { get; set; } = new HashSet<Message>();
    public string LastMessage { get; set; } = null!;
    public DateTime? LastMessageAt { get; set; }

    public bool IsClosed { get; set; } = false;
}