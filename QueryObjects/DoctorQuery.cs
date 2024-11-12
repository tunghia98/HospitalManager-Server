

using EHospital.DTO;
using HospitalManagementSystem.Models;
using HospitalManagementSystem.Libs.Filters;

namespace HospitalManagementSystem.QueryObjects
{
    public class DoctorQuery : PaginationQuery, IFilterObject<Doctor>
    {
        public StringFilter Name { get; set; } = new ();
        public StringFilter Specialization { get; set; } = new ();
        public StringFilter PhoneNumber { get; set; } = new ();
        public StringFilter Email { get; set; } = new ();
        public NumberFilter<int> DepartmentId = new();

        public IQueryable<Doctor> ApplyFilter(IQueryable<Doctor> query)
        {
            query = Name.ApplyFilter(query, x => x.Name);
            query = Specialization.ApplyFilter(query, x => x.Specialization);
            query = PhoneNumber.ApplyFilter(query, x => x.PhoneNumber);
            query = Email.ApplyFilter(query, x => x.Email);
            query = DepartmentId.ApplyFilter(query, x => x.DepartmentId ?? 0);
            // query.Select(x => x.Department);
            // query.Select(x => x.User);
            return query;
        }
    }
}