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
    public class MemberRepository : GenericRepository<Member>, IMemberRepository
    {
        public MemberRepository(GymDBContext dbContext) : base(dbContext)
        {
        }

        public Member? GetWithDetails(int id)
        {
            return _dbContext.Members
                .Include(m => m.HealthRecord)
                .Include(m => m.Memberships)
                    .ThenInclude(ms => ms.Plan)
                .FirstOrDefault(m => m.Id == id);
        }

        public async Task<Member?> GetWithDetailsAsync(int id)
        {
            return await _dbContext.Members
                .Include(m => m.HealthRecord)
                .Include(m => m.Memberships)
                    .ThenInclude(ms => ms.Plan)
                .Include(m => m.MemberSessions)
                    .ThenInclude(ms => ms.Session)
                .FirstOrDefaultAsync(m => m.Id == id);
        }

        public IEnumerable<Member> GetAllWithDetails()
        {
            return _dbContext.Members
                .Include(m => m.HealthRecord)
                .Include(m => m.Memberships)
                    .ThenInclude(ms => ms.Plan)
                .AsNoTracking()
                .ToList();
        }

        public async Task<IEnumerable<Member>> GetAllWithDetailsAsync()
        {
            return await _dbContext.Members
                .Include(m => m.HealthRecord)
                .Include(m => m.Memberships)
                    .ThenInclude(ms => ms.Plan)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<int> GetNewMembersCountAsync(int month, int year)
        {
            return await _dbContext.Members.CountAsync(m => m.CreatedAt.Month == month && m.CreatedAt.Year == year);
        }

        public async Task<Dictionary<string, int>> GetMemberGrowthLast6MonthsAsync(int months = 6)
        {
            var result = new Dictionary<string, int>();
            var now = DateTime.Now;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = now.AddMonths(-i);
                var count = await GetNewMembersCountAsync(targetDate.Month, targetDate.Year);
                result.Add(targetDate.ToString("MMM yyyy"), count);
            }

            return result;
        }
        public async Task<Dictionary<string, int>> GetMemberGrowthByRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = new Dictionary<string, int>();
            var current = new DateTime(startDate.Year, startDate.Month, 1);
            var end = new DateTime(endDate.Year, endDate.Month, 1);

            while (current <= end)
            {
                var count = await GetNewMembersCountAsync(current.Month, current.Year);
                result.Add(current.ToString("MMM yyyy"), count);
                current = current.AddMonths(1);
            }

            return result;
        }
    }
}
