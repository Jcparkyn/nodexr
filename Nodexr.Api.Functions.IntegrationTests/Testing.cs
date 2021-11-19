namespace Nodexr.Api.Functions.IntegrationTests;
using MediatR;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Moq;
using Nodexr.Api.Functions.Common;
using NUnit.Framework;
using Respawn;
using System;
using System.IO;
using System.Threading.Tasks;

[SetUpFixture]
public class Testing
{
    private static IConfigurationRoot _configuration = null!;
    private static IServiceScopeFactory _scopeFactory = null!;
    private static Checkpoint _checkpoint = null!;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var startup = new Startup();
        var host = new HostBuilder()
            .ConfigureWebJobs(startup.Configure)
            .Build();

        var services = host.Services;

        _scopeFactory = services.GetRequiredService<IServiceScopeFactory>();

        //_checkpoint = new Checkpoint
        //{
        //    TablesToIgnore = new[] { "__EFMigrationsHistory" }
        //};

        //EnsureDatabase();
    }

    //private static void EnsureDatabase()
    //{
    //    using var scope = _scopeFactory.CreateScope();

    //    var context = scope.ServiceProvider.GetRequiredService<NodexrContext>();

    //    context.Database.Migrate();
    //}

    public static async Task<TResponse> SendAsync<TResponse>(IRequest<TResponse> request)
    {
        using var scope = _scopeFactory.CreateScope();

        var mediator = scope.ServiceProvider.GetRequiredService<ISender>();

        return await mediator.Send(request);
    }

    public static async Task ResetState()
    {
        //await _checkpoint.Reset(_configuration.GetConnectionString("DefaultConnection"));

        //_currentUserId = null;
    }

    public static async Task<TEntity?> FindAsync<TEntity>(params object[] keyValues)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<NodexrContext>();

        return await context.FindAsync<TEntity>(keyValues);
    }

    public static async Task AddAsync<TEntity>(TEntity entity)
        where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<NodexrContext>();

        context.Add(entity);

        await context.SaveChangesAsync();
    }

    public static async Task<int> CountAsync<TEntity>() where TEntity : class
    {
        using var scope = _scopeFactory.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<NodexrContext>();

        return await context.Set<TEntity>().CountAsync();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
