using System;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Nodexr.Shared;
using Nodexr.Shared.Services;
using Blazored;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace Nodexr
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");

            builder.Services.AddTransient(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
            builder.Services.AddSingleton<INodeDragService, NodeDragService>();
            builder.Services.AddSingleton<INoodleDragService, NoodleDragService>();
            builder.Services.AddSingleton<INodeHandler, NodeHandler>();
            builder.Services.AddSingleton<RegexReplaceHandler>();
            builder.Services.AddBlazoredModal();

            await builder.Build().RunAsync();
        }
    }
}
