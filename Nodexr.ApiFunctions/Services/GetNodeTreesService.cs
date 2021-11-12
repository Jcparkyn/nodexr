using Nodexr.ApiFunctions.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Nodexr.ApiFunctions.Services
{
    public interface IGetNodeTreesService
    {
        IQueryable<NodeTreeModel> GetAllNodeTrees(string? titleSearch = null);
    }

    public class GetNodeTreesService : IGetNodeTreesService
    {
        private readonly NodeTreeContext nodeTreeContext;

        public GetNodeTreesService(NodeTreeContext nodeTreeContext)
        {
            this.nodeTreeContext = nodeTreeContext;
        }

        public IQueryable<NodeTreeModel> GetAllNodeTrees(string? titleSearch = null)
        {
            var query = nodeTreeContext.NodeTrees.AsQueryable();

            if (!string.IsNullOrEmpty(titleSearch))
            {
                query = query.Where(tree =>
                    // Case-insensitive search (StringComparison doesn't work with Cosmos)
                    tree.Title.ToLower().Contains(titleSearch.ToLowerInvariant())
                );
            }

            return query;
        }
    }
}
