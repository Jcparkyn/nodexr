namespace Nodexr.Api.Functions.NodeTrees;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.NodeTrees.Queries;
using MediatR;
using Nodexr.Api.Functions.Models;
using System.Text.Json;

public record NodeTreeApi(
    INodexrContext dbContext,
    ISender mediator
)
{
    [FunctionName("CreateNodeTree")]
    public async Task<IActionResult> CreateNodeTree(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nodetree")] HttpRequest req,
        ILogger log,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Creating new NodeTree");

        var command = JsonSerializer.Deserialize<CreateNodeTreeCommand>(req.Body);
        if (command is null) return new BadRequestResult();

        string id = await mediator.Send(command, cancellationToken);

        return new OkObjectResult(id);
    }

    [FunctionName("CreateAnonymousNodeTree")]
    public async Task<IActionResult> CreateAnonymousNodeTree(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nodetree/anon")] HttpRequest req,
        ILogger log,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Creating new NodeTree");

        try
        {
            var command = JsonSerializer.Deserialize<CreateAnonymousNodeTreeCommand>(req.Body);

            if (command is null) return new BadRequestResult();

            var newTree = new NodeTree()
            {
                Searchable = false,
                Nodes = command.Nodes,
            };

            dbContext.NodeTrees.Add(newTree);
            await dbContext.SaveChangesAsync(cancellationToken);

            return new OkObjectResult(newTree.Id);
        }
        catch (JsonException)
        {
            return new BadRequestResult();
        }
    }

    [FunctionName("GetNodeTreeById")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "req is required by Azure Functions")]
    public async Task<ActionResult<NodeTreePreviewDto>> GetNodeTreeById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nodetree/{id}")] HttpRequest req,
        ILogger log,
        string id)
    {
        log.LogInformation("Retrieving NodeTree with Id " + id);

        var tree = await dbContext.NodeTrees.FindAsync(id);

        if (tree is null)
            return new NotFoundResult();

        return CustomResult.Json(new NodeTreePreviewDto
        {
            Id = tree.Id,
            Description = tree.Description,
            Expression = tree.Expression,
            Nodes = tree.Nodes,
            Title = tree.Title,
        });
    }

    [FunctionName("GetAllNodeTrees")]
    public async Task<IActionResult> GetAllNodeTrees(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nodetree")] HttpRequest req,
        ILogger log,
        CancellationToken cancellationToken)
    {
        string titleSearch = req.Query["search"];
        int start = GetQueryInt(req, "start") ?? 0;
        int limit = GetQueryInt(req, "limit") ?? 50;

        log.LogInformation($"Retrieving all NodeTrees, with start = {start}, limit = {limit}, search = {titleSearch}");

        var query = new GetNodeTreesQuery
        {
            SearchString = req.Query["search"],
            Pagination = new(start, limit),
        };

        var trees = await mediator.Send(query, cancellationToken);

        return CustomResult.Json(trees);
    }

    private static int? GetQueryInt(HttpRequest req, string queryName)
    {
        string countParam = req.Query[queryName];
        return int.TryParse(countParam, out int count) ? count : null;
    }
}
