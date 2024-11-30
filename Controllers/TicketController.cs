using AutoMapper;
using AutoMapper.QueryableExtensions;
using Azure;
using EHospital.DTO;
using EHospital.Models;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.QueryObjects;
using HospitalManagementSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;

namespace EHospital.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TicketController : ControllerBase
    {
        private readonly HospitalDbContext _context;
        private readonly IMapper _mapper;
        private readonly MessageService _messageService;

        public TicketController(HospitalDbContext context, IMapper mapper, MessageService messageService)
        {
            _context = context;
            _mapper = mapper;
            _messageService = messageService;
        }

        [HttpPost("OpenTicket")]
        public async Task<ActionResult<TicketDTO>> OpenTicket(int patientId)
        {
            return Ok(await _messageService.OpenTicket(patientId,"Yêu cầu hỗ trợ"));
        }

        [HttpPost("AssignDoctor")]
        public async Task<ActionResult<Ticket>> AssignDoctor(Guid ticketId, int doctorId)
        {
            return await _messageService.AssignDoctor(ticketId, doctorId);
        }

        [HttpPost("CloseTicket")]
        public async Task<ActionResult<Ticket>> CloseTicket(Guid ticketId)
        {
            return await _messageService.CloseTicket(ticketId);
        }

        [HttpPost("SendMessage")]
        public async Task<ActionResult<Message>> SendMessage(Guid ticketId, string content, string userId)
        {
            return await _messageService.SendMessage(ticketId, content, userId);
        }

        [HttpGet("{ticketId}/Messages")]
        public async Task<ActionResult<Paginated<MessageDTO>>> GetMessages(Guid ticketId, [FromQuery] PaginationQuery query)
        {
            return await _context.Messages
                .Where(m => m.TicketId == ticketId)
                .OrderByDescending(m => m.CreatedAt)
                .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider)
                .ToPaginatedAsync(query);
        }

        [HttpGet("{ticketId}")]
        public async Task<ActionResult<TicketDTO>> GetTicket(Guid ticketId)
        {
            var ticket = await _context.Tickets
            .Include(t => t.Doctor)
            .ThenInclude(d => d.User)
            .Include(t => t.Patient)
            .Where(t => t.TicketId == ticketId).FirstOrDefaultAsync();
            if (ticket == null)
            {
                return NotFound();
            }
            return _mapper.Map<TicketDTO>(ticket);
        }

        [HttpGet("DoctorTickets")]
        public async Task<ActionResult<Paginated<Ticket>>> GetDoctorTickets(int doctorId, [FromQuery] PaginationQuery query)
        {
            return Ok(await _messageService.GetDoctorTickets(doctorId, query.Page, query.PageSize));
        }

        [HttpPost("{messageId}/Read")]
        public async Task<ActionResult<Message>> ReadMessage(Guid messageId)
        {
            return await _messageService.ReadMessage(messageId);
        }

        [HttpGet("PatientTickets")]
        public async Task<ActionResult<IEnumerable<Ticket>>> GetPatientTickets(int patientId, [FromQuery] PaginationQuery query)
        {
            return Ok(await _messageService.GetPatientTickets(patientId, query.Page, query.PageSize));
        }

        [HttpGet("UnAssignedTickets")]
        public async Task<ActionResult<Paginated<Ticket>>> GetUnAssignedTickets([FromQuery] PaginationQuery query)
        {
            return Ok(await _messageService.GetUnassignedTickets(query.Page, query.PageSize));
        }
    }
}