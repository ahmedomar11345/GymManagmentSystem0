using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.PlanViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class PlanService : IPlanService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public IEnumerable<PlanViewModel> GetAllPlans()
        {
            var plans = _unitOfWork.GetRepository<Plane>().GetAll();
            if(plans is null || !plans.Any()) return [];
            return _mapper.Map<IEnumerable<PlanViewModel>>(plans);
        }

        public PlanViewModel? GetPlanById(int id)
        {
            var plan = _unitOfWork.GetRepository<Plane>().GetById(id);
            if (plan is null) return null;
            return _mapper.Map<PlanViewModel>(plan);
        }

        public UpdatePlanViewModel? GetPlanToUpdate(int planId)
        {
            var plan = _unitOfWork.GetRepository<Plane>().GetById(planId);
            if (plan is null || HasActiveMemberShips(planId)) return null;
            return _mapper.Map<UpdatePlanViewModel>(plan);

        }

        

        public bool UpdatePlan(int planId, UpdatePlanViewModel PlanToUpdate)
        {
            var plan = _unitOfWork.GetRepository<Plane>().GetById(planId);
            if (plan is null || HasActiveMemberShips(planId)) return false;
            (plan.Name,plan.Description,plan.DurationDays, plan.Price) = 
                (PlanToUpdate.PlanName, PlanToUpdate.Description, PlanToUpdate.DurationDays, PlanToUpdate.Price);
            _unitOfWork.GetRepository<Plane>().Update(plan);
            return _unitOfWork.SaveChanges() > 0;
        }

        public bool ToggleStatus(int planId)
        {
            var repo = _unitOfWork.GetRepository<Plane>();
            var plan = repo.GetById(planId);
            if (plan is null || HasActiveMemberShips(planId)) return false;
            plan.IsActive = plan.IsActive == true ? false : true; 
            plan.UpdatedAt = DateTime.Now;
            try
            {
                repo.Update(plan);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        #region Helper
        private bool HasActiveMemberShips(int planId)
        {
            var memberships = _unitOfWork.GetRepository<MemberShip>().GetAll(m => m.PlanId == planId && m.Status == "Active");
            return memberships.Any();
        }
        #endregion
    }
}
