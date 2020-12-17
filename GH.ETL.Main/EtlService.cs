using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class EtlService : IHostedService, IDisposable
    {
        private readonly IConfiguration _configuration;
        private readonly EtlWorker _etlWorker;
        private readonly ILogger _logger;
        private Timer _timer;

        public EtlService(
            IConfiguration configuration,
            EtlWorker etlWorker,
            ILogger<EtlService> logger)
        {
            _configuration = configuration;
            _etlWorker = etlWorker;
            _logger = logger;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting ETL Service...");

            _timer = new Timer(
                e => Task.Run(_etlWorker.Task),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMilliseconds(_configuration.GetSection("Parameter").GetValue<double>("Milliseconds Interval")));

            _logger.LogInformation("Done.");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping ETL Service...");

            _timer?.Change(Timeout.Infinite, 0);

            _logger.LogInformation("Done.");

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
