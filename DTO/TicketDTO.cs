using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{


    public class TicketDTO
    {
        public Guid TicketId { get; set; } = Guid.NewGuid();

        public int PatientId { get; set; }

        public int? DoctorId { get; set; }

        public DateTime CreatedAt { get; set; }

        public virtual PatientDTO Patient { get; set; } = null!;
        public virtual DoctorDTO? Doctor { get; set; }

        public virtual ICollection<MessageDTO> Messages { get; set; } = new HashSet<MessageDTO>();
        public string LastMessage { get; set; } = null!;
        
        public DateTime? LastMessageAt { get; set; }

        public bool IsClosed { get; set; } = false;

        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Ticket, TicketDTO>().ForMember(
                    dest => dest.Messages,
                    opt => opt.Ignore()
                );

            }
        }
    }
}