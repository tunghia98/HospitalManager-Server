using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class DiagnosisDTO
    {
        public int DiagnosisId { get; set; }

        public int AppointmentId { get; set; }

        public DateTime DiagnosisDate { get; set; }

        public string? Description { get; set; }

        public string? Notes { get; set; }
        public  AppointmentDTO? Appointment { get; set; } 
        public class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Diagnosis, DiagnosisDTO>();
                CreateMap<DiagnosisDTO, Diagnosis>().ForMember(dest => dest.Appointment, opt => opt.Ignore());
   
            }
        }
    }
}