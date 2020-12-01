using Microsoft.EntityFrameworkCore;
using Nodexr.ApiShared.Pagination;
using Nodexr.ApiFunctions.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nodexr.ApiFunctions.Services
{
    public class NodeTreeRepository
    {
        private readonly NodeTreeContext nodeTreeContext;

        public NodeTreeRepository(NodeTreeContext nodeTreeContext)
        {
            this.nodeTreeContext = nodeTreeContext;
        }

        public async Task InsertNodeTree(NodeTreeModel tree)
        {
            nodeTreeContext.NodeTrees.Add(tree);
            await nodeTreeContext.SaveChangesAsync();
        }

        public async Task<NodeTreeModel> GetNodeTreeById(string id)
        {
            return await nodeTreeContext.NodeTrees.FindAsync(id);
        }

        public async Task<Paged<NodeTreeModel>> GetAllNodeTrees(PaginationFilter pagination)
        {
            var nodeTrees = nodeTreeContext.NodeTrees.AsQueryable();

            return await pagination.ApplyTo(nodeTrees);
        }

        public async Task<Paged<NodeTreeModel>> GetNodeTreesWithTitleLike(string search, PaginationFilter pagination)
        {
            //Hack: Filtering on client-side because Where is not supported on older versions 
            //of EF Core with Cosmos, and DI is not supported on 5.0.
            var trees = nodeTreeContext.NodeTrees.AsNoTracking().Where(tree => tree.Title == search);
            return await pagination.ApplyTo(trees);
        }
    }
}
