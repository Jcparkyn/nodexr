using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nodexr.ApiFunctions.Models;
using Nodexr.ApiFunctions.Services;
using System;


[assembly: FunctionsStartup(typeof(Nodexr.ApiFunctions.Startup))]

namespace Nodexr.ApiFunctions
{
    internal class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string key = Environment.GetEnvironmentVariable("databaseKey")
                ?? throw new InvalidOperationException("databaseKey must be provided in application settings");
            string endpoint = Environment.GetEnvironmentVariable("databaseEndpoint")
                ?? throw new InvalidOperationException("databaseEndpoint must be provided in application settings");
            string databaseName = Environment.GetEnvironmentVariable("databaseName")
                ?? throw new InvalidOperationException("databaseName must be provided in application settings");

            builder.Services.AddDbContext<NodeTreeContext>(
                options => options.UseCosmos(
                    endpoint,
                    key,
                    databaseName: databaseName));

            builder.Services.AddTransient<IGetNodeTreesService, GetNodeTreesService>();
        }
    }
}
