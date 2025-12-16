using GymManagmentDAL.Data.Context;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Reposotories.Classes
{
    internal class SessionRepository : GenericRepository<Session>
    {
        private readonly GymDBContext _dbContext;
        public SessionRepository(GymDBContext dbContext):base(dbContext)
        {
        }
    }
}
