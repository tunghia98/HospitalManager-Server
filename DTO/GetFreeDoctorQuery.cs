
using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class GetFreeDoctorsQuery
    {
        public DateTime AppointmentDate { get; set; }
        public int? DepartmentId { get; set; }


    }
}