using GymManagmentDAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface INotificationService
    {
        Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId);
        Task<IEnumerable<Notification>> GetRecentNotificationsAsync(string userId, int count = 5);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task DeleteNotificationAsync(int notificationId);
        Task DeleteAllNotificationsAsync(string userId);
        Task CreateNotificationAsync(string title, string message, string? url, string? userId = null, string type = "info");
        Task<IEnumerable<Notification>> GetAllNotificationsAsync(string userId);
    }
}
