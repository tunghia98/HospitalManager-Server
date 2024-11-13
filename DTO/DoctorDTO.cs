
using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class DoctorDTO
    {

        public int DoctorId { get; set; }

        public string Name { get; set; } = null!;

        public string? Specialization { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Email { get; set; }

        public int? DepartmentId { get; set; }

        // New Foreign Key for User
        public string? UserId { get; set; } // assuming the type in AspNetUsers is string (Id)

        // public virtual ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();

        public DepartmentDTO? Department { get; set; }

        // public virtual ICollection<DoctorSchedule> DoctorSchedules { get; set; } = new List<DoctorSchedule>();

        // Remove or modify this line to avoid FK constraint with User
        // [InverseProperty("Doctor")]
        // public virtual ICollection<User> Users { get; set; } = new List<User>();

        // Navigation property for User
        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Doctor, DoctorDTO>();
            }
        }
    }
}