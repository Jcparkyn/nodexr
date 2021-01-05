using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Nodexr.ApiFunctions.Models;
using Nodexr.ApiFunctions.Services;
using System;
using System.Collections.Generic;
using System.Text;


[assembly: FunctionsStartup(typeof(Nodexr.ApiFunctions.Startup))]

namespace Nodexr.ApiFunctions
{
    class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            string? key = Environment.GetEnvironmentVariable("databaseKey");
            string? endpoint = Environment.GetEnvironmentVariable("databaseEndpoint");
            string? databaseName = Environment.GetEnvironmentVariable("databaseName");

            builder.Services.AddDbContext<NodeTreeContext>(
                options => options.UseCosmos(
                    endpoint,
                    key,
                    databaseName: databaseName));

            builder.Services.AddTransient<IGetNodeTreesService, GetNodeTreesService>();
        }
    }
}
