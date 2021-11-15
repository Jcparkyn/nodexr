namespace Nodexr.Api.Functions.Common;
using Microsoft.EntityFrameworkCore;
using Nodexr.Api.Functions.Models;
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

    public DbSet<NodeTree> NodeTrees => Set<NodeTree>();
}
