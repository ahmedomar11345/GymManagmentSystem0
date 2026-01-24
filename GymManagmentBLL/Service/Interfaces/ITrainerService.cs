using GymManagmentBLL.ViewModels.TrainerViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface ITrainerService
    {
        bool CreateTrainer(CreateTrainerViewModel createTrainer);
        bool UpdateTrainerDetails(TrainerToUpdateViewModel updatedTrainer, int trainerId);
        bool RemoveTrainer(int trainerId);
        TrainerViewModel? GetTrainerDetails(int trainerId);
        TrainerToUpdateViewModel? GetTrainerToUpdate(int trainerId);
        IEnumerable<TrainerViewModel> GetAllTrainers();
    }
}
