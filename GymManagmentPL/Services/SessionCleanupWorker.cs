using GymManagmentBLL.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagmentPL.Services
{
    public class SessionCleanupWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<SessionCleanupWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24); // Check everyday

        public SessionCleanupWorker(IServiceProvider serviceProvider, ILogger<SessionCleanupWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Session Cleanup Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Session Cleanup Worker running at: {time}", DateTimeOffset.Now);

                try
                {
                    await CleanupOldSessions();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while cleaning up old sessions.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Session Cleanup Worker is stopping.");
        }

        private async Task CleanupOldSessions()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var sessionService = scope.ServiceProvider.GetRequiredService<ISessionService>();
                var scheduledTaskService = scope.ServiceProvider.GetRequiredService<IScheduledTaskService>();
                
                // Logic: Delete sessions where EndDate is older than 30 days
                await sessionService.CleanupOldSessionsAsync(30);

                // Auto-cleanup for Walk-in bookings based on gym settings
                await scheduledTaskService.CleanupWalkInBookingsAsync();
            }
        }
    }
}
