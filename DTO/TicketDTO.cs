using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class TicketDTO
    {
        public Guid TicketId { get; set; }
        public int PatientId { get; set; }
        public int? DoctorId { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsClosed { get; set; }
        public DoctorDTO? Doctor { get; set; }
        public PatientDTO Patient { get; set; } = null!;
        public MessageDTO? LastMessage { get; set; }
        public DateTime? LastMessageAt { get; set; }

        private class TicketDTOProfile : Profile
        {
            public TicketDTOProfile()
            {
                CreateMap<Ticket, TicketDTO>();
            }
        }
    }
}