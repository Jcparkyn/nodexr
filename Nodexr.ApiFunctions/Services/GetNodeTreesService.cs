using Nodexr.ApiFunctions.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace Nodexr.ApiFunctions.Services
{
    public interface IGetNodeTreesService
    {
        IQueryable<NodeTreeModel> GetAllNodeTrees(string? titleSearch = null, Expression<Func<NodeTreeModel, bool>>? filter = null);
        Task<NodeTreeModel> GetNodeTreeById(string id);
    }

    public class GetNodeTreesService : IGetNodeTreesService
    {
        private readonly NodeTreeContext nodeTreeContext;

        public GetNodeTreesService(NodeTreeContext nodeTreeContext)
        {
            this.nodeTreeContext = nodeTreeContext;
        }

        public async Task<NodeTreeModel> GetNodeTreeById(string id)
        {
            return await nodeTreeContext.NodeTrees.FindAsync(id);
        }

        public IQueryable<NodeTreeModel> GetAllNodeTrees(
            string? titleSearch = null,
            Expression<Func<NodeTreeModel, bool>>? filter = null
            )
        {
            var query = nodeTreeContext.NodeTrees.AsQueryable();

            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (titleSearch != null)
            {
                query = query.Where(tree => tree.Title == titleSearch);
            }

            return query;
        }
    }
}
