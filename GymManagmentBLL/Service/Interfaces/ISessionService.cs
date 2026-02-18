using GymManagmentBLL.ViewModels.SessionViewModel;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface ISessionService
    {
        Task<IEnumerable<SessionViewModel>> GetAllSessionAsync();
        Task<PagedResult<SessionViewModel>> GetSessionsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<bool> CreateSessionAsync(CreateSessionViewModel createSession);
        Task<SessionViewModel?> GetSessionByIdAsync(int sessionId);
        Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId);
        Task<bool> UpdateSessionAsync(int sessionId, UpdateSessionViewModel updateSession);
        Task<bool> RemoveSessionAsync(int sessionId);
        Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync();
        Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync();
        Task<int> CleanupOldSessionsAsync(int daysOld);
    }
}
