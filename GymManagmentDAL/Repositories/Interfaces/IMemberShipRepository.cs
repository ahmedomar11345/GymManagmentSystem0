using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface IMemberShipRepository : IGenericRepository<MemberShip>
    {
        IEnumerable<MemberShip> GetAllActiveWithDetails();
        Task<IEnumerable<MemberShip>> GetAllActiveWithDetailsAsync();
        
        PagedResult<MemberShip> GetPagedActiveWithDetails(int pageNumber, int pageSize, string? searchTerm = null);
        Task<PagedResult<MemberShip>> GetPagedActiveWithDetailsAsync(int pageNumber, int pageSize, string? searchTerm = null);
        
        MemberShip? GetByCompositeKey(int memberId, int planId);
        Task<MemberShip?> GetByCompositeKeyAsync(int memberId, int planId);
        
        bool HasActiveMembership(int memberId);
        Task<bool> HasActiveMembershipAsync(int memberId);
        Task<bool> AnyActiveWithPlanIdAsync(int planId);
        
        IEnumerable<MemberShip> GetExpiringWithin(int days);
        Task<IEnumerable<MemberShip>> GetExpiringWithinAsync(int days);
        
        int GetActiveCount();
        Task<int> GetActiveCountAsync();
        Task<List<int>> GetActiveMemberIdsAsync();
        
        Task<decimal> GetTotalRevenueAsync();
        Task<decimal> GetMonthlyRevenueAsync(int month, int year);
        Task<Dictionary<string, decimal>> GetRevenueLast6MonthsAsync(int months = 6);
        Task<Dictionary<string, decimal>> GetRevenueByRangeAsync(DateTime startDate, DateTime endDate);
    }
}
