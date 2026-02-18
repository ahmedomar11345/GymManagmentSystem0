using GymManagmentBLL.ViewModels.MemberSessionViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IMemberSessionService
    {
        Task<IEnumerable<SessionForBookingViewModel>> GetUpcomingSessionsAsync();
        Task<IEnumerable<SessionForBookingViewModel>> GetOngoingSessionsAsync();
        Task<SessionMembersViewModel?> GetMembersForUpcomingSessionAsync(int sessionId);
        Task<SessionMembersViewModel?> GetMembersForOngoingSessionAsync(int sessionId);
        Task<(bool Success, string Message)> CreateBookingAsync(CreateBookingViewModel createBooking);
        Task<bool> CancelBookingAsync(int memberId, int sessionId);
        Task<bool> MarkAttendanceAsync(int memberId, int sessionId);
        Task<IEnumerable<MemberForBookingSelectViewModel>> GetMembersWithActiveMembershipForDropDownAsync(int sessionId);
        Task<string?> GetSessionNameAsync(int sessionId);
        Task<SessionMembersViewModel?> GetSessionMembersAsync(int sessionId);
    }
}
