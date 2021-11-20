namespace Nodexr.Api.Functions.IntegrationTests;

using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Nodexr.Api.Functions.Common;
using NUnit.Framework;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

[SetUpFixture]
public class Testing
{
    private static IServiceScopeFactory _scopeFactory = null!;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var startup = new TestStartup();
        var host = new HostBuilder()
            .ConfigureWebJobs(startup.Configure)
            .Build();

        var services = host.Services;

        _scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
    }

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request, CancellationToken cancellationToken = default)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request, cancellationToken);
    }

    public static Task ResetState()
    {
        return Task.CompletedTask; // TODO
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<INodexrContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<INodexrContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }

    private class TestStartup : Startup
    {
        protected override IConfigurationBuilder InitializeConfiguration(IConfigurationBuilder builder)
        {
            string basePath = Directory.GetCurrentDirectory();
            return builder
                .SetBasePath(basePath)
                .AddJsonFile("test.settings.json", optional: false, reloadOnChange: false)
                .AddUserSecrets<Testing>()
                .AddEnvironmentVariables();
        }
    }
}
