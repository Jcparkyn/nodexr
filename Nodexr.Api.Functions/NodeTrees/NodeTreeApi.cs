namespace Nodexr.Api.Functions.NodeTrees;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.Common;
using System.Threading;
using MediatR;

public record NodeTreeApi(
    INodexrContext dbContext,
    ISender mediator
){

    [FunctionName("CreateNodeTree")]
    public async Task<IActionResult> CreateNodeTree(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nodetree")] HttpRequest req,
        ILogger log,
        CancellationToken cancellationToken)
    {
        log.LogInformation("Creating new NodeTree");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var command = JsonConvert.DeserializeObject<CreateNodeTreeCommand>(requestBody);

        string id = await mediator.Send(command, cancellationToken);

        return new OkObjectResult(id);
    }

    [FunctionName("GetNodeTreeById")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "req is required by Azure Functions")]
    public async Task<IActionResult> GetNodeTreeById(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nodetree/{id}")] HttpRequest req,
        ILogger log,
        string id)
    {
        log.LogInformation("Retrieving NodeTree with Id " + id);

        var tree = await dbContext.NodeTrees.FindAsync(id);

        if (tree is null)
            return new NotFoundResult();

        return new OkObjectResult(tree);
    }

    [FunctionName("GetAllNodeTrees")]
    public async Task<IActionResult> GetAllNodeTrees(
        [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nodetree")] HttpRequest req,
        ILogger log,
        CancellationToken cancellationToken)
    {
        string titleSearch = req.Query["search"];
        int start = GetQueryInt(req, "start") ?? 0;
        int? limit = GetQueryInt(req, "limit");

        log.LogInformation($"Retrieving all NodeTrees, with start = {start}, limit = {limit}, search = {titleSearch}");

        var query = new GetNodeTreesQuery
        {
            SearchString = req.Query["search"],
            Start = start,
            Limit = limit,
        };

        var trees = await mediator.Send(query, cancellationToken);

        return new OkObjectResult(trees);
    }

    private static int? GetQueryInt(HttpRequest req, string queryName)
    {
        string countParam = req.Query[queryName];
        return int.TryParse(countParam, out int count) ? count : null;
    }
}
