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
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        public SessionRepository(GymDBContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<Session> GetAllWithTrainerAndCategory()
        {
            return _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).AsNoTracking().OrderByDescending(s => s.StartDate).ToList();
        }

        public async Task<IEnumerable<Session>> GetAllWithTrainerAndCategoryAsync()
        {
            return await _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).AsNoTracking().OrderByDescending(s => s.StartDate).ToListAsync();
        }

        public PagedResult<Session> GetPagedWithDetails(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s => s.Description.ToLower().Contains(searchTerm) || s.SessionTrainer.Name.ToLower().Contains(searchTerm));
            }
            var totalCount = query.Count();
            var items = query.OrderByDescending(s => s.StartDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<Session> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public async Task<PagedResult<Session>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(s => s.Description.ToLower().Contains(searchTerm) || s.SessionTrainer.Name.ToLower().Contains(searchTerm));
            }
            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(s => s.StartDate).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Session> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public int GetBookedSlotCount(int sessionId) => _dbContext.MemberSessions.Count(ms => ms.SessionId == sessionId);

        public async Task<int> GetBookedSlotCountAsync(int sessionId) => await _dbContext.MemberSessions.CountAsync(ms => ms.SessionId == sessionId);

        public Session? GetWithTrainerAndCategory(int sessionId) => _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).AsNoTracking().FirstOrDefault(s => s.Id == sessionId);

        public async Task<Session?> GetWithTrainerAndCategoryAsync(int sessionId) => await _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).AsNoTracking().FirstOrDefaultAsync(s => s.Id == sessionId);

        public IEnumerable<Session> GetUpcomingSessions()
        {
            var now = DateTime.Now;
            return _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).Where(s => s.StartDate > now).OrderBy(s => s.StartDate).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<Session>> GetUpcomingSessionsAsync()
        {
            var now = DateTime.Now;
            return await _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).Where(s => s.StartDate > now).OrderBy(s => s.StartDate).AsNoTracking().ToListAsync();
        }

        public IEnumerable<Session> GetOngoingSessions()
        {
            var now = DateTime.Now;
            return _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).Where(s => s.StartDate <= now && s.EndDate > now).OrderBy(s => s.EndDate).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<Session>> GetOngoingSessionsAsync()
        {
            var now = DateTime.Now;
            return await _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).Where(s => s.StartDate <= now && s.EndDate > now).OrderBy(s => s.EndDate).AsNoTracking().ToListAsync();
        }

        public IEnumerable<Session> GetCompletedSessions()
        {
            var now = DateTime.Now;
            return _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).Where(s => s.EndDate < now).OrderByDescending(s => s.EndDate).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<Session>> GetCompletedSessionsAsync()
        {
            var now = DateTime.Now;
            return await _dbContext.Sessions.Include(s => s.SessionTrainer).Include(s => s.SessionCategory).Where(s => s.EndDate < now).OrderByDescending(s => s.EndDate).AsNoTracking().ToListAsync();
        }
    }
}
