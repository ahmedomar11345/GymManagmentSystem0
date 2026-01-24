using GymManagmentBLL.ViewModels.MemberShipViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IMemberShipService
    {
        IEnumerable<MemberShipViewModel> GetAllActiveMemberShips();
        bool CreateMemberShip(CreateMemberShipViewModel createMemberShip);
        bool CancelMemberShip(int memberId, int planId);
        IEnumerable<MemberSelectViewModel> GetMembersForDropDown();
        IEnumerable<PlanSelectViewModel> GetActivePlansForDropDown();
    }
}
