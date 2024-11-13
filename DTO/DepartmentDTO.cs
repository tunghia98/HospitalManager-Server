using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class DepartmentDTO
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; } = null!;

        public string? Description { get; set; }


        private class Mapping : Profile
        {
            public Mapping()
            {
                CreateMap<Department, DepartmentDTO>();
            }
        }

    }
}