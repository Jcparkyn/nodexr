namespace Nodexr.Api.Functions.Common;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Contracts.Pagination;
using System.Linq;
using System.Threading;
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

    public async Task<Paged<T>> ApplyTo<T>(IQueryable<T> collection, CancellationToken cancellationToken)
    {
        return new Paged<T>(
            await collection.Skip(Start).Take(Limit).ToListAsync(cancellationToken),
            await collection.CountAsync(cancellationToken), // TODO: Parallelize
            Start,
            Limit);
    }
}

public static class PaginationExtensions
{
    public static async Task<Paged<T>> WithPagination<T>(
        this IQueryable<T> query,
        PaginationFilter pagination,
        CancellationToken cancellationToken = default)
    {
        return await pagination.ApplyTo(query, cancellationToken);
    }
}
