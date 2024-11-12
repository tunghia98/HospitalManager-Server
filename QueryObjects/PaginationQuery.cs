

using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace HospitalManagementSystem.QueryObjects
{
    public class PaginationQuery
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public int Skip => (Page - 1) * PageSize;
        public int Take => PageSize;
    }

}