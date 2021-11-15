using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nodexr.Api.Functions.Common;
using Nodexr.Api.Functions.NodeTrees.Queries;
using System;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.Configuration;

[assembly: FunctionsStartup(typeof(Nodexr.Api.Functions.Startup))]

namespace Nodexr.Api.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        string key = Environment.GetEnvironmentVariable("databaseKey")
            ?? throw new InvalidOperationException("databaseKey must be provided in application settings");
        string endpoint = Environment.GetEnvironmentVariable("databaseEndpoint")
            ?? throw new InvalidOperationException("databaseEndpoint must be provided in application settings");
        string databaseName = Environment.GetEnvironmentVariable("databaseName")
            ?? throw new InvalidOperationException("databaseName must be provided in application settings");

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        builder.Services.AddDbContext<NodexrContext>(
            options => options.UseCosmos(
                endpoint,
                key,
                databaseName: databaseName));

        builder.Services.AddScoped<INodexrContext, NodexrContext>();

        builder.Services.AddTransient<IGetNodeTreesQuery, GetNodeTreesQuery>();
    }
}
