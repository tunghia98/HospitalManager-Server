using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EHospital.Models;
using HospitalManagementSystem.Models;
using EHospital.DTO;
using HospitalManagementSystem.QueryObjects;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace EHospital.Controllers
{
    public class DashboardDTO
    {
        public List<WeeklyAppointmentDTO> WeeklyAppointments { get; set; } =[];
        public List<WeeklyRevenueDTO> WeeklyRevenue { get; set; } =[];
        public List<AppointmentByDepartmentDTO> AppointmentsByDepartment { get; set; } =[];
    }

    public class WeeklyAppointmentDTO
    {
        public string DayOfWeek { get; set; } =null!;
        public int Count { get; set; }
    }

    public class WeeklyRevenueDTO
    {
        public string DayOfWeek { get; set; } =null!;
        public decimal Amount { get; set; }
    }

    public class AppointmentByDepartmentDTO
    {
        public string DepartmentName { get; set; } =null!;
        public int Count { get; set; }
    }
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController(HospitalDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly HospitalDbContext _context = context;
        private readonly IMapper _mapper = mapper;


        [HttpGet]
        public async Task<ActionResult<DashboardDTO>> GetDashboardData()
        {
            var startDate = DateTime.Today.AddDays(-(int)DateTime.Today.DayOfWeek);
            var endDate = startDate.AddDays(7);

            var weeklyAppointments = await _context.Appointments
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate < endDate)
                .GroupBy(a => a.AppointmentDate.DayOfWeek)
                .Select(g => new WeeklyAppointmentDTO
                {
                    DayOfWeek = g.Key.ToString(),
                    Count = g.Count()
                })
                .ToListAsync();

            var weeklyRevenue = await _context.Invoices
                .Where(i => i.InvoiceDate >= startDate && i.InvoiceDate < endDate)
                .GroupBy(i => i.InvoiceDate.DayOfWeek)
                .Select(g => new WeeklyRevenueDTO
                {
                    DayOfWeek = g.Key.ToString(),
                    Amount = g.Sum(i => i.TotalAmount)
                })
                .ToListAsync();

            var appointmentsByDepartment = await _context.Appointments
                .Include(a => a.Doctor)
                .ThenInclude(d => d.Department)
                .Where(a => a.AppointmentDate >= startDate && a.AppointmentDate < endDate)
                .GroupBy(a => a.Doctor.Department.DepartmentName)
                .Select(g => new AppointmentByDepartmentDTO
                {
                    DepartmentName = g.Key,
                    Count = g.Count()
                })
                .ToListAsync();
            return Ok(new DashboardDTO
            {
                WeeklyAppointments = weeklyAppointments,
                WeeklyRevenue = weeklyRevenue,
                AppointmentsByDepartment = appointmentsByDepartment
            });
        }
    }
}