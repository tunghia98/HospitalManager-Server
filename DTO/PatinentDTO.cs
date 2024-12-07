using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class PatientDTO
    {
        public int PatientId { get; set; }

        public string Name { get; set; } = null!;

        public DateOnly DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public string? Gender { get; set; }

        public string? HealthInsurance { get; set; }

        // New foreign key for User
        public string? UserId { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; } = [];

        public virtual ICollection<EmergencyContact> EmergencyContacts { get; set; } = [];



        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Patient, PatientDTO>();
            }
        }
    }
}