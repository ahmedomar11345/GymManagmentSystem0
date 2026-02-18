using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface ITrainerSpecialtyService
    {
        Task<IEnumerable<TrainerSpecialty>> GetAllSpecialtiesAsync();
        Task<TrainerSpecialty?> GetSpecialtyByIdAsync(int id);
        Task<bool> CreateSpecialtyAsync(TrainerSpecialty specialty);
        Task<bool> UpdateSpecialtyAsync(TrainerSpecialty specialty);
        Task<bool> DeleteSpecialtyAsync(int id);
    }
}
