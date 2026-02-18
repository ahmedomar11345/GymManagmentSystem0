using GymManagmentBLL.ViewModels.TrainerViewModel;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface ITrainerService
    {
        Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync();
        Task<PagedResult<TrainerViewModel>> GetTrainersPagedAsync(int pageNumber, int pageSize, string? searchTerm = null);
        Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer);
        Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId);
        Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId);
        Task<bool> UpdateTrainerDetailsAsync(TrainerToUpdateViewModel updatedTrainer, int trainerId);
        Task<bool> RemoveTrainerAsync(int trainerId);
    }
}
