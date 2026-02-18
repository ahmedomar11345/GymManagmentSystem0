using GymManagmentBLL.ViewModels.MemberViewModel;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IMemberService
    {
        Task<IEnumerable<MemberViewModel>> GetAllMembersAsync();
        Task<PagedResult<MemberViewModel>> GetMembersPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<bool> CreateMemberAsync(CreateMemberViewModel createMember);
        Task<MemberViewModel?> GetMemberDetailsAsync(int memberId);
        Task<HealthRecordViewModel?> GetMemberHealthRecordDetailsAsync(int memberId);
        Task<MemberToUpdateViewModel?> GetMemberToUpdateAsync(int memberId);
        Task<bool> UpdateMemberAsync(int memberId, MemberToUpdateViewModel memberToUpdate);
        Task<bool> RemoveMemberAsync(int memberId);
        Task<bool> RefreshAccessKeyAsync(int memberId);
        
        // Health Progress Tracking
        Task<IEnumerable<HealthProgressViewModel>> GetMemberHealthProgressAsync(int memberId);
        Task<bool> AddHealthProgressAsync(HealthProgressViewModel progress);
    }
}
