using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace GH.ETL.Main
{
    class ServiceLifetime : ServiceBase, IHostLifetime
    {
        private readonly TaskCompletionSource<object> _delayStart = new TaskCompletionSource<object>();

        public ServiceLifetime(IHostApplicationLifetime hostApplicationLifetime)
        {
            HostApplicationLifetime = hostApplicationLifetime;
        }

        public IHostApplicationLifetime HostApplicationLifetime { get; }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            Stop();
            return Task.CompletedTask;
        }

        public Task WaitForStartAsync(CancellationToken cancellationToken)
        {
            cancellationToken.Register(() => _delayStart.TrySetCanceled());
            HostApplicationLifetime.ApplicationStopping.Register(Stop);


            new Thread(Run).Start();
            return _delayStart.Task;
        }

        private void Run()
        {
            try
            {
                Run(this);
                _delayStart.TrySetException(new InvalidOperationException("Stopping withou starting."));
            }
            catch (Exception ex)
            {
                _delayStart.TrySetException(ex);
            }
        }

        protected override void OnStart(string[] args)
        {
            _delayStart.TrySetResult(null);
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            HostApplicationLifetime.StopApplication();
            base.OnStop();
        }
    }
}
