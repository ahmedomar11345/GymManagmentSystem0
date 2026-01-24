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
        IEnumerable<SessionForBookingViewModel> GetUpcomingSessions();
        IEnumerable<SessionForBookingViewModel> GetOngoingSessions();
        SessionMembersViewModel? GetMembersForUpcomingSession(int sessionId);
        SessionMembersViewModel? GetMembersForOngoingSession(int sessionId);
        bool CreateBooking(CreateBookingViewModel createBooking);
        bool CancelBooking(int memberId, int sessionId);
        bool MarkAttendance(int memberId, int sessionId);
        IEnumerable<MemberForBookingSelectViewModel> GetMembersWithActiveMembershipForDropDown(int sessionId);
    }
}
