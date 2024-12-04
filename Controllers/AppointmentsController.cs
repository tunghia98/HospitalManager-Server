using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Models;
using EHospital.Models;
using EHospital.DTO;
using HospitalManagementSystem.QueryObjects;
using AutoMapper;
using AutoMapper.QueryableExtensions;

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentsController(HospitalDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly HospitalDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        // GET: api/Appointments
        [HttpGet]
        public async Task<ActionResult<Paginated<AppointmentDTO>>> GetAppointments([FromQuery] AppointmentQuery query)
        {
            return await query.ApplyFilter(_context.Appointments
            .Include(a => a.Patient)
            .Include(a => a.Doctor)
            )
            .ProjectTo<AppointmentDTO>(_mapper.ConfigurationProvider)
            .ToPaginatedAsync(query);
        }
        // GET: api/Appointments/5
        [HttpGet("{id}")]

        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);

            if (appointment == null)
            {
                return NotFound();
            }

            return appointment;
        }

        // PUT: api/Appointments/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutAppointment(int id, Appointment appointment)
        {
            if (id != appointment.AppointmentId)
            {
                return BadRequest();
            }

            _context.Entry(appointment).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AppointmentExists(id))
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

        // POST: api/Appointments
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Appointment>> PostAppointment(Appointment appointment)
        {
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetAppointment", new { id = appointment.AppointmentId }, appointment);
        }

        // DELETE: api/Appointments/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            var appointment = await _context.Appointments.FindAsync(id);
            if (appointment == null)
            {
                return NotFound();
            }

            _context.Appointments.Remove(appointment);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet("doctor/{doctorId}/date/{appointmentDate}")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointmentsByDoctorAndDate(int doctorId, DateTime appointmentDate)
        {
            // Lấy danh sách tất cả lịch hẹn của bác sĩ theo ID trong ngày đã cho
            var appointments = await _context.Appointments
                .Where(a => a.DoctorId == doctorId && a.AppointmentDate.Date == appointmentDate.Date)
                .ToListAsync();

            if (appointments == null || !appointments.Any())
            {
                return NotFound("Không tìm thấy lịch hẹn nào cho bác sĩ này trong ngày đã chỉ định.");
            }

            return Ok(appointments);
        }
        private bool AppointmentExists(int id)
        {
            return _context.Appointments.Any(e => e.AppointmentId == id);
        }
        [HttpPost("Make")]
        public async Task<ActionResult<AppointmentDTO>> MakeAppointment(MakeAppointmentRequest request)
        {
            if (request.PatientId is null && request.Email is null)
            {
                return BadRequest("Vui lòng cung cấp thông tin bệnh nhân hoặc thông tin liên hệ.");
            }
            Patient? patient = null;
            if (request.PatientId is null)
            {
                var existingPatient = await _context.Patients.Where(p => p.Email == request.Email).FirstOrDefaultAsync();
                if (existingPatient != null)
                    return Unauthorized("Email đã được sử dụng cho một bệnh nhân khác.");
                patient = new Patient
                {
                    Name = request.Name,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                };
                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();
            }
            else
            {
                patient = await _context.Patients.Where(p => p.PatientId == request.PatientId).FirstOrDefaultAsync();
                if (patient is null)
                {
                    return NotFound("Không tìm thấy bệnh nhân.");
                }
            }
            var appointment = new Appointment
            {
                DoctorId = request.DoctorId,
                PatientId = patient.PatientId,
                AppointmentDate = request.AppointmentDate,
                Status = "SCHEDULED",
                CreatedAt = DateTime.Now,
                AppointmentTime = request.AppointmentDate.TimeOfDay,
            };
            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync();
            return _mapper.Map<AppointmentDTO>(appointment);
        }

        [HttpPatch("{id}/status")]
        public async Task<ActionResult<AppointmentDTO>> UpdateAppointmentStatus(int id, [FromBody] string status)
        {
            var appointment = await _context.Appointments.Where(a => a.AppointmentId == id).FirstOrDefaultAsync();
            if (appointment is null)
            {
                return NotFound("Không tìm thấy lịch hẹn.");
            }
            appointment.Status = status;
            await _context.SaveChangesAsync();
            return _mapper.Map<AppointmentDTO>(appointment);
        }
    }
}
