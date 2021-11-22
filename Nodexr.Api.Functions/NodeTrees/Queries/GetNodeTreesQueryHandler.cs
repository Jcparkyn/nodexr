namespace Nodexr.Api.Functions.NodeTrees.Queries;

using MediatR;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Contracts.Pagination;
using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.NodeTrees;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public record GetNodeTreesQueryHandler(
    INodexrContext nodeTreeContext
) : IRequestHandler<GetNodeTreesQuery, Paged<NodeTreePreviewDto>>
{
    public async Task<Paged<NodeTreePreviewDto>> Handle(GetNodeTreesQuery request, CancellationToken cancellationToken)
    {
        return await nodeTreeContext.NodeTrees
            .AsNoTracking()
            .WithSearchString(request.SearchString)
            .OrderBy(tree => tree.Title)
            .Select(tree => new NodeTreePreviewDto
            {
                Id = tree.Id,
                Description = tree.Description,
                Expression = tree.Expression,
                Title = tree.Title,
            })
            .ToPagedAsync(
                new(request.Start ?? 0, request.Limit ?? 10),
                cancellationToken
            );
    }
}
