using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class ScheduledTaskService : IScheduledTaskService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailService _emailService;
        private readonly INotificationService _notificationService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly ILogger<ScheduledTaskService> _logger;

        public ScheduledTaskService(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            INotificationService notificationService,
            IGymSettingsService gymSettingsService,
            ILogger<ScheduledTaskService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailService = emailService;
            _notificationService = notificationService;
            _gymSettingsService = gymSettingsService;
            _logger = logger;
        }

        private bool IsArabic => CultureInfo.CurrentUICulture.Name.StartsWith("ar");

        public async Task<int> SendMembershipExpiryRemindersAsync(int daysBeforeExpiry = 7, bool? isArabic = null)
        {
            int remindersSent = 0;
            bool useArabic = isArabic ?? IsArabic;

            try
            {
                var targetDate = DateTime.Now.Date.AddDays(daysBeforeExpiry);
                var today = DateTime.Now.Date;

                var expiringMemberships = await _unitOfWork.GetRepository<MemberShip>()
                    .GetAllAsync(ms => 
                        !ms.IsDeleted && 
                        ms.EndDate.Date == targetDate &&
                        ms.EndDate > today);

                if (!expiringMemberships.Any())
                {
                    _logger.LogInformation("No memberships expiring in {Days} days.", daysBeforeExpiry);
                    return 0;
                }

                foreach (var membership in expiringMemberships)
                {
                    try
                    {
                        var member = await _unitOfWork.MemberRepository.GetByIdAsync(membership.MemberId);
                        if (member == null || member.IsDeleted) continue;

                        var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(membership.PlanId);
                        if (plan == null) continue;

                        var existingReminder = await _unitOfWork.GetRepository<Notification>()
                            .ExistsAsync(n => 
                                n.Title == "Membership Expiry Reminder" && 
                                n.Message.Contains(member.Name) &&
                                n.CreatedAt.Date == today);

                        if (existingReminder) continue;

                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string emailContent = EmailTemplates.ExpirationAlert(
                            member.Name,
                            gymSettings.GymName,
                            gymSettings.Phone,
                            gymSettings.Address,
                            gymSettings.Email,
                            plan.Name,
                            daysBeforeExpiry,
                            useArabic
                        );

                        string subject = useArabic ? "⚠️ تنبيه: اشتراكك سينتهي قريباً" : "⚠️ Alert: Your membership is expiring soon";

                        await _emailService.SendEmailAsync(member.Email, subject, emailContent);

                        await _notificationService.CreateNotificationAsync(
                            "Membership Expiry Reminder",
                            $"Reminder sent to {member.Name} - Expires on {membership.EndDate:dd/MM/yyyy}",
                            $"/Member/Details/{member.Id}",
                            null,
                            "warning"
                        );

                        remindersSent++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send reminder for membership {MembershipId}", membership.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMembershipExpiryRemindersAsync");
            }

            return remindersSent;
        }

        public async Task<int> SendSessionRemindersAsync(int hoursBeforeSession = 1, bool? isArabic = null)
        {
            int remindersSent = 0;
            bool useArabic = isArabic ?? IsArabic;

            try
            {
                var now = DateTime.Now;
                var targetTime = now.AddHours(hoursBeforeSession);

                var upcomingSessions = await _unitOfWork.SessionRepository.GetAllAsync(s =>
                    !s.IsDeleted &&
                    s.StartDate > now &&
                    s.StartDate <= targetTime);

                foreach (var session in upcomingSessions)
                {
                    var bookings = await _unitOfWork.MemberSessionRepository.GetAllAsync(ms =>
                        ms.SessionId == session.Id && !ms.IsDeleted);

                    foreach (var booking in bookings)
                    {
                        try
                        {
                            var member = await _unitOfWork.MemberRepository.GetByIdAsync(booking.MemberId);
                            if (member == null || member.IsDeleted) continue;

                            var todayStart = DateTime.Today;
                            var existingReminder = await _unitOfWork.GetRepository<Notification>()
                                .ExistsAsync(n =>
                                    n.Title == "Session Reminder" &&
                                    n.Message.Contains($"Session {session.Id}") &&
                                    n.Message.Contains(member.Name) &&
                                    n.CreatedAt >= todayStart);

                            if (existingReminder) continue;

                            var category = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
                            string sessionName = category?.CategoryName.ToString() ?? "Training Session";

                            var gymSettings = await _gymSettingsService.GetSettingsAsync();
                            string emailContent = EmailTemplates.BookingConfirmation(
                                member.Name,
                                gymSettings.GymName,
                                gymSettings.Phone,
                                gymSettings.Address,
                                gymSettings.Email,
                                sessionName,
                                session.StartDate,
                                useArabic
                            );

                            string subject = useArabic 
                                ? $"⏰ تذكير: جلستك تبدأ خلال ساعة - {sessionName}" 
                                : $"⏰ Reminder: Your session starts in an hour - {sessionName}";

                            await _emailService.SendEmailAsync(member.Email, subject, emailContent);

                            await _notificationService.CreateNotificationAsync(
                                "Session Reminder",
                                $"Session {session.Id} reminder sent to {member.Name}",
                                $"/Session/Details/{session.Id}",
                                null,
                                "info"
                            );

                            remindersSent++;
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Failed to send session reminder for booking {BookingId}", booking.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendSessionRemindersAsync");
            }

            return remindersSent;
        }

        public async Task<int> SendBirthdayWishesAsync(int discountPercentage = 10, bool? isArabic = null)
        {
            int wishesSent = 0;
            bool useArabic = isArabic ?? IsArabic;

            try
            {
                var today = DateTime.Today;
                var allMembers = (await _unitOfWork.MemberRepository.GetAllAsync(m => !m.IsDeleted)).ToList();

                var birthdayMembers = allMembers.Where(m =>
                    m.DateOfBirth.Month == today.Month &&
                    m.DateOfBirth.Day == today.Day);

                foreach (var member in birthdayMembers)
                {
                    try
                    {
                        var existingWish = await _unitOfWork.GetRepository<Notification>()
                            .ExistsAsync(n =>
                                n.Title == "Birthday Wish Sent" &&
                                n.Message.Contains(member.Name) &&
                                n.CreatedAt.Date == today);

                        if (existingWish) continue;

                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string emailContent = EmailTemplates.BirthdayWish(member.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, discountPercentage, useArabic);
                        string subject = useArabic 
                            ? $"🎂 عيد ميلاد سعيد يا {member.Name}! 🎉" 
                            : $"🎂 Happy Birthday, {member.Name}! 🎉";

                        await _emailService.SendEmailAsync(member.Email, subject, emailContent);

                        await _notificationService.CreateNotificationAsync(
                            "Birthday Wish Sent",
                            $"Birthday wish sent to {member.Name} with {discountPercentage}% discount",
                            $"/Member/Details/{member.Id}",
                            null,
                            "success"
                        );

                        wishesSent++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send birthday wish to {MemberId}", member.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendBirthdayWishesAsync");
            }

            return wishesSent;
        }

        public async Task<int> AlertInactiveMembersAsync(int inactiveDays = 14, bool? isArabic = null)
        {
            int inactiveCount = 0;
            bool useArabic = isArabic ?? IsArabic;

            try
            {
                var cutoffDate = DateTime.Now.AddDays(-inactiveDays);
                var allMembers = (await _unitOfWork.MemberRepository.GetAllAsync(m => !m.IsDeleted)).ToList();

                var inactiveMembers = new List<Member>();

                foreach (var member in allMembers)
                {
                    var lastCheckIn = (await _unitOfWork.GetRepository<CheckIn>()
                        .GetAllAsync(c => c.MemberId == member.Id))
                        .OrderByDescending(c => c.CheckInTime)
                        .FirstOrDefault();

                    if (lastCheckIn == null || lastCheckIn.CheckInTime < cutoffDate)
                    {
                        var hasActiveMembership = await _unitOfWork.GetRepository<MemberShip>()
                            .ExistsAsync(ms => ms.MemberId == member.Id && ms.EndDate > DateTime.Now && !ms.IsDeleted);

                        if (hasActiveMembership)
                        {
                            inactiveMembers.Add(member);
                        }
                    }
                }

                if (inactiveMembers.Any())
                {
                    var memberNames = string.Join(", ", inactiveMembers.Take(5).Select(m => m.Name));
                    var moreRaw = inactiveMembers.Count > 5 ? (useArabic ? $" و {inactiveMembers.Count - 5} آخرين" : $" and {inactiveMembers.Count - 5} others") : "";

                    string message = useArabic 
                        ? $"هناك {inactiveMembers.Count} أعضاء لم يحضروا منذ {inactiveDays} يوماً: {memberNames}{moreRaw}"
                        : $"{inactiveMembers.Count} members inactive for {inactiveDays}+ days: {memberNames}{moreRaw}";

                    await _notificationService.CreateNotificationAsync(
                        "Inactive Members Alert",
                        message,
                        "/Member/Index",
                        null,
                        "warning"
                    );

                    inactiveCount = inactiveMembers.Count;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in AlertInactiveMembersAsync");
            }

            return inactiveCount;
        }

        public async Task<int> RefreshMemberAccessKeysAsync(bool? isArabic = null)
        {
            int count = 0;
            try
            {
                var activeMemberships = await _unitOfWork.GetRepository<MemberShip>()
                    .GetAllAsync(ms => ms.EndDate > DateTime.Now && !ms.IsDeleted);
                
                var activeMemberIds = activeMemberships.Select(ms => ms.MemberId).Distinct().ToList();
                
                foreach (var memberId in activeMemberIds)
                {
                    var result = await RefreshSingleMemberKeyAsync(memberId, isArabic);
                    if (result) count++;
                }

                if (count > 0)
                {
                    await _notificationService.CreateNotificationAsync(
                        "Security: QR Codes Refreshed",
                        $"Successfully refreshed access keys and sent new QR codes to {count} active members.",
                        "/Member/Index",
                        null,
                        "info"
                    );
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in RefreshMemberAccessKeysAsync");
            }
            return count;
        }

        private async Task<bool> RefreshSingleMemberKeyAsync(int memberId, bool? isArabic = null)
        {
            try
            {
                bool useArabic = isArabic ?? IsArabic;
                var member = await _unitOfWork.MemberRepository.GetByIdAsync(memberId);
                if (member == null || member.IsDeleted) return false;

                member.AccessKey = Guid.NewGuid().ToString("N");
                _unitOfWork.MemberRepository.Update(member);
                await _unitOfWork.SaveChangesAsync();

                using (var qrGenerator = new QRCoder.QRCodeGenerator())
                {
                    var qrCodeData = qrGenerator.CreateQrCode(member.AccessKey, QRCoder.QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCoder.PngByteQRCode(qrCodeData))
                    {
                        byte[] qrCodeBytes = qrCode.GetGraphic(20);
                        string qrContentId = $"qrcode_refresh_{member.Id}_{Guid.NewGuid():N}";
                        
                        string customMsg = useArabic 
                            ? "تم تحديث رمز الدخول الخاص بك لدواعي الأمان المتبعة دورياً. يرجى استخدام الرمز الجديد للدخول."
                            : "Your access key has been refreshed for security. Please use this new QR code for your next visit.";

                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string template = EmailTemplates.MemberQRCodeWithCID(member.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, qrContentId, useArabic, customMsg);
                        
                        string subject = useArabic ? "أمان: رمز الدخول QR كود الجديد" : "Security: Your new QR access code";

                        await _emailService.SendEmailWithImageAsync(
                            member.Email,
                            subject,
                            template,
                            qrCodeBytes,
                            qrContentId
                        );
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to refresh key for member {Id}", memberId);
                return false;
            }
        }

        public async Task<int> SendMembershipExpiredNotificationsAsync(bool? isArabic = null)
        {
            int notificationsSent = 0;
            bool useArabic = isArabic ?? IsArabic;

            try
            {
                var today = DateTime.Now.Date;

                // Find memberships that expired today (EndDate.Date == today)
                var expiredMemberships = await _unitOfWork.GetRepository<MemberShip>()
                    .GetAllAsync(ms =>
                        !ms.IsDeleted &&
                        ms.EndDate.Date == today);

                if (!expiredMemberships.Any())
                {
                    _logger.LogInformation("No memberships expired today.");
                    return 0;
                }

                foreach (var membership in expiredMemberships)
                {
                    try
                    {
                        var member = await _unitOfWork.MemberRepository.GetByIdAsync(membership.MemberId);
                        if (member == null || member.IsDeleted) continue;

                        var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(membership.PlanId);
                        if (plan == null) continue;

                        // Check if we already sent this notification today
                        var alreadySent = await _unitOfWork.GetRepository<Notification>()
                            .ExistsAsync(n =>
                                n.Title == "Membership Expired" &&
                                n.Message.Contains(member.Name) &&
                                n.CreatedAt.Date == today);

                        if (alreadySent) continue;

                        // Check that the member doesn't have another active membership
                        var hasOtherActive = await _unitOfWork.GetRepository<MemberShip>()
                            .ExistsAsync(ms =>
                                ms.MemberId == member.Id &&
                                ms.Id != membership.Id &&
                                ms.EndDate > today &&
                                !ms.IsDeleted);

                        if (hasOtherActive) continue;

                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string emailContent = EmailTemplates.MembershipExpired(
                            member.Name,
                            gymSettings.GymName,
                            gymSettings.Phone,
                            gymSettings.Address,
                            gymSettings.Email,
                            plan.Name,
                            membership.EndDate,
                            useArabic
                        );

                        string subject = useArabic
                            ? "🔔 إشعار: اشتراكك في النادي قد انتهى"
                            : "🔔 Notice: Your gym membership has expired";

                        await _emailService.SendEmailAsync(member.Email, subject, emailContent);

                        await _notificationService.CreateNotificationAsync(
                            "Membership Expired",
                            $"{member.Name}'s membership ({plan.Name}) expired on {membership.EndDate:dd/MM/yyyy}",
                            $"/Member/Details/{member.Id}",
                            null,
                            "danger"
                        );

                        notificationsSent++;
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send expired membership notification for membership {MembershipId}", membership.Id);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SendMembershipExpiredNotificationsAsync");
            }

            return notificationsSent;
        }
    }
}
