using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using AutoMapper;
using EHospital.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("Message")]
public partial class Message
{
    [Key]
    public Guid MessageId { get; set; }
    public Guid TicketId { get; set; }
    [ForeignKey("TicketId")]
    [InverseProperty("Messages")]
    public virtual Ticket Ticket { get; set; } = null!;
    
    public string UserId { get; set; } = null!;
    [ForeignKey("UserId")]
    public virtual IdentityUser User { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    [StringLength(1000)]
    public string Content { get; set; } = null!;

}

