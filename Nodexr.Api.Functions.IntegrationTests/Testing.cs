namespace Nodexr.Api.Functions.IntegrationTests;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

[SetUpFixture]
public class Testing
{
    private static IServiceScopeFactory _scopeFactory = null!;

    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        var startup = new Startup();
        var host = new HostBuilder()
            .ConfigureWebJobs(startup.Configure)
            .Build();

        var services = host.Services;

        _scopeFactory = services.GetRequiredService<IServiceScopeFactory>();
    }

    [OneTimeTearDown]
    public void RunAfterAnyTests()
    {
    }
}
