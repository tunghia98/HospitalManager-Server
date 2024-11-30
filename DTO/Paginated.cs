

using System.Collections.ObjectModel;
using HospitalManagementSystem.QueryObjects;
using Microsoft.EntityFrameworkCore;

namespace EHospital.DTO
{
    public class Paginated<T> where T : class
    {
        public int TotalItems { get; set; }
        public IEnumerable<T> Data { get; set; } = [];
        public int NextPage { get; set; }
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
                Data = data,
                NextPage = page + 1
            };
        }
         public static async Task<Paginated<T>> ToPaginatedAsync<T>(this IQueryable<T> items, PaginationQuery query) where T : class
        {
            var page = query.Page;
            var limit = query.PageSize;
            var sortBy = query.SortBy;
            var sortOrder = query.SortOrder;
            var totalItems = await items.AsNoTracking().CountAsync();
            List<T> data;
            Console.WriteLine("SortBy: " + sortBy);
            if (sortBy is not null)
            {
                data = sortOrder is null || sortOrder.Equals("ASC", StringComparison.OrdinalIgnoreCase)
                    ? await items.AsNoTracking().OrderBy(e => EF.Property<object>(e, sortBy)).Skip((page - 1) * limit).Take(limit).ToListAsync()
                    : await items.AsNoTracking().OrderByDescending(e => EF.Property<object>(e, sortBy)).Skip((page - 1) * limit).Take(limit).ToListAsync();
            }
            else
            {
                data = await items.AsNoTracking().Skip((page - 1) * limit).Take(limit).ToListAsync();
            }

            return new Paginated<T>
            {
                TotalItems = totalItems,
                Data = data,
                NextPage = page + 1
            };
        }
    }
}