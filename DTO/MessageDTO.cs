using AutoMapper;
using EHospital.DTO;
using HospitalManagementSystem.Models;
namespace EHospital.DTO;
public class MessageDTO
{

    public Guid MessageId { get; set; }
    public Guid TicketId { get; set; }

    public  TicketDTO Ticket { get; set; } = null!;
    
    public string UserId { get; set; } = null!;
    public  UserProfileDTO User { get; set; } = null!;

    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
    public string Content { get; set; } = null!;

    private class MessageDTOProfile : Profile
    {
        public MessageDTOProfile()
        {
            CreateMap<Message, MessageDTO>();
        }
    }
}