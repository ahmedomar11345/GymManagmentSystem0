using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Classes
{
    public class NotificationRepository : GenericRepository<Notification>, INotificationRepository
    {
        private readonly GymDBContext _context;

        public NotificationRepository(GymDBContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Notification>> GetUnreadByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => !n.IsRead && (n.UserId == userId || n.UserId == null))
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Notification>> GetRecentByUserIdAsync(string userId, int count)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId || n.UserId == null)
                .OrderByDescending(n => n.CreatedAt)
                .Take(count)
                .ToListAsync();
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            var notification = await _context.Notifications.FindAsync(notificationId);
            if (notification != null)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                // SaveChanges is handled by UnitOfWork usually, but here we might need explicit save or rely on UoW.
                // Assuming UoW pattern where Save happens later, but for single repo action like this,
                // often better to let Service handle the Save via UoW.
                // However, GenericRepository usually doesn't save.
            }
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            var notifications = await _context.Notifications
                .Where(n => !n.IsRead && (n.UserId == userId || n.UserId == null))
                .ToListAsync();

            foreach (var n in notifications)
            {
                n.IsRead = true;
            }
            _context.Notifications.UpdateRange(notifications);
        }

        public async Task<IEnumerable<Notification>> GetAllByUserIdAsync(string userId)
        {
            return await _context.Notifications
                .Where(n => n.UserId == userId || n.UserId == null)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }
    }
}
