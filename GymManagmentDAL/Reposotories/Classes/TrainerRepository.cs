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
    internal class TrainerRepository : GenericRepository<Trainer>
    {
        private readonly GymDBContext _dbContext;

        public TrainerRepository(GymDBContext dbContext): base(dbContext)
        {
        }

    }
}
