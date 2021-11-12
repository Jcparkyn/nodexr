namespace Nodexr.ApiFunctions.Models;
using Microsoft.EntityFrameworkCore;

public class NodeTreeContext : DbContext
{
    public NodeTreeContext(DbContextOptions<NodeTreeContext> options)
        : base(options)
    { }

    public DbSet<NodeTreeModel> NodeTrees => Set<NodeTreeModel>();
}
