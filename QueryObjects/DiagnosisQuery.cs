

using EHospital.DTO;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Libs.Filters;

namespace HospitalManagementSystem.QueryObjects
{

    public class DiagnosisQuery : PaginationQuery, IFilterObject<Diagnosis>
    {

        public NumberFilter<int> AppointmentId { get; set; } = new NumberFilter<int>();

        public DateTimeFilter DiagnosisDate { get; set; } = new DateTimeFilter();

        public NumberFilter<int> PatientId { get; set; } = new NumberFilter<int>();
        public NumberFilter<int> DoctorId { get; set; } = new NumberFilter<int>();
        public string? Search { get; set; }
        public IQueryable<Diagnosis> ApplyFilter(IQueryable<Diagnosis> query)
        {
            query = AppointmentId.ApplyFilter(query, x => x.AppointmentId);
            query = DiagnosisDate.ApplyFilter(query, x => x.DiagnosisDate);
            query = PatientId.ApplyFilter(query, x => x.Appointment!.PatientId);
            query = DoctorId.ApplyFilter(query, x => x.Appointment!.DoctorId);
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(x => x.Description!.Contains(Search) || x.Notes!.Contains(Search));
            }
            return query;
        }
    }

}