using Microsoft.AspNetCore.Components.Builder;
using Microsoft.Extensions.DependencyInjection;
using RegexNodes.Shared;

namespace RegexNodes
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton<INodeDragService, NodeDragService>();
            services.AddSingleton<INodeHandler, NodeHandler>();
            services.AddSingleton<RegexReplaceHandler>();
            //services.AddSingleton<IZoomHandler, ZoomHandler>();
        }

        public void Configure(IComponentsApplicationBuilder app)
        {
            app.AddComponent<App>("app");
        }
    }
}
