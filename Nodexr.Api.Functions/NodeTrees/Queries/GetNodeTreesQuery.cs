namespace Nodexr.Api.Functions.NodeTrees.Queries;

using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.Models;
using System.Linq;

public interface IGetNodeTreesQuery
{
    IQueryable<NodeTreeModel> GetAllNodeTrees(string? titleSearch = null);
}

public class GetNodeTreesQuery : IGetNodeTreesQuery
{
    private readonly NodeTreeContext nodeTreeContext;

    public GetNodeTreesQuery(NodeTreeContext nodeTreeContext)
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
