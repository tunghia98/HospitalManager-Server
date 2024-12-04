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
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorSchedulesController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;

        public DoctorSchedulesController(HospitalDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }


        // GET: api/DoctorSchedules
        [HttpGet]
        public async Task<ActionResult<Paginated<DoctorScheduleDTO>>> GetAllDoctorSchedules([FromQuery] DoctorScheduleQuery query)
        {
            return await query.ApplyFilter(
                _context.DoctorSchedules.Include(ds => ds.Doctor)
            )
            .ProjectTo<DoctorScheduleDTO>(_mapper.ConfigurationProvider)
            .ToPaginatedAsync(query);
        }

        // GET: api/DoctorSchedules/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorScheduleDTO>> GetDoctorSchedule(int id)
        {
            var doctorSchedule = await _context.DoctorSchedules.Include(ds => ds.Doctor)
                .Where(ds => ds.ScheduleId == id)
                .ProjectTo<DoctorScheduleDTO>(_mapper.ConfigurationProvider)
                .FirstOrDefaultAsync();

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
        public async Task<IActionResult> PutDoctorSchedule(int id,[FromBody] DoctorScheduleCreateDTO doctorSchedule)
        {
            if (id != doctorSchedule.ScheduleId)
            {
                return BadRequest();
            }
            var existDoctorSchedule = await _context.DoctorSchedules.FindAsync(id);
            if (existDoctorSchedule == null)
            {
                return NotFound("Lịch làm việc không tồn tại.");
            }
            existDoctorSchedule.DayOfWeek = doctorSchedule.DayOfWeek;
            existDoctorSchedule.StartTime = doctorSchedule.StartTime;
            existDoctorSchedule.EndTime = doctorSchedule.EndTime;
            await _context.SaveChangesAsync();
            
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
        public async Task<ActionResult<DoctorSchedule>> BulkCreateDoctorSchedules(IEnumerable<DoctorSchedule> doctorSchedule)
        {
            _context.DoctorSchedules.AddRange(doctorSchedule);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctorSchedule", new
            {
                id =
                doctorSchedule.Select(ds => ds.ScheduleId).ToList()
            }, doctorSchedule);
        }
        [HttpPost("Bulk/{doctorId}")]
        public async Task<ActionResult<DoctorSchedule>> BulkUpdateDoctorSchedules(int doctorId,[FromBody] IEnumerable<DoctorScheduleCreateDTO> doctorSchedule)
        {
            // Kiểm tra xem bác sĩ có tồn tại không
            var doctor = await _context.Doctors.FindAsync(doctorId);
            if (doctor == null)
            {
                return NotFound("Bác sĩ không tồn tại.");
            }

         
            _context.DoctorSchedules.RemoveRange(_context.DoctorSchedules.Where(ds => ds.DoctorId == doctorId));
            var doctorSchedules = doctorSchedule.Select(ds => new DoctorSchedule
            {
                DoctorId = doctorId,
                DayOfWeek = ds.DayOfWeek,
                StartTime = ds.StartTime,
                EndTime = ds.EndTime
            });
            // Thêm lịch làm việc mới
            _context.DoctorSchedules.AddRange(doctorSchedules);
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
        public async Task<ActionResult<IEnumerable<DoctorDTO>>> GetFreeDoctors([FromQuery] GetFreeDoctorsQuery query)
        {
           int dateOfWeek = ((int)query.AppointmentDate.DayOfWeek);
           var time = TimeOnly.FromDateTime(query.AppointmentDate);
           var queryable =  _context.DoctorSchedules
                                    .Include(ds => ds.Doctor)
                                    .Where(ds => ds.DayOfWeek == dateOfWeek && ds.StartTime <= time && ds.EndTime >= time);
            if (query.DepartmentId.HasValue)
            {
                queryable = queryable.Where(ds => ds.Doctor.DepartmentId == query.DepartmentId);
            }
            var doctors = await queryable
                .Select(ds => ds.Doctor)
                .ProjectTo<DoctorDTO>(_mapper.ConfigurationProvider)
                .ToListAsync();
            return Ok(doctors);

        }
    }
}
