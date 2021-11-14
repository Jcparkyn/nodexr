namespace Nodexr.Api.Functions.NodeTrees;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nodexr.Api.Functions.Models;
using System.Linq;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.NodeTrees.Queries;
using Nodexr.Api.Functions.Common;

public class NodeTreeApi
{
    private readonly NodeTreeContext dbContext;
    private readonly IGetNodeTreesQuery getNodeTreeService;

    public NodeTreeApi(NodeTreeContext dbContext, IGetNodeTreesQuery getNodeTreeService)
    {
        this.dbContext = dbContext;
        this.getNodeTreeService = getNodeTreeService;
    }

    [FunctionName("CreateNodeTree")]
    public async Task<IActionResult> CreateNodeTree(
        [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nodetree")] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("Creating new NodeTree");

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        var publishModel = JsonConvert.DeserializeObject<NodeTreePublishDto>(requestBody);
        var newTree = new NodeTreeModel(publishModel.Title)
        {
            Description = publishModel.Description,
            Expression = publishModel.Expression,
        };

        //Add the new tree to the database
        dbContext.NodeTrees.Add(newTree);
        await dbContext.SaveChangesAsync();

        return new OkObjectResult(newTree);
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
        ILogger log)
    {
        string titleSearch = req.Query["search"];
        int? limit = GetQueryInt(req, "limit");
        int start = GetQueryInt(req, "start") ?? 0;

        log.LogInformation($"Retrieving all NodeTrees, with start = {start}, limit = {limit}, search = {titleSearch}");

        var paginationFilter = new PaginationFilter(start, limit ?? 50);

        var query = getNodeTreeService.GetAllNodeTrees(
            titleSearch: titleSearch
            )
            .OrderBy(tree => tree.Title)
            .Select(tree => new NodeTreePreviewDto
            {
                Description = tree.Description,
                Expression = tree.Expression,
                Title = tree.Title,
            });

        var trees = await paginationFilter.ApplyTo(query);

        return new OkObjectResult(trees);
    }

    private static int? GetQueryInt(HttpRequest req, string queryName)
    {
        string countParam = req.Query[queryName];
        return int.TryParse(countParam, out int count) ? count : null;
    }
}
