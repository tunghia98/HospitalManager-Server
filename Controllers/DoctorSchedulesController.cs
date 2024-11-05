using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using EHospital.Models;
using HospitalManagementSystem.Models;

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
        public async Task<ActionResult<IEnumerable<DoctorSchedule>>> GetDoctorSchedules()
        {
            return await _context.DoctorSchedules.ToListAsync();
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
