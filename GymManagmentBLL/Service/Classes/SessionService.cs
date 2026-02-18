using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.SessionViewModel;
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
    public class SessionService : ISessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<SessionService> _logger;

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<SessionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<bool> CreateSessionAsync(CreateSessionViewModel createSession)
        {
            try
            {
                if (createSession.StartDate >= createSession.EndDate) return false;
                if (createSession.Capacity > 25 || createSession.Capacity <= 0) return false;

                if (!await _unitOfWork.GetRepository<Trainer>().ExistsAsync(t => t.Id == createSession.TrainerId)) return false;
                if (!await _unitOfWork.GetRepository<Category>().ExistsAsync(c => c.Id == createSession.CategoryId)) return false;

                // 1. Initial Session
                var sessionEntity = _mapper.Map<Session>(createSession);
                await _unitOfWork.SessionRepository.AddAsync(sessionEntity);

                // 2. Handle Recurrence
                if (createSession.RecurrenceType != GymManagmentDAL.Entities.Enums.RecurrenceType.None && createSession.RecurrenceCount > 1)
                {
                    for (int i = 1; i < createSession.RecurrenceCount; i++)
                    {
                        var recurringSession = _mapper.Map<Session>(createSession);
                        TimeSpan offset;

                        switch (createSession.RecurrenceType)
                        {
                            case GymManagmentDAL.Entities.Enums.RecurrenceType.Daily:
                                offset = TimeSpan.FromDays(i);
                                break;
                            case GymManagmentDAL.Entities.Enums.RecurrenceType.Weekly:
                                offset = TimeSpan.FromDays(i * 7);
                                break;
                            case GymManagmentDAL.Entities.Enums.RecurrenceType.Monthly:
                                // Approximation for simplicity, or use AddMonths
                                recurringSession.StartDate = createSession.StartDate.AddMonths(i);
                                recurringSession.EndDate = createSession.EndDate.AddMonths(i);
                                await _unitOfWork.SessionRepository.AddAsync(recurringSession);
                                continue;
                            default:
                                continue;
                        }

                        recurringSession.StartDate = createSession.StartDate.Add(offset);
                        recurringSession.EndDate = createSession.EndDate.Add(offset);
                        await _unitOfWork.SessionRepository.AddAsync(recurringSession);
                    }
                }

                var result = await _unitOfWork.SaveChangesAsync() > 0;
                if (result) _logger.LogInformation("Session(s) created successfully.");
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create session(s)");
                return false;
            }
        }

        public async Task<IEnumerable<SessionViewModel>> GetAllSessionAsync()
        {
            var sessions = _unitOfWork.SessionRepository.GetAllWithTrainerAndCategory();
            var mappedSessions = _mapper.Map<IEnumerable<SessionViewModel>>(sessions).ToList();
            
            foreach (var vm in mappedSessions)
            {
                vm.AvailableSlot = vm.Capacity - _unitOfWork.SessionRepository.GetBookedSlotCount(vm.Id);
            }
            
            return mappedSessions;
        }

        public async Task<PagedResult<SessionViewModel>> GetSessionsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var pagedResult = await _unitOfWork.SessionRepository.GetPagedWithDetailsAsync(pageNumber, pageSize, searchTerm);
            var viewModels = _mapper.Map<IEnumerable<SessionViewModel>>(pagedResult.Items).ToList();
            
            foreach (var vm in viewModels)
            {
                vm.AvailableSlot = vm.Capacity - _unitOfWork.SessionRepository.GetBookedSlotCount(vm.Id);
            }

            return new PagedResult<SessionViewModel>
            {
                Items = viewModels,
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<SessionViewModel?> GetSessionByIdAsync(int sessionId)
        {
            var session = _unitOfWork.SessionRepository.GetWithTrainerAndCategory(sessionId);
            if (session == null) return null;

            var viewModel = _mapper.Map<SessionViewModel>(session);
            viewModel.AvailableSlot = viewModel.Capacity - _unitOfWork.SessionRepository.GetBookedSlotCount(sessionId);
            return viewModel;
        }

        public async Task<UpdateSessionViewModel?> GetSessionToUpdateAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null || session.StartDate <= DateTime.Now) return null;
            return _mapper.Map<UpdateSessionViewModel>(session);
        }

        public async Task<bool> UpdateSessionAsync(int sessionId, UpdateSessionViewModel updateSession)
        {
            try
            {
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                if (session == null || session.StartDate <= DateTime.Now) return false;
                if (updateSession.StartDate >= updateSession.EndDate) return false;

                _mapper.Map(updateSession, session);
                _unitOfWork.SessionRepository.Update(session);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update session {Id}", sessionId);
                return false;
            }
        }

        public async Task<bool> RemoveSessionAsync(int sessionId)
        {
            try
            {
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                if (session == null) return false;

                // Allow deletion if session is upcoming or already finished. 
                // Only block if it's currently LIVE.
                if (session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) 
                {
                    _logger.LogWarning("Cannot delete live session {Id}", sessionId);
                    return false;
                }

                _unitOfWork.SessionRepository.Delete(session);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove session {Id}", sessionId);
                return false;
            }
        }

        public async Task<IEnumerable<TrainerSelectViewModel>> GetTrainerForDropDownAsync()
        {
            var trainers = await _unitOfWork.GetRepository<Trainer>().GetAllAsync();
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainers);
        }

        public async Task<IEnumerable<CategorySelectViewModel>> GetCategoryForDropDownAsync()
        {
            var categories = await _unitOfWork.GetRepository<Category>().GetAllAsync();
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(categories);
        }

        public async Task<int> CleanupOldSessionsAsync(int daysOld)
        {
            try
            {
                var cutoffDate = DateTime.Now.AddDays(-daysOld);
                var oldSessions = await _unitOfWork.SessionRepository.GetAllAsync(s => s.EndDate < cutoffDate);
                
                if (!oldSessions.Any()) return 0;

                int count = 0;
                foreach (var session in oldSessions)
                {
                    // Optionally: we might want to keep the record but delete associations
                    // But the user asked to "تحذف automatic" (delete automatic).
                    _unitOfWork.SessionRepository.Delete(session);
                    count++;
                }

                await _unitOfWork.SaveChangesAsync();
                _logger.LogInformation("Cleaned up {Count} sessions older than {Days} days.", count, daysOld);
                return count;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during session cleanup.");
                return 0;
            }
        }
    }
}
