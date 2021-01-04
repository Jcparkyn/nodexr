using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace Nodexr.ApiFunctions.Models
{
    public class NodeTreeContext : DbContext
    {
        public NodeTreeContext(DbContextOptions<NodeTreeContext> options)
            : base(options)
        { }

        public DbSet<NodeTreeModel> NodeTrees => Set<NodeTreeModel>();
    }

    /*public class NodeTreeContextFactory : IDesignTimeDbContextFactory<NodeTreeContext>
    {
        public NodeTreeContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<NodeTreeContext>();

            string endpoint = "https://nodexr-db.documents.azure.com:443";
            string key = "Q96I1YOpXtftLIjgtoN6ICtg5PULJYL6GXrOFbpoQUqisKC04SnYr9on3R7zw7FyR7l67WJyGOCzdppOzRDdiw==;";
            optionsBuilder.UseCosmos(
                    endpoint,
                    key,
                    databaseName: "NodexrDb");

            return new NodeTreeContext(optionsBuilder.Options);
        }
    }*/

}
