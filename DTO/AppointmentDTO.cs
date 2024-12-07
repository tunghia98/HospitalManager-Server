using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class AppointmentDTO
    {

        public int AppointmentId { get; set; }

        public int PatientId { get; set; }

        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }

        public string Status { get; set; } = null!;

        public DateTime? CreatedAt { get; set; }
        public TimeSpan AppointmentTime { get; set; }

        public DoctorDTO Doctor { get; set; } = null!;
        public PatientDTO Patient { get; set; } = null!;
        public ICollection<DiagnosisDTO>? Diagnoses { get; set; }

        public ICollection<InvoiceDTO>? Invoices { get; set; }
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))

                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient))
                .AfterMap((src, dest) =>
                {
                    dest.Patient.Appointments = [];
                });

                CreateMap<AppointmentDTO, Appointment>()
                .ForMember(dest => dest.Doctor, opt => opt.Ignore())
                .ForMember(dest => dest.Patient, opt => opt.Ignore())
                .ForMember(dest => dest.Diagnoses, opt => opt.Ignore())
                .ForMember(dest => dest.Invoices, opt => opt.Ignore());
                
            }

        }
    }
}