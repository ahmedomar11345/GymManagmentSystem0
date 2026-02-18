using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IScheduledTaskService
    {
        /// <summary>
        /// Sends reminder emails to members whose memberships expire within the specified days.
        /// </summary>
        /// <param name="daysBeforeExpiry">Number of days before expiry to send reminder (default: 7)</param>
        /// <returns>Number of reminders sent</returns>
        Task<int> SendMembershipExpiryRemindersAsync(int daysBeforeExpiry = 7, bool? isArabic = null);

        /// <summary>
        /// Sends reminder emails to members with upcoming session bookings.
        /// </summary>
        /// <param name="hoursBeforeSession">Hours before session to send reminder (default: 1)</param>
        /// <returns>Number of reminders sent</returns>
        Task<int> SendSessionRemindersAsync(int hoursBeforeSession = 1, bool? isArabic = null);

        /// <summary>
        /// Sends birthday wishes to members celebrating their birthday today.
        /// </summary>
        /// <param name="discountPercentage">Discount percentage to offer in the birthday wish (default: 10)</param>
        /// <returns>Number of wishes sent</returns>
        Task<int> SendBirthdayWishesAsync(int discountPercentage = 10, bool? isArabic = null);

        /// <summary>
        /// Alerts admin about members who haven't checked in for specified days.
        /// </summary>
        /// <param name="inactiveDays">Days without check-in to be considered inactive (default: 14)</param>
        /// <returns>Number of inactive members found</returns>
        Task<int> AlertInactiveMembersAsync(int inactiveDays = 14, bool? isArabic = null);

        /// <summary>
        /// Refreshes access keys and sends new QR codes to all active members.
        /// </summary>
        /// <returns>Number of keys refreshed</returns>
        Task<int> RefreshMemberAccessKeysAsync(bool? isArabic = null);

        /// <summary>
        /// Sends notification emails to members whose memberships have expired today.
        /// </summary>
        /// <returns>Number of expired membership notifications sent</returns>
        Task<int> SendMembershipExpiredNotificationsAsync(bool? isArabic = null);
    }
}
