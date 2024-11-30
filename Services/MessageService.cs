using System.Data.Entity;

namespace HospitalManagementSystem.Services;

using System;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.ExceptionServices;
using System.Security.Claims;
using System.Text;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using EHospital.DTO;
using EHospital.Models;
using HospitalManagementSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR;
using Microsoft.IdentityModel.Tokens;

public class MessageService(HospitalDbContext context, TokenService tokenService, IMapper mapper) : Hub
{
    private readonly HospitalDbContext _context = context;
    private readonly TokenService _tokenService = tokenService;
    private readonly IMapper _mapper = mapper;

    public override async Task OnConnectedAsync()
    {
        Console.WriteLine("Connected");
        await base.OnConnectedAsync();
    }
    public async Task<TicketDTO> OpenTicket(int patientId, string message)
    {
        Console.WriteLine("OpenTicket for patientId: " + patientId);
        var ticket = new Ticket
        {
            PatientId = patientId,
            CreatedAt = DateTime.Now,
            LastMessageAt = DateTime.Now,
            LastMessage = message,
        };
        _context.Tickets.Add(ticket);
        var user = await _context.Patients.FindAsync(patientId);
        var firstMessage = new Message
        {
            TicketId = ticket.TicketId,
            Content = message,
            UserId = user!.UserId!,
            CreatedAt = DateTime.Now,
            IsRead = false,
            MessageId = Guid.NewGuid(),
        };
        _context.Messages.Add(firstMessage);
        await _context.SaveChangesAsync();
        await Clients.All.SendAsync("NewTicket", ticket.TicketId);
        return new TicketDTO
        {
            TicketId = ticket.TicketId,
        };
    }

    public async Task<Ticket> AssignDoctor(Guid ticketId, int doctorId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        ticket!.DoctorId = doctorId;
        await _context.SaveChangesAsync();
        return ticket;
    }
    public async Task<Ticket> CloseTicket(Guid ticketId)
    {
        var ticket = await _context.Tickets.FindAsync(ticketId);
        ticket!.IsClosed = true;
        await _context.SaveChangesAsync();
        return ticket;
    }
    public async Task<Message> SendMessage(Guid ticketId, string content, string userId)
    {
        Console.WriteLine("SendMessage for ticketId: " + ticketId);
        var message = new Message
        {
            TicketId = ticketId,
            Content = content,
            UserId = userId,
            CreatedAt = DateTime.Now,
            IsRead = false,
        };
        _context.Messages.Add(message);
        var ticket = await _context.Tickets.FindAsync(ticketId);
        ticket!.LastMessageAt = DateTime.Now;
        ticket.LastMessage = content;
        await _context.SaveChangesAsync();
        await Clients.All.SendAsync("NewMessage", ticketId);
        return message;
    }
    public async Task<Paginated<MessageDTO>> GetMessages(Guid ticketId, int page, int pageSize)
    {
        var query = _context.Messages
            .Where(m => m.TicketId == ticketId)
            .ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);
        return await query.ToPaginatedAsync(page, pageSize);
    }
    public async Task<Message> ReadMessage(Guid messageId)
    {
        var message = await _context.Messages.FindAsync(messageId);
        message!.IsRead = true;
        await _context.SaveChangesAsync();
        return message;
    }
    public async Task<Paginated<TicketDTO>> GetDoctorTickets(int doctorId, int page, int pageSize)
    {
        var query = _context.Tickets
            .Include(t => t.Patient)
            .Where(t => t.DoctorId == doctorId)
            .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider).OrderByDescending(t => t.LastMessageAt);
        return await query.ToPaginatedAsync(page, pageSize);
    }
    public async Task<Paginated<TicketDTO>> GetPatientTickets(int patientId, int page, int pageSize)
    {
        var query = _context.Tickets
            .Include(t => t.Doctor)
            .Where(t => t.PatientId == patientId)
            .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider).OrderByDescending(t => t.LastMessageAt);
        return await query.ToPaginatedAsync(page, pageSize);
    }
    public class LastMessageInfo
    {
        public Guid TicketId { get; set; }
        public Message? LastMessage { get; set; }
    }
    public async Task<Paginated<TicketDTO>> GetUnassignedTickets(int page, int pageSize)
    {
        var query = _context.Tickets
            .Include(t => t.Patient)
            .Where(t => t.DoctorId == null)
            .ProjectTo<TicketDTO>(_mapper.ConfigurationProvider)
            .OrderByDescending(t => t.LastMessageAt);

        return await query.ToPaginatedAsync(page, pageSize);
    }

}