namespace Nodexr;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.FeatureManagement;
using Nodexr.Services;
using Blazored.Modal;
using Blazored.Toast;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");

        builder.Services.AddBlazoredToast();
        builder.Services.AddTransient(_ => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
        builder.Services.AddScoped<INodeDragService, NodeDragService>();
        builder.Services.AddScoped<INoodleDragService, NoodleDragService>();
        builder.Services.AddScoped<INodeHandler, NodeHandler>();
        builder.Services.AddScoped<RegexReplaceHandler>();
        builder.Services.AddScoped<NodeTreeBrowserService>();
        builder.Services.AddBlazoredModal();
        builder.Services.AddFeatureManagement();

        await builder.Build().RunAsync().ConfigureAwait(false);
    }
}
