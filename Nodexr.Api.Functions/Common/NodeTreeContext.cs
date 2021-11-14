namespace Nodexr.Api.Functions.Common;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Functions.Models;

public class NodeTreeContext : DbContext
{
    public NodeTreeContext(DbContextOptions<NodeTreeContext> options)
        : base(options)
    { }

    public DbSet<NodeTreeModel> NodeTrees => Set<NodeTreeModel>();
}
