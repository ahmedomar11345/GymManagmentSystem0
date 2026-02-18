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
    public class TrainerSpecialtyRepository : GenericRepository<TrainerSpecialty>, ITrainerSpecialtyRepository
    {
        public TrainerSpecialtyRepository(GymDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<IEnumerable<TrainerSpecialty>> GetAllWithTrainersAsync()
        {
            return await _dbSet.Include(s => s.Trainers).AsNoTracking().ToListAsync();
        }
    }
}
