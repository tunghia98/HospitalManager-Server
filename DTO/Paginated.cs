

using System.Collections.ObjectModel;
using Microsoft.EntityFrameworkCore;

namespace EHospital.DTO
{
    public class Paginated<T> where T : class
    {
        public int TotalItems { get; set; }
        public IEnumerable<T> Data { get; set; } = [];
    }
    public static class PaginationExtensions
    {
        public static async Task<Paginated<T>> ToPaginatedAsync<T>(this IQueryable<T> items, int page, int limit) where T : class
        {

            var totalItems = await items.AsNoTracking().CountAsync();
            var data = await items.AsNoTracking().Skip((page - 1) * limit).Take(limit).ToListAsync();

            return new Paginated<T>
            {
                TotalItems = totalItems,
                Data = data
            };
        }
    }
}