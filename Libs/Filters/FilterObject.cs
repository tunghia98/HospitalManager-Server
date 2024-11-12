
using System.Linq.Expressions;

namespace HospitalManagementSystem.Libs.Filters;


public interface IFilterObject<T>
{
    public IQueryable<T> ApplyFilter(IQueryable<T> query);
}