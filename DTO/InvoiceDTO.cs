using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class InvoiceDTO
    {
        public int InvoiceId { get; set; }

        public int PatientId { get; set; }

        public int? AppointmentId { get; set; }

        public DateTime InvoiceDate { get; set; }

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = null!;
        public PatientDTO Patient { get; set; } = null!;
        private class MappingProfile : Profile
        {
            public MappingProfile()
            {
                CreateMap<Invoice, InvoiceDTO>();
                CreateMap<InvoiceDTO, Invoice>();
            }
        }
    }
}
