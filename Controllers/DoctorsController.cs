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
using HospitalManagementSystem.QueryObjects;
using AutoMapper.QueryableExtensions;
using AutoMapper;

namespace EHospital.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class DoctorsController(UserManager<IdentityUser> userManager, HospitalDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly HospitalDbContext _context = context;
        private readonly IMapper _mapper = mapper;

        // GET: api/Doctors
        [HttpGet]
        public async Task<ActionResult<Paginated<DoctorDTO>>> GetDoctors([FromQuery] DoctorQuery query)
        {
            return await query.ApplyFilter(_context.Doctors.Include(x => x.Department))
            .ProjectTo<DoctorDTO>(_mapper.ConfigurationProvider)
            .ToPaginatedAsync(query);
        }

        // GET: api/Doctors/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DoctorDTO>> GetDoctor(int id)
        {
            var doctor = await _context.Doctors
            .Include(x => x.Department)
            .Where(x => x.DoctorId == id)
            .ProjectTo<DoctorDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();


            if (doctor == null)
            {
                return NotFound();
            }

            return doctor;
        }

        // PUT: api/Doctors/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDoctor(int id, Doctor doctor)
        {
            if (id != doctor.DoctorId)
            {
                return BadRequest();
            }

            _context.Entry(doctor).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DoctorExists(id))
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

        // POST: api/Doctors
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Doctor>> PostDoctor(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDoctor", new { id = doctor.DoctorId }, doctor);
        }

        // DELETE: api/Doctors/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDoctor(int id)
        {
            var doctor = await _context.Doctors.FindAsync(id);
            if (doctor == null)
            {
                return NotFound();
            }

            _context.Doctors.Remove(doctor);
            await _context.SaveChangesAsync();

            return NoContent();
        }
        [HttpPost("doctorregister")]
        public async Task<IActionResult> DoctorsRegister(DoctorRegistration registration)
        {
            // Step 1: Create the Identity User
            var user = new IdentityUser
            {
                UserName = registration.Email,
                Email = registration.Email
            };

            var result = await _userManager.CreateAsync(user, registration.Password);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors);
            }
            // Step 2: Assign the Doctor role
            await _userManager.AddToRoleAsync(user, "Doctor");

            // Step 3: Create the Doctor entry
            var doctor = new Doctor
            {
                Name = registration.Name,
                Specialization = registration.Specialization,
                PhoneNumber = registration.PhoneNumber,
                Email = registration.Email,
                DepartmentId = registration.DepartmentId,
                UserId = user.Id // Link the doctor with the created Identity user
            };

            _context.Doctors.Add(doctor);
            await _context.SaveChangesAsync();

            return Ok("Doctor registered successfully");
        }

        private bool DoctorExists(int id)
        {
            return _context.Doctors.Any(e => e.DoctorId == id);
        }
    }
}
