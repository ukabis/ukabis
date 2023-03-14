using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace JP.DataHub.MVC.HostService
{
    public class GCHostedService : IHostedService, IDisposable
    {
        private readonly ILogger<GCHostedService> _logger;
        private Timer _timer;
        private readonly int _intervalSeconds = 60;
        public GCHostedService(ILogger<GCHostedService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _intervalSeconds = configuration.GetValue<int?>("GCIntervalSeconds") ?? 60;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GC Hosted Service running.");
            if (_intervalSeconds > 0)
            {
                _timer = new Timer(DoWork, null, TimeSpan.Zero, TimeSpan.FromSeconds(_intervalSeconds));
            }
            return Task.CompletedTask;
        }

        private void DoWork(object state)
        {
            _logger.LogInformation("Excute GC.");
            GC.Collect();
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("GC Hosted Service is stopping.");
            _timer?.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}