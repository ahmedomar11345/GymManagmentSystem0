using GymManagmentBLL.ViewModels.WalkInViewModel;
using GymManagmentDAL.Entities;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IWalkInService
    {
        /// <summary>إنشاء حجز walk-in جديد (عضو جديد أو موجود)</summary>
        Task<WalkInBooking> CreateBookingAsync(CreateWalkInViewModel model);

        /// <summary>قائمة كل الحجوزات</summary>
        Task<IEnumerable<WalkInBooking>> GetAllBookingsAsync();

        /// <summary>تفاصيل حجز واحد</summary>
        Task<WalkInBooking?> GetBookingByIdAsync(int id);

        /// <summary>تسجيل استخدام حصة من رصيد العضو</summary>
        Task<bool> UseSessionAsync(int bookingId, int? sessionId = null);

        /// <summary>الحجوزات النشطة (فيها حصص متبقية ولم تنته صلاحيتها)</summary>
        Task<IEnumerable<WalkInBooking>> GetActiveBookingsAsync();

        /// <summary>البحث باسم أو تليفون</summary>
        Task<IEnumerable<WalkInBooking>> SearchAsync(string query);

        /// <summary>حذف الحجوزات المنتهية بناءً على المدة المحددة</summary>
        Task<int> CleanupExpiredBookingsAsync(int retentionDays);
    }
}
