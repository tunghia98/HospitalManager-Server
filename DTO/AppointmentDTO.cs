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

        // public virtual ICollection<Diagnosis> Diagnoses { get; set; } = new List<Diagnosis>();
        public  DoctorDTO Doctor { get; set; } = null!;
        public PatientDTO Patient { get; set; } = null!;
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Appointment, AppointmentDTO>()
                .ForMember(dest => dest.Doctor, opt => opt.MapFrom(src => src.Doctor))
                .ForMember(dest => dest.Patient, opt => opt.MapFrom(src => src.Patient));
                
            }
        }

    }
}