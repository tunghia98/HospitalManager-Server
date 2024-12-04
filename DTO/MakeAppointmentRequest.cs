using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class MakeAppointmentRequest
    {

        public int AppointmentId { get; set; }
        public int DoctorId { get; set; }

        public DateTime AppointmentDate { get; set; }
        public int? PatientId { get; set; }

        public string? Email { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Name { get; set; }
    }
}