using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Repositories.Interfaces
{
    public interface IMemberRepository : IGenericRepository<Member>
    {
        Member? GetWithDetails(int id);
        Task<Member?> GetWithDetailsAsync(int id);
        IEnumerable<Member> GetAllWithDetails();
        Task<IEnumerable<Member>> GetAllWithDetailsAsync();

        Task<int> GetNewMembersCountAsync(int month, int year);
        Task<Dictionary<string, int>> GetMemberGrowthLast6MonthsAsync(int months = 6);
        Task<Dictionary<string, int>> GetMemberGrowthByRangeAsync(DateTime startDate, DateTime endDate);
    }
}
