using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.PlanViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<PlanService> _logger;

        public PlanService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PlanService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<PlanViewModel>> GetAllPlansAsync()
        {
            var plans = await _unitOfWork.GetRepository<Plane>().GetAllAsync();
            if (plans is null || !plans.Any()) return Enumerable.Empty<PlanViewModel>();
            return _mapper.Map<IEnumerable<PlanViewModel>>(plans);
        }

        public async Task<PlanViewModel?> GetPlanByIdAsync(int id)
        {
            var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(id);
            if (plan is null) return null;
            return _mapper.Map<PlanViewModel>(plan);
        }

        public async Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId)
        {
            var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(planId);
            if (plan is null || await HasActiveMemberShipsAsync(planId)) return null;
            return _mapper.Map<UpdatePlanViewModel>(plan);
        }

        public async Task<bool> UpdatePlanAsync(int planId, UpdatePlanViewModel PlanToUpdate)
        {
            try
            {
                var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(planId);
                if (plan is null || await HasActiveMemberShipsAsync(planId)) return false;

                _mapper.Map(PlanToUpdate, plan);
                _unitOfWork.GetRepository<Plane>().Update(plan);
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result) _logger.LogInformation("Plan updated successfully: {PlanId}", planId);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating plan {PlanId}", planId);
                return false;
            }
        }

        public async Task<bool> ToggleStatusAsync(int planId)
        {
            try
            {
                var repo = _unitOfWork.GetRepository<Plane>();
                var plan = await repo.GetByIdAsync(planId);
                if (plan is null) return false;

                plan.IsActive = !plan.IsActive;
                plan.UpdatedAt = DateTime.Now;

                repo.Update(plan);
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result) _logger.LogInformation("Plan {PlanId} status toggled to {Status}", planId, plan.IsActive ? "Active" : "Inactive");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error toggling status for plan {PlanId}", planId);
                return false;
            }
        }

        public async Task<bool> CreatePlanAsync(CreatePlanViewModel model)
        {
            try
            {
                var plan = _mapper.Map<Plane>(model);
                plan.CreatedAt = DateTime.Now;
                plan.UpdatedAt = DateTime.Now;
                plan.IsActive = true; // Default to active

                await _unitOfWork.GetRepository<Plane>().AddAsync(plan);
                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result) _logger.LogInformation("Plan created successfully: {PlanName}", model.Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating plan {PlanName}", model.Name);
                return false;
            }
        }

        #region Helper
        private async Task<bool> HasActiveMemberShipsAsync(int planId)
        {
            return await _unitOfWork.MemberShipRepository.AnyActiveWithPlanIdAsync(planId);
        }
        #endregion
    }
}

