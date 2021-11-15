namespace Nodexr.Api.Functions.NodeTrees.Commands;

using MediatR;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.Models;
using System.Threading;
using System.Threading.Tasks;

public class CreateNodeTreeCommandHandler : IRequestHandler<CreateNodeTreeCommand, string>
{
    private readonly NodexrContext dbContext;

    public CreateNodeTreeCommandHandler(NodexrContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public async Task<string> Handle(CreateNodeTreeCommand request, CancellationToken cancellationToken)
    {
        var newTree = new NodeTree(request.Title)
        {
            Description = request.Description,
            Expression = request.Expression,
        };

        await dbContext.NodeTrees.AddAsync(newTree, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return newTree.id;
    }
}
