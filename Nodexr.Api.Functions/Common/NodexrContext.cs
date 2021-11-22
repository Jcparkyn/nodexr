namespace Nodexr.Api.Functions.Common;

using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using Nodexr.Api.Functions.Models;
using System;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;

public interface INodexrContext : IDisposable, IAsyncDisposable
{
    DbSet<NodeTree> NodeTrees { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    ValueTask<TEntity?> FindAsync<TEntity>(object?[]? keyValues, CancellationToken cancellationToken = default)
        where TEntity : class;

    EntityEntry<TEntity> Add<TEntity>(TEntity entity) where TEntity : class;
}

public class NodexrContext : DbContext, INodexrContext
{
    private readonly NodexrDbConfiguration configuration;

    public NodexrContext(DbContextOptions<NodexrContext> options, IOptions<NodexrDbConfiguration> configuration)
        : base(options)
    {
        this.configuration = configuration.Value;
    }

    public DbSet<NodeTree> NodeTrees => Set<NodeTree>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<NodeTree>()
            .HasNoDiscriminator()
            .ToContainer(nameof(NodeTrees))
            .HasPartitionKey(x => x.Id)
            .HasKey(x => x.Id);

        modelBuilder.Entity<NodeTree>()
            .Property(x => x.Id);
    }

    /// <summary>
    /// Returns a proxy reference to the Cosmos container for <see cref="NodeTrees"/>.
    /// </summary>
    /// <remarks>
    /// Proxy reference doesn't guarantee existence.
    /// </remarks>
    public Container NodeTreeContainer => Database
        .GetCosmosClient()
        .GetContainer(configuration.DatabaseName, nameof(NodeTrees));
}

public class NodexrDbConfiguration
{
    [Required]
    public string ConnectionString { get; set; } = null!;
    [Required]
    public string DatabaseName { get; set; } = null!;
}
