using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberShipViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberShipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<MemberShipViewModel> GetAllActiveMemberShips()
        {
            var allMemberShips = _unitOfWork.GetRepository<MemberShip>().GetAll();

            if (allMemberShips is null || !allMemberShips.Any()) return [];

            // Filter active memberships (EndDate > Now)
            var activeMemberShips = allMemberShips.Where(m => m.EndDate > DateTime.Now).ToList();

            if (!activeMemberShips.Any()) return [];

            var viewModels = new List<MemberShipViewModel>();

            foreach (var memberShip in activeMemberShips)
            {
                var member = _unitOfWork.GetRepository<Member>().GetById(memberShip.MemberId);
                var plan = _unitOfWork.GetRepository<Plane>().GetById(memberShip.PlanId);

                viewModels.Add(new MemberShipViewModel
                {
                    MemberId = memberShip.MemberId,
                    MemberName = member?.Name ?? "Unknown",
                    PlanId = memberShip.PlanId,
                    PlanName = plan?.Name ?? "Unknown",
                    StartDate = memberShip.CreatedAt.ToString("MMM dd, yyyy"),
                    EndDate = memberShip.EndDate.ToString("MMM dd, yyyy"),
                    Status = memberShip.Status
                });
            }

            return viewModels;
        }

        public bool CreateMemberShip(CreateMemberShipViewModel createMemberShip)
        {
            try
            {
                // Rule 1: Member must exist
                var member = _unitOfWork.GetRepository<Member>().GetById(createMemberShip.MemberId);
                if (member is null) return false;

                // Rule 2: Plan must exist
                var plan = _unitOfWork.GetRepository<Plane>().GetById(createMemberShip.PlanId);
                if (plan is null) return false;

                // Rule 4: Only active plans can be assigned
                if (!plan.IsActive) return false;

                // Rule 3: Member cannot have more than one Active membership
                var hasActiveMembership = _unitOfWork.GetRepository<MemberShip>()
                    .GetAll()
                    .Any(m => m.MemberId == createMemberShip.MemberId && m.EndDate > DateTime.Now);
                if (hasActiveMembership) return false;

                // Rule 5: EndDate is calculated based on plan duration
                var memberShip = new MemberShip
                {
                    MemberId = createMemberShip.MemberId,
                    PlanId = createMemberShip.PlanId,
                    CreatedAt = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(plan.DurationDays)
                };

                _unitOfWork.GetRepository<MemberShip>().Add(memberShip);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool CancelMemberShip(int memberId, int planId)
        {
            try
            {
                var memberShipRepo = _unitOfWork.GetRepository<MemberShip>();
                
                // Find by composite key (MemberId + PlanId)
                var memberShip = memberShipRepo.GetAll()
                    .FirstOrDefault(m => m.MemberId == memberId && m.PlanId == planId);

                if (memberShip is null) return false;

                // Rule 8: Can only delete if Active (EndDate > Now)
                if (memberShip.EndDate <= DateTime.Now) return false;

                // Rule 7: Delete membership
                memberShipRepo.Delete(memberShip);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<MemberSelectViewModel> GetMembersForDropDown()
        {
            var members = _unitOfWork.GetRepository<Member>().GetAll();
            if (members is null || !members.Any()) return [];

            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public IEnumerable<PlanSelectViewModel> GetActivePlansForDropDown()
        {
            // Rule 4: Only active plans
            var plans = _unitOfWork.GetRepository<Plane>().GetAll().Where(p => p.IsActive);
            if (plans is null || !plans.Any()) return [];

            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(plans);
        }
    }
}
