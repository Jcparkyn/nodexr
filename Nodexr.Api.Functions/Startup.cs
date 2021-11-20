using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nodexr.Api.Functions.Common;
using System;
using MediatR;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using System.IO;
using Microsoft.Extensions.Options;

[assembly: FunctionsStartup(typeof(Nodexr.Api.Functions.Startup))]

namespace Nodexr.Api.Functions;

public class Startup : FunctionsStartup
{
    public override void Configure(IFunctionsHostBuilder builder)
    {
        var config = InitializeConfiguration();

        builder.Services.AddSingleton(config);

        builder.Services.AddOptions<NodexrDbConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Database").Bind(settings);
            })
            .ValidateDataAnnotations();

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        builder.Services.AddDbContext<NodexrContext>((services, options) =>
        {
            var config = services.GetRequiredService<IOptions<NodexrDbConfiguration>>().Value;
            options.UseCosmos(
                config.ConnectionString ?? throw new InvalidOperationException("ConnectionString must be provided in application settings"),
                config.DatabaseName ?? throw new InvalidOperationException("ConnectionString must be provided in application settings"));
        });

        builder.Services.AddScoped<INodexrContext, NodexrContext>();
    }

    private static IConfiguration InitializeConfiguration()
    {
        string basePath = Directory.GetCurrentDirectory();
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
            .AddUserSecrets<Startup>()
            .AddEnvironmentVariables()
            .Build();
    }
}
