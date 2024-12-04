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
    public class InvoicesController : ControllerBase
    {
        private readonly HospitalDbContext _context;

        public InvoicesController(HospitalDbContext context)
        {
            _context = context;
        }

        // GET: api/Invoices
        [HttpGet]
        public async Task<ActionResult<Paginated<Invoice>>> GetInvoices([FromQuery] InvoiceQuery query)
        {
            return await query.ApplyFilter(_context.Invoices).ToPaginatedAsync(query);
        }

        // GET: api/Invoices/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Invoice>> GetInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);

            if (invoice == null)
            {
                return NotFound();
            }

            return invoice;
        }

        // PUT: api/Invoices/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutInvoice(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                return BadRequest();
            }
            var invoiceToUpdate = await _context.Invoices.Where(i => i.InvoiceId == id).FirstOrDefaultAsync();
            if (invoiceToUpdate == null)
            {
                return NotFound();
            }
            invoiceToUpdate.PatientId = invoice.PatientId;
            invoiceToUpdate.AppointmentId = invoice.AppointmentId;
            invoiceToUpdate.InvoiceDate = invoice.InvoiceDate;
            invoiceToUpdate.Status = invoice.Status;
            invoiceToUpdate.TotalAmount = invoice.TotalAmount;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        // POST: api/Invoices
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Invoice>> PostInvoice(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetInvoice", new { id = invoice.InvoiceId }, invoice);
        }

        // DELETE: api/Invoices/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteInvoice(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null)
            {
                return NotFound();
            }

            _context.Invoices.Remove(invoice);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool InvoiceExists(int id)
        {
            return _context.Invoices.Any(e => e.InvoiceId == id);
        }
    }
    
}
