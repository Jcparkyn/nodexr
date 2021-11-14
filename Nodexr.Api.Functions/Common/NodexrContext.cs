namespace Nodexr.Api.Functions.Common;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Functions.Models;

public class NodexrContext : DbContext
{
    public NodexrContext(DbContextOptions<NodexrContext> options)
        : base(options)
    { }

    public DbSet<NodeTreeModel> NodeTrees => Set<NodeTreeModel>();
}
