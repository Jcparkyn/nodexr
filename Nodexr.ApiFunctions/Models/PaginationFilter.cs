namespace Nodexr.ApiFunctions.Models;
using Microsoft.EntityFrameworkCore;
using Nodexr.ApiShared.Pagination;
using System.Linq;
using System.Threading.Tasks;

public class PaginationFilter
{
    public int Start { get; set; }
    public int Limit { get; set; }

    private readonly int maxLimit = 100;

    public PaginationFilter(int start, int limit)
    {
        Start = start < 0 ? 0 : start;
        Limit = limit > maxLimit ? maxLimit : limit;
    }

    public async Task<Paged<T>> ApplyTo<T>(IQueryable<T> collection)
    {
        return new Paged<T>(
            await collection.Skip(Start).Take(Limit).ToListAsync(),
            collection.Count(),
            Start,
            Limit);
    }
}
