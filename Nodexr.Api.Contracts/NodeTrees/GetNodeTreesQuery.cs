namespace Nodexr.Api.Contracts.NodeTrees;
using MediatR;
using Nodexr.Api.Contracts.Pagination;

public record GetNodeTreesQuery : IRequest<Paged<NodeTreePreviewDto>>
{
    public string? SearchString { get; init; }
    public int? Start { get; init; }
    public int? Limit { get; set; }
}
