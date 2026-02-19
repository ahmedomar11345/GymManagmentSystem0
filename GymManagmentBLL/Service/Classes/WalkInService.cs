using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.WalkInViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;

namespace GymManagmentBLL.Service.Classes
{
    public class WalkInService : IWalkInService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGymSettingsService _gymSettingsService;

        public WalkInService(IUnitOfWork unitOfWork, IGymSettingsService gymSettingsService)
        {
            _unitOfWork = unitOfWork;
            _gymSettingsService = gymSettingsService;
        }

        public async Task<WalkInBooking> CreateBookingAsync(CreateWalkInViewModel model)
        {
            if (model.PricePerSession < 0)
                throw new ArgumentException("Price per session cannot be negative.");

            var booking = new WalkInBooking
            {
                GuestName      = model.GuestName.Trim(),
                GuestPhone     = model.GuestPhone.Trim(),
                MemberId       = model.MemberId,
                SessionCount   = model.SessionCount,
                PricePerSession = model.PricePerSession,
                SessionsUsed   = 0,
                ExpiryDate     = DateTime.Now.AddDays(1), // صلاحية يوم واحد فقط
                Notes          = model.Notes
            };

            await _unitOfWork.GetRepository<WalkInBooking>().AddAsync(booking);
            await _unitOfWork.SaveChangesAsync();
            return booking;
        }

        public async Task<IEnumerable<WalkInBooking>> GetAllBookingsAsync()
        {
            return await _unitOfWork.GetRepository<WalkInBooking>()
                .GetAllAsync(null, b => b.WalkInSessions, b => b.Member!);
        }

        public async Task<WalkInBooking?> GetBookingByIdAsync(int id)
        {
            var all = await _unitOfWork.GetRepository<WalkInBooking>()
                .GetAllAsync(b => b.Id == id, b => b.WalkInSessions, b => b.Member!);
            return all.FirstOrDefault();
        }

        public async Task<bool> UseSessionAsync(int bookingId, int? sessionId = null)
        {
            var booking = await GetBookingByIdAsync(bookingId);
            if (booking == null) return false;
            if (booking.SessionsRemaining <= 0) return false;
            if (booking.ExpiryDate < DateTime.Now) return false;

            // تسجيل استخدام الحصة
            var walkInSession = new WalkInSession
            {
                WalkInBookingId = bookingId,
                SessionId       = sessionId,
                IsAttended      = true,
                AttendedAt      = DateTime.Now
            };
            await _unitOfWork.GetRepository<WalkInSession>().AddAsync(walkInSession);

            // خصم حصة من الرصيد
            booking.SessionsUsed++;
            _unitOfWork.GetRepository<WalkInBooking>().Update(booking);

            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<WalkInBooking>> GetActiveBookingsAsync()
        {
            var all = await _unitOfWork.GetRepository<WalkInBooking>()
                .GetAllAsync(b => b.SessionsUsed < b.SessionCount && b.ExpiryDate >= DateTime.Now,
                             b => b.Member!);
            return all;
        }

        public async Task<IEnumerable<WalkInBooking>> SearchAsync(string query)
        {
            query = query.Trim().ToLower();
            var all = await _unitOfWork.GetRepository<WalkInBooking>()
                .GetAllAsync(b => b.GuestName.ToLower().Contains(query) || b.GuestPhone.Contains(query),
                             b => b.Member!);
            return all;
        }

        public async Task<int> CleanupExpiredBookingsAsync(int retentionDays)
        {
            var cutoffDate = DateTime.Now.AddDays(-retentionDays);
            
            // نجلب الحجوزات المنتهية تاريخياً أو التي استنفدت حصصها ومضى عليها الوقت المحدد
            var expiredBookings = await _unitOfWork.GetRepository<WalkInBooking>()
                .GetAllAsync(b => b.ExpiryDate < cutoffDate || 
                                (b.SessionsUsed >= b.SessionCount && b.CreatedAt < cutoffDate));

            int count = 0;
            foreach (var booking in expiredBookings)
            {
                _unitOfWork.GetRepository<WalkInBooking>().Delete(booking);
                count++;
            }

            if (count > 0)
            {
                await _unitOfWork.SaveChangesAsync();
            }

            return count;
        }
    }
}
