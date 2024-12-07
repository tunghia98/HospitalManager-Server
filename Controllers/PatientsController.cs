using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using HospitalManagementSystem.Models;
using EHospital.DTO;
using Microsoft.AspNetCore.Identity;
using EHospital.Models;
using HospitalManagementSystem.Services;
using AutoMapper;
using HospitalManagementSystem.QueryObjects;
using AutoMapper.QueryableExtensions;

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController(UserManager<IdentityUser> userManager, HospitalDbContext context, UserService userService, IMapper mapper)
     : ControllerBase
    {
        private readonly HospitalDbContext _context = context;
        private readonly UserService _userService = userService;
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IMapper _mapper = mapper;

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<Paginated<PatientDTO>>> GetPatients([FromQuery] PatientQuery query)
        {
            return await query.ApplyFilter(_context.Patients).ProjectTo<PatientDTO>(_mapper.ConfigurationProvider).ToPaginatedAsync(query);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // PUT: api/Patients/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, Patient patient)
        {
            if (id != patient.PatientId)
            {
                return BadRequest();
            }
            var existingPatient = await _context.Patients.Where(p => p.PatientId == id).FirstOrDefaultAsync();
            if (existingPatient != null)
            {
                var existingUser = await _context.Users.Where(u => u.Id == existingPatient.UserId).FirstOrDefaultAsync();
                if (existingUser != null)
                {
                    existingUser.PhoneNumber = patient.PhoneNumber;
                }
                existingPatient.DateOfBirth = patient.DateOfBirth;
                existingPatient.Name = patient.Name;
                existingPatient.PhoneNumber = patient.PhoneNumber;
                existingPatient.HealthInsurance = patient.HealthInsurance;
            }
            else
            {
                return NotFound();
            }

            await _context.SaveChangesAsync();


            return NoContent();
        }

        // POST: api/Patients
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(Patient patient)
        {
            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.PatientId }, patient);
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("patientregister")]
        public async Task<IActionResult> PatientRegister(PatientRegistration registration)
        {
            var result = await _userService.RegisterUser(registration);
            return Ok(result);
        }
        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.PatientId == id);
        }
        [HttpGet("{id}/Appointments")]
        public async Task<ActionResult<Paginated<AppointmentDTO>>> GetPatientAppointments(int id, [FromQuery] AppointmentQuery query)
        {
            query.PatientId.Equal = id;
            return await query.ApplyFilter(_context.Appointments
                                .Include(a => a.Patient)
                                .Include(a => a.Doctor)
                                .ThenInclude(a => a.Department)
                                .Include(a => a.Diagnoses)
                                .Include(a => a.Invoices)
             )
             .ProjectTo<AppointmentDTO>(_mapper.ConfigurationProvider)
             .ToPaginatedAsync(query);
        }
    }
}
