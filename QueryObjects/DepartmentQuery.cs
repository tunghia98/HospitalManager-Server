

using EHospital.DTO;
using HospitalManagementSystem.Models;
using  HospitalManagementSystem.Libs.Filters;
namespace HospitalManagementSystem.QueryObjects
{
    public class DepartmentQuery : PaginationQuery, IFilterObject<Department>
    {
        public StringFilter Name { get; set; } = new StringFilter();
        public StringFilter Description { get; set; } = new StringFilter();
        public string? Search {get;set;}
        public IQueryable<Department> ApplyFilter(IQueryable<Department> query)
        {
            query = Name.ApplyFilter(query, x => x.DepartmentName);
            query = Description.ApplyFilter(query, x => x.Description);
            if (!string.IsNullOrEmpty(Search))
            {
                query = query.Where(x => x.DepartmentName.Contains(Search) || x.Description.Contains(Search));
            }
            return query;
        }
    }
}