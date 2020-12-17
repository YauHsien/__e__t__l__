using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    static class ServiceLifetimeExtensions
    {
        public static IHostBuilder UseServiceBaseLifeTime(this IHostBuilder hostBuilder)
        {
            return hostBuilder.ConfigureServices((hostContext, services) => services.AddSingleton<IHostLifetime, ServiceLifetime>());
        }

        public static Task RunAsServiceAsync(this IHostBuilder hostBuilder, CancellationToken cancellationToken)
        {
            return hostBuilder.UseServiceBaseLifeTime().Build().RunAsync(cancellationToken);
        }
    }
}
