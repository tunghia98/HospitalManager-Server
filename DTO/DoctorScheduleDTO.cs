
using AutoMapper;
using HospitalManagementSystem.Models;

namespace EHospital.DTO
{
    public class DoctorScheduleDTO
    {
        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }

        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }


        public DoctorDTO Doctor { get; set; } = null!;

        public class DoctorScheduleProfile : Profile
        {
            public DoctorScheduleProfile()
            {
                CreateMap<DoctorSchedule, DoctorScheduleDTO>();
            }
        }
    }
     public class DoctorScheduleCreateDTO
    {
        public int ScheduleId { get; set; }

        public int DoctorId { get; set; }

        public int DayOfWeek { get; set; }

        public TimeOnly StartTime { get; set; }

        public TimeOnly EndTime { get; set; }
    }
}