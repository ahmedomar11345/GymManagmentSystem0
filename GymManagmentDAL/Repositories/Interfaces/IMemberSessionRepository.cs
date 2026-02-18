using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface IMemberSessionRepository : IGenericRepository<MemberSession>
    {
        IEnumerable<MemberSession> GetSessionBookingsWithMembers(int sessionId);
        Task<IEnumerable<MemberSession>> GetSessionBookingsWithMembersAsync(int sessionId);
        
        MemberSession? GetByCompositeKey(int memberId, int sessionId);
        Task<MemberSession?> GetByCompositeKeyAsync(int memberId, int sessionId);
        
        int GetBookingCount(int sessionId);
        Task<int> GetBookingCountAsync(int sessionId);
        
        int GetWaitlistCount(int sessionId);
        Task<int> GetWaitlistCountAsync(int sessionId);
        
        bool IsAlreadyBooked(int memberId, int sessionId);
        Task<bool> IsAlreadyBookedAsync(int memberId, int sessionId);
        
        IEnumerable<MemberSession> GetMemberUpcomingBookings(int memberId);
        Task<IEnumerable<MemberSession>> GetMemberUpcomingBookingsAsync(int memberId);
        
        (int TotalBooked, int Attended) GetAttendanceStats(int sessionId);
        Task<(int TotalBooked, int Attended)> GetAttendanceStatsAsync(int sessionId);
    }
}
