using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using RegexNodes.Shared;
using Blazored;
using Blazored.Modal;
using Blazored.Modal.Services;

namespace RegexNodes
{
    public class Startup
    {

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INodeDragService, NodeDragService>();
            services.AddSingleton<INodeHandler, NodeHandler>();
            services.AddSingleton<RegexReplaceHandler>();
            services.AddBlazoredModal();
            //services.AddSingleton<IZoomHandler, ZoomHandler>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
