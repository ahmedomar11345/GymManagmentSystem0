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
    public class MemberShipRepository : GenericRepository<MemberShip>, IMemberShipRepository
    {
        public MemberShipRepository(GymDBContext dbContext) : base(dbContext)
        {
        }

        public IEnumerable<MemberShip> GetAllActiveWithDetails()
        {
            var now = DateTime.Now;
            return _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).Where(ms => ms.EndDate >= DateTime.Now.Date).OrderByDescending(ms => ms.CreatedAt).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<MemberShip>> GetAllActiveWithDetailsAsync()
        {
            var now = DateTime.Now;
            return await _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).Where(ms => ms.EndDate >= DateTime.Now.Date).OrderByDescending(ms => ms.CreatedAt).AsNoTracking().ToListAsync();
        }

        public PagedResult<MemberShip> GetPagedActiveWithDetails(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var now = DateTime.Now;
            var query = _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).Where(ms => ms.EndDate >= DateTime.Now.Date).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(ms => ms.Member.Name.ToLower().Contains(searchTerm) || ms.Member.Email.ToLower().Contains(searchTerm) || ms.Plan.Name.ToLower().Contains(searchTerm));
            }
            var totalCount = query.Count();
            var items = query.OrderByDescending(ms => ms.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<MemberShip> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public async Task<PagedResult<MemberShip>> GetPagedActiveWithDetailsAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var now = DateTime.Now;
            var query = _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).Where(ms => ms.EndDate >= DateTime.Now.Date).AsNoTracking();
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                searchTerm = searchTerm.ToLower();
                query = query.Where(ms => ms.Member.Name.ToLower().Contains(searchTerm) || ms.Member.Email.ToLower().Contains(searchTerm) || ms.Plan.Name.ToLower().Contains(searchTerm));
            }
            var totalCount = await query.CountAsync();
            var items = await query.OrderByDescending(ms => ms.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<MemberShip> { Items = items, TotalCount = totalCount, PageNumber = pageNumber, PageSize = pageSize };
        }

        public MemberShip? GetByCompositeKey(int memberId, int planId) => _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).FirstOrDefault(ms => ms.MemberId == memberId && ms.PlanId == planId);

        public async Task<MemberShip?> GetByCompositeKeyAsync(int memberId, int planId) => await _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).FirstOrDefaultAsync(ms => ms.MemberId == memberId && ms.PlanId == planId);

        public bool HasActiveMembership(int memberId)
        {
            var now = DateTime.Now;
            return _dbContext.MemberShips.Any(ms => ms.MemberId == memberId && ms.EndDate >= now.Date);
        }

        public async Task<bool> HasActiveMembershipAsync(int memberId)
        {
            var now = DateTime.Now;
            return await _dbContext.MemberShips.AnyAsync(ms => ms.MemberId == memberId && ms.EndDate >= now.Date);
        }

        public async Task<MemberShip?> GetActiveMembershipWithPlanByMemberIdAsync(int memberId)
        {
            var now = DateTime.Now;
            return await _dbContext.MemberShips
                .Include(ms => ms.Plan)
                .OrderByDescending(ms => ms.CreatedAt)
                .FirstOrDefaultAsync(ms => ms.MemberId == memberId && ms.EndDate >= now.Date);
        }

        public async Task<bool> AnyActiveWithPlanIdAsync(int planId)
        {
            var now = DateTime.Now;
            return await _dbContext.MemberShips.AnyAsync(ms => ms.PlanId == planId && ms.EndDate >= now.Date);
        }

        public IEnumerable<MemberShip> GetExpiringWithin(int days)
        {
            var now = DateTime.Now;
            var expirationDate = now.AddDays(days);
            return _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).Where(ms => ms.EndDate > now && ms.EndDate <= expirationDate).OrderBy(ms => ms.EndDate).AsNoTracking().ToList();
        }

        public async Task<IEnumerable<MemberShip>> GetExpiringWithinAsync(int days)
        {
            var now = DateTime.Now;
            var expirationDate = now.AddDays(days);
            return await _dbContext.MemberShips.Include(ms => ms.Member).Include(ms => ms.Plan).Where(ms => ms.EndDate > now && ms.EndDate <= expirationDate).OrderBy(ms => ms.EndDate).AsNoTracking().ToListAsync();
        }

        public int GetActiveCount()
        {
            return _dbContext.MemberShips.Count(ms => ms.EndDate >= DateTime.Now.Date);
        }
 
        public async Task<int> GetActiveCountAsync()
        {
            return await _dbContext.MemberShips.CountAsync(ms => ms.EndDate >= DateTime.Now.Date);
        }

        public async Task<List<int>> GetActiveMemberIdsAsync()
        {
            var now = DateTime.Now;
            return await _dbContext.MemberShips
                .Where(ms => ms.EndDate >= now.Date)
                .Select(ms => ms.MemberId)
                .Distinct()
                .ToListAsync();
        }

        public async Task<decimal> GetTotalRevenueAsync()
        {
            return await _dbContext.MemberShips.Include(ms => ms.Plan).SumAsync(ms => ms.Plan.Price);
        }

        public async Task<decimal> GetMonthlyRevenueAsync(int month, int year)
        {
            return await _dbContext.MemberShips
                .Include(ms => ms.Plan)
                .Where(ms => ms.CreatedAt.Month == month && ms.CreatedAt.Year == year)
                .SumAsync(ms => ms.Plan.Price);
        }

        public async Task<Dictionary<string, decimal>> GetRevenueLast6MonthsAsync(int months = 6)
        {
            var result = new Dictionary<string, decimal>();
            var now = DateTime.Now;

            for (int i = months - 1; i >= 0; i--)
            {
                var targetDate = now.AddMonths(-i);
                var revenue = await GetMonthlyRevenueAsync(targetDate.Month, targetDate.Year);
                result.Add(targetDate.ToString("MMM yyyy"), revenue);
            }

            return result;
        }
        public async Task<Dictionary<string, decimal>> GetRevenueByRangeAsync(DateTime startDate, DateTime endDate)
        {
            var result = new Dictionary<string, decimal>();
            var current = new DateTime(startDate.Year, startDate.Month, 1);
            var end = new DateTime(endDate.Year, endDate.Month, 1);

            while (current <= end)
            {
                var revenue = await GetMonthlyRevenueAsync(current.Month, current.Year);
                result.Add(current.ToString("MMM yyyy"), revenue);
                current = current.AddMonths(1);
            }

            return result;
        }
    }
}
