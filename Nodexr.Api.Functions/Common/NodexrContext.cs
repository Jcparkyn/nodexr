namespace Nodexr.Api.Functions.Common;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Functions.Models;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

public interface INodexrContext
{
    DbSet<NodeTree> NodeTrees { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
}

public class NodexrContext : DbContext, INodexrContext
{
    public NodexrContext(DbContextOptions<NodexrContext> options)
        : base(options)
    { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.HasDefaultContainer("NodeTreeContext");
        modelBuilder.Entity<NodeTree>();
    }

    public DbSet<NodeTree> NodeTrees => Set<NodeTree>();
}

public class NodexrDbConfiguration
{
    [Required]
    public string ConnectionString { get; set; } = null!;
    [Required]
    public string DatabaseName { get; set; } = null!;
}
