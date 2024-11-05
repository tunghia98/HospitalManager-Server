using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.Models;

[Table("DoctorSchedule")]
public partial class DoctorSchedule
{
    [Key]
    public int ScheduleId { get; set; }

    public int DoctorId { get; set; }

    public int DayOfWeek { get; set; }

    public TimeOnly StartTime { get; set; }

    public TimeOnly EndTime { get; set; }

    [ForeignKey("DoctorId")]
    [InverseProperty("DoctorSchedules")]
    public virtual Doctor Doctor { get; set; } = null!;
}
