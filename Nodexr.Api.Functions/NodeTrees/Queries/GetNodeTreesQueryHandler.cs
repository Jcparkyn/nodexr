namespace Nodexr.Api.Functions.NodeTrees.Queries;

using MediatR;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Contracts.Pagination;
using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.Models;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public interface IGetNodeTreesQuery
{
    IQueryable<NodeTree> GetAllNodeTrees(string? titleSearch = null);
}

public record GetNodeTreesQueryHandler(
    INodexrContext nodeTreeContext
) : IRequestHandler<GetNodeTreesQuery, Paged<NodeTreePreviewDto>>
{
    public async Task<Paged<NodeTreePreviewDto>> Handle(GetNodeTreesQuery request, CancellationToken cancellationToken)
    {
        return await nodeTreeContext.NodeTrees
            .WithSearchString(request.SearchString)
            .OrderBy(tree => tree.Title)
            .Select(tree => new NodeTreePreviewDto
            {
                Description = tree.Description,
                Expression = tree.Expression,
                Title = tree.Title,
            })
            .WithPagination(
                new(request.Start ?? 0, request.Limit ?? 10),
                cancellationToken
            );
    }
}

public static class NodeTreeExtensions
{
    public static IQueryable<NodeTree> WithSearchString(this IQueryable<NodeTree> query, string? searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return query;
        }

        return query.Where(tree =>
            // Case-insensitive search (StringComparison doesn't work with Cosmos)
            tree.Title.ToLower().Contains(searchString.ToLowerInvariant())
        );
    }
}
