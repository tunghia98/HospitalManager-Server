﻿using System;
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
    public class DiagnosesController(HospitalDbContext context, IMapper mapper) : ControllerBase
    {
        private readonly HospitalDbContext _context = context;
        private readonly IMapper _mapper = mapper;


        // GET: api/Diagnoses
        [HttpGet]
        public async Task<ActionResult<Paginated<DiagnosisDTO>>> GetDiagnoses([FromQuery] DiagnosisQuery query)
        {
            return await query.ApplyFilter(
                _context.Diagnoses.Include(d => d.Appointment)
                .ThenInclude(a => a.Patient)
                .Include(d => d.Appointment.Doctor)
            )
            .ProjectTo<DiagnosisDTO>(_mapper.ConfigurationProvider)
            .ToPaginatedAsync(query);
        }

        // GET: api/Diagnoses/5
        [HttpGet("{id}")]
        public async Task<ActionResult<DiagnosisDTO>> GetDiagnosis(int id)
        {
            var diagnosis = await _context.Diagnoses
            .Include(d => d.Appointment)
            .ThenInclude(a => a.Patient)
            .Include(d => d.Appointment.Doctor)
            .Where(d => d.DiagnosisId == id)
            .ProjectTo<DiagnosisDTO>(_mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

            if (diagnosis == null)
            {
                return NotFound();
            }

            return diagnosis;
        }

        // PUT: api/Diagnoses/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutDiagnosis(int id, Diagnosis diagnosis)
        {
            if (id != diagnosis.DiagnosisId)
            {
                return BadRequest();
            }

            _context.Entry(diagnosis).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiagnosisExists(id))
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

        // POST: api/Diagnoses
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Diagnosis>> PostDiagnosis(DiagnosisDTO diagnosis)
        {
            var diagnosisEntity = _mapper.Map<DiagnosisDTO,Diagnosis >(diagnosis);
            _context.Diagnoses.Add(diagnosisEntity);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiagnosis", new { id = diagnosis.DiagnosisId }, diagnosis);
        }

        // DELETE: api/Diagnoses/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDiagnosis(int id)
        {
            var diagnosis = await _context.Diagnoses.FindAsync(id);
            if (diagnosis == null)
            {
                return NotFound();
            }

            _context.Diagnoses.Remove(diagnosis);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool DiagnosisExists(int id)
        {
            return _context.Diagnoses.Any(e => e.DiagnosisId == id);
        }
    }
}
