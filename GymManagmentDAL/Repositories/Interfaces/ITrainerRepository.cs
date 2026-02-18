using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface ITrainerRepository : IGenericRepository<Trainer>
    {
        Task<Trainer?> GetTrainerWithSpecialtyAsync(int id);
        Task<PagedResult<Trainer>> GetTrainersWithSpecialtyPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
    }
}
