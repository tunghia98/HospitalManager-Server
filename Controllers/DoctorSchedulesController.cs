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

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSchedulesController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public DoctorSchedulesController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/DoctorSchedules
        [HttpGet]
        public async Task<ActionResult<Paginated<DoctorSchedule>>> GetDoctorSchedules([FromQuery] DoctorScheduleQuery query)
        {
            return await query.ApplyFilter(_context.DoctorSchedules).ToPaginatedAsync(query);
        }

        // GET: api/DoctorSchedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorSchedule>> GetDoctorSchedule(int id)
        {
            var doctorSchedule = await _context.DoctorSchedules.FindAsync(id);

            if (doctorSchedule == null)
            {
                return NotFound();
            }

            return doctorSchedule;
        }
        // GET: api/Appointments/doctor-schedule/{doctorId}
        [HttpGet("doctor-schedule/{doctorId}")]
        public async Task<ActionResult<IEnumerable<DoctorSchedule>>> GetDoctorSchedules(int doctorId)
        {
            // Lấy tất cả lịch làm việc của bác sĩ theo ID
            var doctorSchedules = await _context.DoctorSchedules
                .Where(ds => ds.DoctorId == doctorId)
                .ToListAsync();

            if (doctorSchedules == null || !doctorSchedules.Any())
            {
                return NotFound("Không tìm thấy lịch làm việc cho bác sĩ này.");
            }

            // Nhóm theo DayOfWeek
            var groupedSchedules = doctorSchedules
                .GroupBy(ds => ds.DayOfWeek)
                .Select(g => new
                {
                    DayOfWeek = g.Key,
                    Schedules = g.ToList()
                })
                .ToList();

            return Ok(groupedSchedules);
        }

        // PUT: api/DoctorSchedules/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctorSchedule(int id, DoctorSchedule doctorSchedule)
        {
            if (id != doctorSchedule.ScheduleId)
            {
                return BadRequest();
            }

            _context.Entry(doctorSchedule).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorScheduleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/DoctorSchedules
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<DoctorSchedule>> PostDoctorSchedule(DoctorSchedule doctorSchedule)
        {
            _context.DoctorSchedules.Add(doctorSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctorSchedule", new { id = doctorSchedule.ScheduleId }, doctorSchedule);
        }
        [HttpPost("Bulk")]
        public async Task<ActionResult<DoctorSchedule>> BuildICrateDoctorSchedule(IEnumerable<DoctorSchedule> doctorSchedule)
        {
            _context.DoctorSchedules.AddRange(doctorSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctorSchedule", new
            {
                id =
                doctorSchedule.Select(ds => ds.ScheduleId).ToList()
            }, doctorSchedule);
        }


        // DELETE: api/DoctorSchedules/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctorSchedule(int id)
        {
            var doctorSchedule = await _context.DoctorSchedules.FindAsync(id);
            if (doctorSchedule == null)
            {
                return NotFound();
            }

            _context.DoctorSchedules.Remove(doctorSchedule);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DoctorScheduleExists(int id)
        {
            return _context.DoctorSchedules.Any(e => e.ScheduleId == id);
        }
    }
}
