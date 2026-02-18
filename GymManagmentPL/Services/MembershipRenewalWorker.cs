using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.Service.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GymManagmentPL.Services
{
    public class MembershipRenewalWorker : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<MembershipRenewalWorker> _logger;
        private readonly TimeSpan _checkInterval = TimeSpan.FromHours(24);

        public MembershipRenewalWorker(IServiceProvider serviceProvider, ILogger<MembershipRenewalWorker> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Membership Renewal Worker is starting.");

            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Membership Renewal Worker triggered at: {time}", DateTimeOffset.Now);

                try
                {
                    await SendRenewalReminders();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred while sending renewal reminders.");
                }

                await Task.Delay(_checkInterval, stoppingToken);
            }

            _logger.LogInformation("Membership Renewal Worker is stopping.");
        }

        private async Task SendRenewalReminders()
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var membershipService = scope.ServiceProvider.GetRequiredService<IMemberShipService>();
                var memberService = scope.ServiceProvider.GetRequiredService<IMemberService>();
                var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                var gymSettingsService = scope.ServiceProvider.GetRequiredService<IGymSettingsService>();

                // Get memberships expiring within the next 7 days
                var expiringMemberships = await membershipService.GetExpiringMemberShipsAsync(7);
                int count = 0;

                foreach (var ship in expiringMemberships)
                {
                    try
                    {
                        var member = await memberService.GetMemberDetailsAsync(ship.MemberId);
                        if (member != null && !string.IsNullOrEmpty(member.Email))
                        {
                            // Calculate remaining days
                            if (DateTime.TryParse(ship.EndDate, out DateTime endDate))
                            {
                                var remainingDays = (endDate.Date - DateTime.Now.Date).Days;
                                
                                if (remainingDays == 7 || remainingDays == 3 || remainingDays == 1)
                                {
                                    // Background service defaults to Arabic for now, as it's the primary language
                                    bool isArabic = true; 
                                    var gymSettings = await gymSettingsService.GetSettingsAsync();
                                    string alertTemplate = EmailTemplates.ExpirationAlert(member.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, ship.PlanName, remainingDays, isArabic);
                                    await emailService.SendEmailAsync(member.Email, isArabic ? "تنبيه: اقترب موعد انتهاء اشتراكك" : "Reminder: Your membership expires soon", alertTemplate);
                                    count++;
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send reminder for member {Id}", ship.MemberId);
                    }
                }

                if (count > 0)
                {
                    _logger.LogInformation("Sent {Count} membership renewal reminders.", count);
                }
            }
        }
    }
}
