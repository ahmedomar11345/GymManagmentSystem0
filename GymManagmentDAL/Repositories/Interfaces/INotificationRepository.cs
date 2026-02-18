using GymManagmentDAL.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface INotificationRepository : IGenericRepository<Notification>
    {
        Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId);
        Task<IEnumerable<Notification>> GetRecentByUserIdAsync(string userId, int count);
        Task MarkAsReadAsync(int notificationId);
        Task MarkAllAsReadAsync(string userId);
        Task<IEnumerable<Notification>> GetAllByUserIdAsync(string userId);
    }
}
