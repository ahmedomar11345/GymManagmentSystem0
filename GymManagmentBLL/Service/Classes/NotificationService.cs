using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;

        public NotificationService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<Notification>> GetUnreadNotificationsAsync(string userId)
        {
            return await _unitOfWork.NotificationRepository.GetUnreadByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Notification>> GetRecentNotificationsAsync(string userId, int count = 5)
        {
            return await _unitOfWork.NotificationRepository.GetRecentByUserIdAsync(userId, count);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _unitOfWork.NotificationRepository.MarkAsReadAsync(notificationId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(string userId)
        {
            await _unitOfWork.NotificationRepository.MarkAllAsReadAsync(userId);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task DeleteNotificationAsync(int notificationId)
        {
            var repo = _unitOfWork.GetRepository<Notification>();
            var notification = await repo.GetByIdAsync(notificationId);
            if (notification != null)
            {
                repo.Delete(notification);
                await _unitOfWork.SaveChangesAsync();
            }
        }

        public async Task DeleteAllNotificationsAsync(string userId)
        {
            var repo = _unitOfWork.NotificationRepository;
            var notifications = await repo.GetAllByUserIdAsync(userId);
            _unitOfWork.GetRepository<Notification>().DeleteRange(notifications);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task CreateNotificationAsync(string title, string message, string? url, string? userId = null, string type = "info")
        {
            var notification = new Notification
            {
                Title = title,
                Message = message,
                Url = url,
                UserId = userId,
                Type = type,
                CreatedAt = DateTime.Now,
                IsRead = false
            };

            await _unitOfWork.GetRepository<Notification>().AddAsync(notification);
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<Notification>> GetAllNotificationsAsync(string userId)
        {
            return await _unitOfWork.NotificationRepository.GetAllByUserIdAsync(userId);
        }
    }
}
