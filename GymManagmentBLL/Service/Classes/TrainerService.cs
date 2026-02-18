using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.TrainerViewModel;
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
    public class TrainerService : ITrainerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TrainerService> _logger;

        public TrainerService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TrainerService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CreateTrainerAsync(CreateTrainerViewModel createTrainer)
        {
            try
            {
                if (await IsEmailExistsAsync(createTrainer.Email))
                {
                    _logger.LogWarning("Attempt to create trainer with duplicate email: {Email}", createTrainer.Email);
                    return false;
                }
                
                if (await IsPhoneExistsAsync(createTrainer.Phone))
                {
                    _logger.LogWarning("Attempt to create trainer with duplicate phone: {Phone}", createTrainer.Phone);
                    return false;
                }

                var trainerEntity = _mapper.Map<Trainer>(createTrainer);
                await _unitOfWork.GetRepository<Trainer>().AddAsync(trainerEntity);

                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result) _logger.LogInformation("Trainer created successfully: {Name}", createTrainer.Name);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating trainer: {Name}", createTrainer.Name);
                return false;
            }
        }

        public async Task<IEnumerable<TrainerViewModel>> GetAllTrainersAsync()
        {
            var trainers = await _unitOfWork.TrainerRepository.GetAllAsync();
            if (trainers is null || !trainers.Any()) return Enumerable.Empty<TrainerViewModel>();
            return _mapper.Map<IEnumerable<TrainerViewModel>>(trainers);
        }

        public async Task<PagedResult<TrainerViewModel>> GetTrainersPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var pagedResult = await _unitOfWork.TrainerRepository.GetTrainersWithSpecialtyPagedAsync(pageNumber, pageSize, searchTerm);

            return new PagedResult<TrainerViewModel>
            {
                Items = _mapper.Map<IEnumerable<TrainerViewModel>>(pagedResult.Items),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<TrainerViewModel?> GetTrainerDetailsAsync(int trainerId)
        {
            if (trainerId <= 0) return null;
            var trainer = await _unitOfWork.TrainerRepository.GetTrainerWithSpecialtyAsync(trainerId);
            if (trainer is null) return null;
            return _mapper.Map<TrainerViewModel>(trainer);
        }

        public async Task<TrainerToUpdateViewModel?> GetTrainerToUpdateAsync(int trainerId)
        {
            if (trainerId <= 0) return null;
            var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);
            if (trainer is null) return null;
            return _mapper.Map<TrainerToUpdateViewModel>(trainer);
        }

        public async Task<bool> UpdateTrainerDetailsAsync(TrainerToUpdateViewModel updatedTrainer, int trainerId)
        {
            try
            {
                if (trainerId <= 0) return false;

                var emailExists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Email == updatedTrainer.Email && t.Id != trainerId);
                var phoneExists = await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Phone == updatedTrainer.Phone && t.Id != trainerId);

                if (emailExists || phoneExists)
                {
                    _logger.LogWarning("Duplicate email/phone for trainer update {Id}", trainerId);
                    return false;
                }

                var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);
                if (trainer is null) return false;

                _mapper.Map(updatedTrainer, trainer);
                _unitOfWork.GetRepository<Trainer>().Update(trainer);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating trainer {Id}", trainerId);
                return false;
            }
        }

        public async Task<bool> RemoveTrainerAsync(int trainerId)
        {
            try
            {
                if (trainerId <= 0) return false;

                // 1. Check Business Rule: Trainer must not have active/future sessions
                if (await HasActiveSessionsAsync(trainerId))
                {
                    _logger.LogWarning("Deletion blocked: Trainer {Id} is assigned to active or future sessions.", trainerId);
                    return false;
                }

                // 2. Clear past records (Cascade-like logic for EF)
                var trainer = await _unitOfWork.GetRepository<Trainer>().GetByIdAsync(trainerId);
                if (trainer is null) return false;

                var pastSessions = await _unitOfWork.SessionRepository.GetAllAsync(s => s.TrainerId == trainerId);
                if (pastSessions.Any())
                {
                    foreach (var session in pastSessions)
                    {
                        var bookings = await _unitOfWork.MemberSessionRepository.GetAllAsync(ms => ms.SessionId == session.Id);
                        if (bookings.Any()) _unitOfWork.MemberSessionRepository.DeleteRange(bookings);
                    }
                    _unitOfWork.SessionRepository.DeleteRange(pastSessions);
                }

                // 3. Final removal (Manual Soft Delete)
                trainer.IsDeleted = true;
                trainer.UpdatedAt = DateTime.Now;
                _unitOfWork.GetRepository<Trainer>().Update(trainer);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                var innerMsg = ex.InnerException?.Message ?? ex.Message;
                _logger.LogError(ex, "FATAL: Could not remove trainer {Id}. Error: {Msg}", trainerId, innerMsg);
                return false;
            }
        }

        private async Task<bool> IsEmailExistsAsync(string email) => await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Email == email);
        private async Task<bool> IsPhoneExistsAsync(string phone) => await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Phone == phone);
        private async Task<bool> HasActiveSessionsAsync(int trainerId)
        {
            return await _unitOfWork.GetRepository<Session>().ExistsAsync(s => s.TrainerId == trainerId && s.EndDate >= DateTime.Now);
        }
    }
}
