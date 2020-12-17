using LoggingAdvanced.Console;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Hosting.WindowsServices;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Logging.EventLog;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    public static class Program
    {
        public const string ServiceName = "ghetl";
        public static bool Stopping = false;

        static async Task Main(string[] args)
        {
            var isService = !(Debugger.IsAttached || args.Contains("--console"));

            var builder = new HostBuilder()
                .ConfigureAppConfiguration((hostingContext, configuration) =>
                {
                    configuration.Sources.Clear();

                    IHostEnvironment env = hostingContext.HostingEnvironment;

                    configuration
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true, true);
                })
                .ConfigureLogging((context, logging) =>
                {
                    logging
                        .ClearProviders();

                    if (isService)
                        logging
                            .AddEventLog(context.Configuration.GetSection("Logging:EventLog").Get<EventLogSettings>());
                    else
                        logging
                            .AddConsoleAdvanced(context.Configuration.GetSection("Logging:Console"));
                })
                .ConfigureServices((hostContext, services) =>
                    services
                        .AddHostedService<EtlService>()
                        .AddSingleton<EtlWorker>());

            var cancellationTokenSource = new CancellationTokenSource();

            if (isService)
                await builder.RunAsServiceAsync(cancellationTokenSource.Token);
            else
                await builder.RunConsoleAsync();
        }
    }
}
