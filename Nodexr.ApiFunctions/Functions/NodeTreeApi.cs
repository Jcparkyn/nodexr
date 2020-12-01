using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Nodexr.ApiFunctions.Models;
using Nodexr.ApiFunctions.Services;
using System.Collections.Generic;
using System.Linq;
using Nodexr.ApiShared;
using Nodexr.ApiShared.Pagination;

namespace Nodexr.ApiFunctions.Functions
{
    public class NodeTreeApi
    {
        private readonly NodeTreeRepository nodeTreeRepository;

        public NodeTreeApi(NodeTreeRepository nodeTreeRepository)
        {
            this.nodeTreeRepository = nodeTreeRepository;
        }

        [FunctionName("CreateNodeTree")]
        public async Task<IActionResult> CreateNodeTree(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "nodetree")] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Creating new NodeTree");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var newTree = JsonConvert.DeserializeObject<NodeTreeModel>(requestBody);

            await nodeTreeRepository.InsertNodeTree(newTree);

            return new OkObjectResult(newTree);
        }

        [FunctionName("GetNodeTreeById")]
        public async Task<IActionResult> GetNodeTreeById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "nodetree/{id}")] HttpRequest req,
            ILogger log,
            string id)
        {
            log.LogInformation("Retrieving NodeTree with Id " + id);

            var tree = await nodeTreeRepository.GetNodeTreeById(id);

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

            log.LogInformation($"Retrieving all NodeTrees, with start = {start}, limit = {limit}");

            var paginationFilter = new PaginationFilter(start, limit ?? 50);

            var trees = string.IsNullOrEmpty(titleSearch) ?
                await nodeTreeRepository.GetAllNodeTrees(paginationFilter) :
                await nodeTreeRepository.GetNodeTreesWithTitleLike(titleSearch, paginationFilter);

            //var response = new HttpResponse<Paged<NodeTreeModel>>(trees);
            
            return new OkObjectResult(trees);
        }

        private int? GetQueryInt(HttpRequest req, string queryName)
        {
            string countParam = req.Query[queryName];
            return int.TryParse(countParam, out int count) ?
                count :
                null as int?;
        }
    }
}
