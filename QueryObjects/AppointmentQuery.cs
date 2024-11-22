

using EHospital.DTO;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Libs.Filters;

namespace HospitalManagementSystem.QueryObjects
{

    public class AppointmentQuery : PaginationQuery, IFilterObject<Appointment>
    {
        public NumberFilter<int> PatientId { get; set; } = new NumberFilter<int>();

        public NumberFilter<int> DoctorId { get; set; } = new NumberFilter<int>();

        public DateTimeFilter AppointmentDate { get; set; } = new DateTimeFilter();

        public StringFilter Status { get; set; } = new StringFilter();

        public DateTimeFilter CreatedAt { get; set; } = new DateTimeFilter();
        public NumberFilter<int> AppointmentTime { get; set; } = new NumberFilter<int>();
        public string? Search { get; set; }
        public IQueryable<Appointment> ApplyFilter(IQueryable<Appointment> query)
        {
            query = PatientId.ApplyFilter(query, x => x.PatientId);
            query = DoctorId.ApplyFilter(query, x => x.DoctorId);
            query = AppointmentDate.ApplyFilter(query, x => x.AppointmentDate);
            query = Status.ApplyFilter(query, x => x.Status);
            query = CreatedAt.ApplyFilter(query, x => x.CreatedAt!.Value);
            query = AppointmentTime.ApplyFilter(query, x => x.AppointmentTime.Milliseconds);
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(x => x.Patient.Name.Contains(Search) || x.Doctor.Name.Contains(Search)||x.Patient.PhoneNumber!.Contains(Search));
            }
            return query;
        }
    }
}