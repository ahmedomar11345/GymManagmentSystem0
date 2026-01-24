using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Classes
{
    public class SessionRepository : GenericRepository<Session>, ISessionRepository
    {
        private readonly GymDBContext _dBContext;

        public SessionRepository(GymDBContext dBContext): base(dBContext) {
        
            _dBContext = dBContext;
        }
        public IEnumerable<Session> GetAllSessionWithTrainerAndCategory()
        {
            return _dBContext.Sessions.Include(s => s.SessionTrainer).Include(x => x.SessionCategory).ToList();  
        }

        public int GetCountofBookedSlot(int sessionId)
        {  
            return _dBContext.MemberSessions.Where(x=> x.SessionId == sessionId).Count();
        }

        public Session? GetSessionWithTrainerandCategory(int sessionId)
        {
            return _dBContext.Sessions.Include(x => x.SessionTrainer)
                .Include(x => x.SessionCategory).FirstOrDefault(x=> x.Id == sessionId);
        }
    }
}
