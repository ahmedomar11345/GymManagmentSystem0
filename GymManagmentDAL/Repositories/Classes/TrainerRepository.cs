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
    public class TrainerRepository : GenericRepository<Trainer>, ITrainerRepository
    {
        public TrainerRepository(GymDBContext dbContext) : base(dbContext)
        {
        }

        public async Task<Trainer?> GetTrainerWithSpecialtyAsync(int id)
        {
            return await _dbSet.Include(t => t.Specialty)
                              .FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<PagedResult<Trainer>> GetTrainersWithSpecialtyPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var query = _dbSet.Include(t => t.Specialty).AsNoTracking();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.ToLower();
                query = query.Where(t => t.Name.ToLower().Contains(term) || 
                                       t.Email.ToLower().Contains(term) || 
                                       t.Phone.Contains(term));
            }

            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(t => t.CreatedAt)
                                   .Skip((pageNumber - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            return new PagedResult<Trainer>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
        }
    }
}
