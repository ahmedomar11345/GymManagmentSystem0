using GymManagmentBLL.ViewModels.MemberShipViewModel;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IMemberShipService
    {
        Task<IEnumerable<MemberShipViewModel>> GetAllActiveMemberShipsAsync();
        Task<PagedResult<MemberShipViewModel>> GetActiveMemberShipsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<IEnumerable<MemberShipViewModel>> GetExpiringMemberShipsAsync(int withinDays = 7);
        Task<bool> CreateMemberShipAsync(CreateMemberShipViewModel createMemberShip);
        Task<bool> CancelMemberShipAsync(int memberId, int planId);
        Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync();
        Task<IEnumerable<MemberSelectViewModel>> GetMembersWithoutActiveMembershipAsync();
        Task<IEnumerable<PlanSelectViewModel>> GetActivePlansForDropDownAsync();
        Task<bool> FreezeMemberShipAsync(int memberId, int planId, int durationDays);
        Task<bool> UnfreezeMemberShipAsync(int memberId, int planId);
        Task<MemberShipViewModel?> GetMemberShipDetailsAsync(int memberId, int planId);
    }
}
