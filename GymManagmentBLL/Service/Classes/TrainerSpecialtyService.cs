using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class TrainerSpecialtyService : ITrainerSpecialtyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public TrainerSpecialtyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<IEnumerable<TrainerSpecialty>> GetAllSpecialtiesAsync()
        {
            return await _unitOfWork.TrainerSpecialtyRepository.GetAllWithTrainersAsync();
        }

        public async Task<TrainerSpecialty?> GetSpecialtyByIdAsync(int id)
        {
            return await _unitOfWork.GetRepository<TrainerSpecialty>().GetByIdAsync(id);
        }

        public async Task<bool> CreateSpecialtyAsync(TrainerSpecialty specialty)
        {
            _unitOfWork.GetRepository<TrainerSpecialty>().Add(specialty);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateSpecialtyAsync(TrainerSpecialty specialty)
        {
            var existing = await _unitOfWork.GetRepository<TrainerSpecialty>().GetByIdAsync(specialty.Id);
            if (existing == null) return false;

            existing.Name = specialty.Name;
            existing.Description = specialty.Description;
            existing.UpdatedAt = DateTime.Now;

            _unitOfWork.GetRepository<TrainerSpecialty>().Update(existing);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteSpecialtyAsync(int id)
        {
            var specialty = await _unitOfWork.GetRepository<TrainerSpecialty>().GetByIdAsync(id);
            if (specialty == null) return false;

            // Find trainers with this specialty and set it to null
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync(t => t.SpecialtyId == id);
            foreach (var trainer in trainers)
            {
                trainer.SpecialtyId = null;
                _unitOfWork.GetRepository<Trainer>().Update(trainer);
            }

            _unitOfWork.GetRepository<TrainerSpecialty>().Delete(specialty);
            return await _unitOfWork.SaveChangesAsync() > 0;
        }
    }
}
