namespace Nodexr.Api.Functions.NodeTrees.Commands;

using MediatR;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.Models;

public record CreateNodeTreeCommandHandler(
    INodexrContext dbContext
) : IRequestHandler<CreateNodeTreeCommand, string>
{
    public async Task<string> Handle(CreateNodeTreeCommand request, CancellationToken cancellationToken)
    {
        var newTree = new NodeTree(request.Title, request.Expression)
        {
            Description = request.Description,
        };

        await dbContext.NodeTrees.AddAsync(newTree, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newTree.Id;
    }
}
