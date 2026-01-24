using GymManagmentBLL.ViewModels.SessionViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface ISessionService
    {
        IEnumerable<SessionViewModel> GetAllSession();

        SessionViewModel? GetSessionById(int Sessionid);
        bool CreateSession(CreateSessionViewModel createSession);
        UpdateSessionViewModel? GetSessionToUpdate(int sessionId);
        bool UpdateSession(int sessionId, UpdateSessionViewModel updateSession);
        bool RemoveSession(int sessionId);

        IEnumerable<TrainerSelectViewModel> GetTrainerForDropDown();
        IEnumerable<CategorySelectViewModel> GetCategoryForDropDown();

    }
}
