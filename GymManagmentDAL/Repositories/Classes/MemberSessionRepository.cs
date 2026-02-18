using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Classes
{
    public class MemberSessionRepository : GenericRepository<MemberSession>, IMemberSessionRepository
    {
        public MemberSessionRepository(GymDBContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<MemberSession> GetSessionBookingsWithMembers(int sessionId)
        {
            return _dbContext.MemberSessions.Include(ms => ms.Member).Where(ms => ms.SessionId == sessionId).OrderByDescending(ms => ms.CreatedAt).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<MemberSession>> GetSessionBookingsWithMembersAsync(int sessionId)
        {
            return await _dbContext.MemberSessions.Include(ms => ms.Member).Where(ms => ms.SessionId == sessionId).OrderByDescending(ms => ms.CreatedAt).AsNoTracking().ToListAsync();
        }

        public MemberSession? GetByCompositeKey(int memberId, int sessionId) => _dbContext.MemberSessions.Include(ms => ms.Member).Include(ms => ms.Session).FirstOrDefault(ms => ms.MemberId == memberId && ms.SessionId == sessionId);

        public async Task<MemberSession?> GetByCompositeKeyAsync(int memberId, int sessionId) => await _dbContext.MemberSessions.Include(ms => ms.Member).Include(ms => ms.Session).FirstOrDefaultAsync(ms => ms.MemberId == memberId && ms.SessionId == sessionId);

        public int GetBookingCount(int sessionId) => _dbContext.MemberSessions.Count(ms => ms.SessionId == sessionId && ms.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Confirmed);

        public async Task<int> GetBookingCountAsync(int sessionId) => await _dbContext.MemberSessions.CountAsync(ms => ms.SessionId == sessionId && ms.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Confirmed);

        public int GetWaitlistCount(int sessionId) => _dbContext.MemberSessions.Count(ms => ms.SessionId == sessionId && ms.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Waitlisted);

        public async Task<int> GetWaitlistCountAsync(int sessionId) => await _dbContext.MemberSessions.CountAsync(ms => ms.SessionId == sessionId && ms.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Waitlisted);

        public bool IsAlreadyBooked(int memberId, int sessionId) => _dbContext.MemberSessions.Any(ms => ms.MemberId == memberId && ms.SessionId == sessionId);

        public async Task<bool> IsAlreadyBookedAsync(int memberId, int sessionId) => await _dbContext.MemberSessions.AnyAsync(ms => ms.MemberId == memberId && ms.SessionId == sessionId);

        public IEnumerable<MemberSession> GetMemberUpcomingBookings(int memberId)
        {
            var now = DateTime.Now;
            return _dbContext.MemberSessions.Include(ms => ms.Session).ThenInclude(s => s.SessionCategory).Include(ms => ms.Session).ThenInclude(s => s.SessionTrainer).Where(ms => ms.MemberId == memberId && ms.Session.StartDate > now).OrderBy(ms => ms.Session.StartDate).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<MemberSession>> GetMemberUpcomingBookingsAsync(int memberId)
        {
            var now = DateTime.Now;
            return await _dbContext.MemberSessions.Include(ms => ms.Session).ThenInclude(s => s.SessionCategory).Include(ms => ms.Session).ThenInclude(s => s.SessionTrainer).Where(ms => ms.MemberId == memberId && ms.Session.StartDate > now).OrderBy(ms => ms.Session.StartDate).AsNoTracking().ToListAsync();
        }

        public (int TotalBooked, int Attended) GetAttendanceStats(int sessionId)
        {
            var bookings = _dbContext.MemberSessions.Where(ms => ms.SessionId == sessionId).Select(ms => ms.IsAttended).ToList();
            return (bookings.Count, bookings.Count(a => a));
        }

        public async Task<(int TotalBooked, int Attended)> GetAttendanceStatsAsync(int sessionId)
        {
            var bookings = await _dbContext.MemberSessions.Where(ms => ms.SessionId == sessionId).Select(ms => ms.IsAttended).ToListAsync();
            return (bookings.Count, bookings.Count(a => a));
        }
    }
}
