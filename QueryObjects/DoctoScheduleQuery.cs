

using EHospital.DTO;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Libs.Filters;

namespace HospitalManagementSystem.QueryObjects
{
    public class DoctorScheduleQuery : PaginationQuery, IFilterObject<DoctorSchedule>
    {

        public NumberFilter<int> DoctorId { get; set; } = new NumberFilter<int>();

        public NumberFilter<int> DayOfWeek { get; set; } = new NumberFilter<int>();

        public TimeOnlyFilter StartTime { get; set; } = new TimeOnlyFilter();

        public TimeOnlyFilter EndTime { get; set; } = new TimeOnlyFilter();
        public string? Search { get; set; }
        public IQueryable<DoctorSchedule> ApplyFilter(IQueryable<DoctorSchedule> query)
        {
            query = DoctorId.ApplyFilter(query, x => x.DoctorId);
            query = DayOfWeek.ApplyFilter(query, x => x.DayOfWeek);
            query = StartTime.ApplyFilter(query, x => x.StartTime);
            query = EndTime.ApplyFilter(query, x => x.EndTime);
            return query;
        }
    }
}