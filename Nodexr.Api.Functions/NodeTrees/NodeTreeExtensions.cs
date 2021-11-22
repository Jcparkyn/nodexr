namespace Nodexr.Api.Functions.NodeTrees;
using Nodexr.Api.Functions.Models;
using System.Linq;

public static class NodeTreeExtensions
{
    public static IQueryable<NodeTree> WithSearchString(this IQueryable<NodeTree> query, string? searchString)
    {
        if (string.IsNullOrEmpty(searchString))
        {
            return query;
        }

        return query.Where(tree =>
            // Case-insensitive search (StringComparison doesn't work with Cosmos)
            tree.Title.ToLower().Contains(searchString.ToLowerInvariant())
        );
    }
}
