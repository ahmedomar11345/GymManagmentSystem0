using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotificationController(INotificationService notificationService, UserManager<ApplicationUser> userManager)
        {
            _notificationService = notificationService;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var notifications = await _notificationService.GetAllNotificationsAsync(userId);
            return View(notifications);
        }

        [HttpGet]
        public async Task<IActionResult> GetRecent()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            var notifications = await _notificationService.GetRecentNotificationsAsync(userId, 5);
            var unreadCount = (await _notificationService.GetUnreadNotificationsAsync(userId)).Count();

            return Json(new { notifications, unreadCount });
        }

        [HttpPost]
        [Route("Notification/MarkAsRead/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead([FromRoute] int id)
        {
            if (id <= 0) return BadRequest();
            
            await _notificationService.DeleteNotificationAsync(id);
            return Ok(new { success = true });
        }

        [HttpPost]
        [Route("Notification/MarkAllAsRead")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? string.Empty;
            await _notificationService.DeleteAllNotificationsAsync(userId);
            return Ok(new { success = true });
        }
    }
}
