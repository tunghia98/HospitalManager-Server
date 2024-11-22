using HospitalManagementSystem.Libs.Filters;
using HospitalManagementSystem.Models;

namespace HospitalManagementSystem.QueryObjects
{

    public class PatientQuery : PaginationQuery, IFilterObject<Patient>
    {
        public string? Search { get; set; }
        public StringFilter Gender { get; set; } = new StringFilter();

        public IQueryable<Patient> ApplyFilter(IQueryable<Patient> query)
        {
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(p => p.Name.Contains(Search) || p.PhoneNumber!.Contains(Search) || p.Email!.Contains(Search) || p.HealthInsurance!.Contains(Search));
            }
            query = Gender.ApplyFilter(query, x => x.Gender!);
            return query;
        }
    }
}