using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface ISessionRepository : IGenericRepository<Session>
    {
        IEnumerable<Session> GetAllWithTrainerAndCategory();
        Task<IEnumerable<Session>> GetAllWithTrainerAndCategoryAsync();
        
        PagedResult<Session> GetPagedWithDetails(int pageNumber, int pageSize, string? searchTerm = null);
        Task<PagedResult<Session>> GetPagedWithDetailsAsync(int pageNumber, int pageSize, string? searchTerm = null);
        
        int GetBookedSlotCount(int sessionId);
        Task<int> GetBookedSlotCountAsync(int sessionId);
        
        Session? GetWithTrainerAndCategory(int sessionId);
        Task<Session?> GetWithTrainerAndCategoryAsync(int sessionId);
        
        IEnumerable<Session> GetUpcomingSessions();
        Task<IEnumerable<Session>> GetUpcomingSessionsAsync();
        
        IEnumerable<Session> GetOngoingSessions();
        Task<IEnumerable<Session>> GetOngoingSessionsAsync();
        
        IEnumerable<Session> GetCompletedSessions();
        Task<IEnumerable<Session>> GetCompletedSessionsAsync();
    }
}
