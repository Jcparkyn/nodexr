using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nodexr.Api.Functions.Common;
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
        var config = InitializeConfiguration(new ConfigurationBuilder()).Build();

        builder.Services.AddSingleton<IConfiguration>(config);

        builder.Services.AddOptions<NodexrDbConfiguration>()
            .Configure<IConfiguration>((settings, configuration) =>
            {
                configuration.GetSection("Database").Bind(settings);
            })
            .ValidateDataAnnotations();

        builder.Services.AddMediatR(Assembly.GetExecutingAssembly());

        builder.Services.AddDbContext<INodexrContext, NodexrContext>((services, options) =>
        {
            var config = services.GetRequiredService<IOptions<NodexrDbConfiguration>>().Value;
            options.UseCosmos(config.ConnectionString, config.DatabaseName);
        });
    }

    protected virtual IConfigurationBuilder InitializeConfiguration(IConfigurationBuilder builder)
    {
        string basePath = Directory.GetCurrentDirectory();
        return builder
            .SetBasePath(basePath)
            .AddJsonFile("local.settings.json", optional: true, reloadOnChange: false)
            .AddUserSecrets<Startup>()
            .AddEnvironmentVariables();
    }
}
