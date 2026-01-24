using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.SessionViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
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

        public SessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public bool CreateSession(CreateSessionViewModel createSession)
        {
            try
            {
                //check if trainer exists
                if (!IsTrainerExists(createSession.TrainerId)) return false;
                //check if category exists
                if (!IsCategoryExists(createSession.CategoryId)) return false;
                //check if startdate is before end date
                if (!IsDateTimeValid(createSession.StartDate, createSession.EndDate)) return false;
                if (createSession.Capacity > 25 || createSession.Capacity < 0) return false;

                var sessionEntity = _mapper.Map<CreateSessionViewModel, Session>(createSession);
                _unitOfWork.GetRepository<Session>().Add(sessionEntity);

                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Craete Session Faild {ex}");
                return false;
            }
        }

        public IEnumerable<SessionViewModel> GetAllSession()
        {
            var session = _unitOfWork.sessionRepository.GetAllSessionWithTrainerAndCategory();
            if (!session.Any()) return [];

            var MappedSession = _mapper.Map<IEnumerable<Session>,IEnumerable< SessionViewModel >> (session);
            foreach(var Sessions in MappedSession)
            {
                Sessions.AvailableSlot = Sessions.Capacity - _unitOfWork.sessionRepository.GetCountofBookedSlot(Sessions.Id);
            }
            return MappedSession;
        }

        public IEnumerable<TrainerSelectViewModel> GetTrainerForDropDown()
        {
            var trainer = _unitOfWork.GetRepository<Trainer>().GetAll();
            if (!trainer.Any()) return [];
            return _mapper.Map<IEnumerable<TrainerSelectViewModel>>(trainer);
        }

        public IEnumerable<CategorySelectViewModel> GetCategoryForDropDown()
        {
            var category = _unitOfWork.GetRepository<Category>().GetAll();
            if (!category.Any()) return [];
            return _mapper.Map<IEnumerable<CategorySelectViewModel>>(category);
        }

        public SessionViewModel? GetSessionById(int Sessionid)
        {
            var session = _unitOfWork.sessionRepository.GetSessionWithTrainerandCategory(Sessionid);
            if (session == null) return null;
            var MappedSession = _mapper.Map<Session, SessionViewModel>(session);
            MappedSession.AvailableSlot = MappedSession.Capacity - _unitOfWork.sessionRepository.GetCountofBookedSlot(MappedSession.Id);
            return MappedSession;

        }

        public UpdateSessionViewModel? GetSessionToUpdate(int sessionId)
        {
            var session = _unitOfWork.sessionRepository.GetById(sessionId);
            if(!IsSessionAvailableToUpdate(session!)) return null;
            return _mapper.Map<UpdateSessionViewModel>(session);
        }

        public bool UpdateSession(int sessionId, UpdateSessionViewModel updateSession)
        {
            try
            {
                var session = _unitOfWork.sessionRepository.GetById(sessionId);
                if (!IsSessionAvailableToUpdate(session!)) return false;
                // check if trainer exists
                if (!IsTrainerExists(updateSession.TrainerId)) return false;
                // check if startdate is before end date
                if (!IsDateTimeValid(updateSession.StartDate, updateSession.EndDate)) return false;
                // map the updated fields
                _mapper.Map(updateSession, session);
                session!.UpdatedAt = DateTime.Now;
                _unitOfWork.GetRepository<Session>().Update(session);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Update Session Faild {ex}");
                return false;
            }
        }

        public bool RemoveSession(int sessionId)
        {
            try
            {
                var session = _unitOfWork.sessionRepository.GetById(sessionId);
                if (!IsSessionAvailableToRemove(session!)) return false;

                // Use transaction: if session is Completed we remove related bookings first to avoid FK issues.
                var result = _unitOfWork.ExecuteInTransaction(() =>
                {
                    // For upcoming sessions there should be no bookings (checked in IsSessionAvailableToRemove)
                    // For completed sessions: delete historical bookings first to avoid FK constraint
                    var bookings = _unitOfWork.GetRepository<MemberSession>().GetAll()
                        .Where(ms => ms.SessionId == sessionId)
                        .ToList();

                    if (bookings.Any())
                    {
                        foreach (var b in bookings)
                        {
                            _unitOfWork.GetRepository<MemberSession>().Delete(b);
                        }
                    }

                    _unitOfWork.sessionRepository.Delete(session!);
                    return true;
                });

                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($" Remove Session Faild {ex}");
                return false;
            }
        }

        #region Helper
        private bool IsTrainerExists(int TrainerId)
        {
            return _unitOfWork.GetRepository<Trainer>().GetById(TrainerId) is not null;
            
        }

        private bool IsCategoryExists(int CategoryId)
        {
            return _unitOfWork.GetRepository<Category>().GetById(CategoryId) is not null;
        }

        private bool IsDateTimeValid(DateTime startDate,DateTime endDate)
        {
            return startDate < endDate; 
        }

        private bool IsSessionAvailableToUpdate(Session session)
        {
            if(session is null) return false;
            // Can only edit Upcoming sessions (not started yet)
            if (session.StartDate <= DateTime.Now) return false;
            return true;
        }

        private bool IsSessionAvailableToRemove(Session session)
        {
            if(session is null) return false;

            // Ongoing sessions: cannot delete
            if (session.StartDate <= DateTime.Now && session.EndDate > DateTime.Now) return false;

            // Upcoming session: allow only when there are no bookings
            if (session.StartDate > DateTime.Now)
            {
                var hasActiveBooking = _unitOfWork.sessionRepository.GetCountofBookedSlot(session.Id) > 0;
                if (hasActiveBooking) return false;
                return true;
            }

            // Completed session (end date in past): allow deletion (cleanup). Bookings will be removed in transaction.
            if (session.EndDate <= DateTime.Now) return true;

            return false;
        }

        



        #endregion

    }
}
