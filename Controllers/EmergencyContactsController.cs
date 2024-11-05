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
    public class EmergencyContactsController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public EmergencyContactsController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/EmergencyContacts
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmergencyContact>>> GetEmergencyContacts()
        {
            return await _context.EmergencyContacts.ToListAsync();
        }

        // GET: api/EmergencyContacts/5
        [HttpGet("{id}")]
        public async Task<ActionResult<EmergencyContact>> GetEmergencyContact(int id)
        {
            var emergencyContact = await _context.EmergencyContacts.FindAsync(id);

            if (emergencyContact == null)
            {
                return NotFound();
            }

            return emergencyContact;
        }

        // PUT: api/EmergencyContacts/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutEmergencyContact(int id, EmergencyContact emergencyContact)
        {
            if (id != emergencyContact.ContactId)
            {
                return BadRequest();
            }

            _context.Entry(emergencyContact).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!EmergencyContactExists(id))
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

        // POST: api/EmergencyContacts
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<EmergencyContact>> PostEmergencyContact(EmergencyContact emergencyContact)
        {
            _context.EmergencyContacts.Add(emergencyContact);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetEmergencyContact", new { id = emergencyContact.ContactId }, emergencyContact);
        }

        // DELETE: api/EmergencyContacts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmergencyContact(int id)
        {
            var emergencyContact = await _context.EmergencyContacts.FindAsync(id);
            if (emergencyContact == null)
            {
                return NotFound();
            }

            _context.EmergencyContacts.Remove(emergencyContact);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool EmergencyContactExists(int id)
        {
            return _context.EmergencyContacts.Any(e => e.ContactId == id);
        }
    }
}
