using Microsoft.EntityFrameworkCore;

namespace VocabTrainer.Api.Pagination
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; }
        public int PageNumber { get; }
        public int PageSize { get; }
        public int TotalCount { get; }

        public PaginatedList(List<T> items, int total, int page, int size)
        {
            Items = items;
            TotalCount = total;
            PageNumber = page;
            PageSize = size;
        }

        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> q, int page, int size)
        {
            var total = await q.CountAsync();
            var items = await q.Skip((page - 1) * size).Take(size).ToListAsync();
            return new PaginatedList<T>(items, total, page, size);
        }
    }
}