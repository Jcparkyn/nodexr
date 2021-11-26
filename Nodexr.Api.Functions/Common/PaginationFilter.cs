namespace Nodexr.Api.Functions.Common;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Contracts.Pagination;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class PaginationFilter
{
    public int Start { get; }
    public int? Limit { get; }

    public PaginationFilter(int start, int? limit)
    {
        if (start < 0) throw new ArgumentOutOfRangeException(nameof(start));
        if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
        Start = start;
        Limit = limit;
    }

    public static PaginationFilter All()
    {
        return new PaginationFilter(0, null);
    }
}

public static class PaginationExtensions
{
    public static async Task<Paged<T>> ToPagedAsync<T>(
        this IQueryable<T> query,
        PaginationFilter pagination,
        CancellationToken cancellationToken = default)
    {
        var query2 = query;
        if (pagination.Start > 0)
        {
            query2 = query2.Skip(pagination.Start);
        }
        if (pagination.Limit is not null)
        {
            query2 = query2.Take(pagination.Limit.Value);
        }
        return new Paged<T>(
            await query2.ToListAsync(cancellationToken),
            await query.CountAsync(cancellationToken),
            pagination.Start,
            pagination.Limit);
    }
}
