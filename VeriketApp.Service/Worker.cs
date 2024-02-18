using Microsoft.Extensions.Options;
using VeriketApp.Core;
using VeriketApp.Service.Services;

namespace VeriketApp.Service
{
    public class Worker(IOptions<AppSettings> settings, ICustomLogService service) : BackgroundService
    {
        private readonly AppSettings _settings = settings.Value;
        private readonly ICustomLogService _service = service;
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _service.WriteLog();
                await Task.Delay(TimeSpan.FromMinutes(_settings.LogPeriodMinute), stoppingToken);
            }
        }
    }
}
