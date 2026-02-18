using Microsoft.Extensions.Localization;
using GymManagmentBLL.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class ScheduledTaskController : Controller
    {
        private readonly IScheduledTaskService _scheduledTaskService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ScheduledTaskController(IScheduledTaskService scheduledTaskService, IStringLocalizer<SharedResource> localizer)
        {
            _scheduledTaskService = scheduledTaskService;
            _localizer = localizer;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunExpiryReminders(int days = 7, bool? isArabic = null)
        {
            var count = await _scheduledTaskService.SendMembershipExpiryRemindersAsync(days, isArabic);
            TempData["SuccessMessage"] = string.Format(_localizer["ExpiryRemindersSent"].Value, count);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunSessionReminders(int hours = 1, bool? isArabic = null)
        {
            var count = await _scheduledTaskService.SendSessionRemindersAsync(hours, isArabic);
            TempData["SuccessMessage"] = string.Format(_localizer["SessionRemindersSent"].Value, count);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunBirthdayWishes(int discount = 10, bool? isArabic = null)
        {
            var count = await _scheduledTaskService.SendBirthdayWishesAsync(discount, isArabic);
            TempData["SuccessMessage"] = string.Format(_localizer["BirthdayWishesSent"].Value, count);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunInactiveAlerts(int days = 14, bool? isArabic = null)
        {
            var count = await _scheduledTaskService.AlertInactiveMembersAsync(days, isArabic);
            TempData["SuccessMessage"] = string.Format(_localizer["InactiveMembersAlertSent"].Value, count);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunKeyRefresh(bool? isArabic = null)
        {
            var count = await _scheduledTaskService.RefreshMemberAccessKeysAsync(isArabic);
            TempData["SuccessMessage"] = string.Format(_localizer["QRCodeRefreshSuccess"].Value, count);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunExpiredNotifications(bool? isArabic = null)
        {
            var count = await _scheduledTaskService.SendMembershipExpiredNotificationsAsync(isArabic);
            TempData["SuccessMessage"] = string.Format(_localizer["ExpiredMembershipNotificationsSent"].Value, count);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunAllTasks(bool? isArabic = null)
        {
            var expiryCount = await _scheduledTaskService.SendMembershipExpiryRemindersAsync(7, isArabic);
            var sessionCount = await _scheduledTaskService.SendSessionRemindersAsync(1, isArabic);
            var birthdayCount = await _scheduledTaskService.SendBirthdayWishesAsync(10, isArabic);
            var inactiveCount = await _scheduledTaskService.AlertInactiveMembersAsync(14, isArabic);
            var expiredCount = await _scheduledTaskService.SendMembershipExpiredNotificationsAsync(isArabic);

            TempData["SuccessMessage"] = _localizer["AllTasksExecuted"].Value;
            return RedirectToAction(nameof(Index));
        }
    }
}
