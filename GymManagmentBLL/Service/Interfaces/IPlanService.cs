using GymManagmentBLL.ViewModels.PlanViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IPlanService
    {
        Task<IEnumerable<PlanViewModel>> GetAllPlansAsync();
        Task<PlanViewModel?> GetPlanByIdAsync(int id);
        Task<UpdatePlanViewModel?> GetPlanToUpdateAsync(int planId);
        Task<bool> UpdatePlanAsync(int planId, UpdatePlanViewModel PlanToUpdate);
        Task<bool> ToggleStatusAsync(int planId);
        Task<bool> CreatePlanAsync(CreatePlanViewModel model);
    }
}
