using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberSessionViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Reposotories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class MemberSessionService : IMemberSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public MemberSessionService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<SessionForBookingViewModel> GetUpcomingSessions()
        {
            var sessions = _unitOfWork.GetRepository<Session>().GetAll()
                .Where(s => s.StartDate > DateTime.Now)
                .ToList();

            if (!sessions.Any()) return [];

            return MapSessionsToViewModel(sessions, "Upcoming");
        }

        public IEnumerable<SessionForBookingViewModel> GetOngoingSessions()
        {
            var sessions = _unitOfWork.GetRepository<Session>().GetAll()
                .Where(s => s.StartDate <= DateTime.Now && s.EndDate > DateTime.Now)
                .ToList();

            if (!sessions.Any()) return [];

            return MapSessionsToViewModel(sessions, "Ongoing");
        }

        public SessionMembersViewModel? GetMembersForUpcomingSession(int sessionId)
        {
            var session = _unitOfWork.GetRepository<Session>().GetById(sessionId);
            if (session is null) return null;

            // Rule: Must be upcoming session
            if (session.StartDate <= DateTime.Now) return null;

            return GetSessionMembersViewModel(session, sessionId);
        }

        public SessionMembersViewModel? GetMembersForOngoingSession(int sessionId)
        {
            var session = _unitOfWork.GetRepository<Session>().GetById(sessionId);
            if (session is null) return null;

            // Rule 6: Must be ongoing session (started but not ended)
            if (session.StartDate > DateTime.Now || session.EndDate <= DateTime.Now) return null;

            return GetSessionMembersViewModel(session, sessionId);
        }

        public bool CreateBooking(CreateBookingViewModel createBooking)
        {
            try
            {
                // Rule 8: Session must exist
                var session = _unitOfWork.GetRepository<Session>().GetById(createBooking.SessionId);
                if (session is null) return false;

                // Rule 8: Member must exist
                var member = _unitOfWork.GetRepository<Member>().GetById(createBooking.MemberId);
                if (member is null) return false;

                // Rule 4: Can only book future sessions
                if (session.StartDate <= DateTime.Now) return false;

                // Rule 1: Member must have active membership
                var hasActiveMembership = _unitOfWork.GetRepository<MemberShip>().GetAll()
                    .Any(m => m.MemberId == createBooking.MemberId && m.EndDate > DateTime.Now);
                if (!hasActiveMembership) return false;

                // Rule 3: Member cannot book same session twice
                var alreadyBooked = _unitOfWork.GetRepository<MemberSession>().GetAll()
                    .Any(ms => ms.MemberId == createBooking.MemberId && ms.SessionId == createBooking.SessionId);
                if (alreadyBooked) return false;

                // Use transactional operation to avoid race on capacity
                bool result = _unitOfWork.ExecuteInTransaction(() =>
                {
                    // Rule 2: Session must have available capacity (re-check inside transaction)
                    var bookedCount = _unitOfWork.sessionRepository.GetCountofBookedSlot(createBooking.SessionId);
                    if (bookedCount >= session.Capacity) return false;

                    var booking = _mapper.Map<MemberSession>(createBooking);
                    _unitOfWork.GetRepository<MemberSession>().Add(booking);
                    return true; // indicate operation should commit
                });

                return result;
            }
            catch
            {
                return false;
            }
        }

        public bool CancelBooking(int memberId, int sessionId)
        {
            try
            {
                // Rule 8: Booking must exist
                var booking = _unitOfWork.GetRepository<MemberSession>().GetAll()
                    .FirstOrDefault(ms => ms.MemberId == memberId && ms.SessionId == sessionId);
                if (booking is null) return false;

                // Rule 8: Session must exist
                var session = _unitOfWork.GetRepository<Session>().GetById(sessionId);
                if (session is null) return false;

                // Rule 5: Can only cancel future sessions (not started yet)
                if (session.StartDate <= DateTime.Now) return false;

                _unitOfWork.GetRepository<MemberSession>().Delete(booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public bool MarkAttendance(int memberId, int sessionId)
        {
            try
            {
                // Rule 8: Session must exist
                var session = _unitOfWork.GetRepository<Session>().GetById(sessionId);
                if (session is null) return false;

                // Rule 6: Can only mark attendance for ongoing sessions
                if (session.StartDate > DateTime.Now || session.EndDate <= DateTime.Now) return false;

                // Rule 8: Booking must exist
                var booking = _unitOfWork.GetRepository<MemberSession>().GetAll()
                    .FirstOrDefault(ms => ms.MemberId == memberId && ms.SessionId == sessionId);
                if (booking is null) return false;

                booking.IsAttended = true;
                booking.UpdatedAt = DateTime.Now;

                _unitOfWork.GetRepository<MemberSession>().Update(booking);
                return _unitOfWork.SaveChanges() > 0;
            }
            catch
            {
                return false;
            }
        }

        public IEnumerable<MemberForBookingSelectViewModel> GetMembersWithActiveMembershipForDropDown(int sessionId)
        {
            // Get all members with active membership (Rule 1)
            var membersWithActiveMembership = _unitOfWork.GetRepository<MemberShip>().GetAll()
                .Where(m => m.EndDate > DateTime.Now)
                .Select(m => m.MemberId)
                .Distinct()
                .ToList();

            if (!membersWithActiveMembership.Any()) return [];

            // Get members who already booked this session (Rule 3)
            var alreadyBookedMemberIds = _unitOfWork.GetRepository<MemberSession>().GetAll()
                .Where(ms => ms.SessionId == sessionId)
                .Select(ms => ms.MemberId)
                .ToList();

            // Get members with active membership who haven't booked yet
            var availableMembers = _unitOfWork.GetRepository<Member>().GetAll()
                .Where(m => membersWithActiveMembership.Contains(m.Id) && !alreadyBookedMemberIds.Contains(m.Id))
                .ToList();

            return _mapper.Map<IEnumerable<MemberForBookingSelectViewModel>>(availableMembers);
        }

        #region Helper Methods
        private SessionMembersViewModel GetSessionMembersViewModel(Session session, int sessionId)
        {
            var category = _unitOfWork.GetRepository<Category>().GetById(session.CategoryId);

            var bookings = _unitOfWork.GetRepository<MemberSession>().GetAll()
                .Where(ms => ms.SessionId == sessionId)
                .ToList();

            var bookingViewModels = _mapper.Map<List<BookingViewModel>>(bookings);

            // Fill MemberName after mapping
            foreach (var bookingVm in bookingViewModels)
            {
                var member = _unitOfWork.GetRepository<Member>().GetById(bookingVm.MemberId);
                bookingVm.MemberName = member?.Name ?? "Unknown";
            }

            return new SessionMembersViewModel
            {
                SessionId = sessionId,
                SessionName = category?.CategoryName.ToString() ?? "Unknown",
                Bookings = bookingViewModels
            };
        }

        private IEnumerable<SessionForBookingViewModel> MapSessionsToViewModel(List<Session> sessions, string status)
        {
            var viewModels = new List<SessionForBookingViewModel>();

            foreach (var session in sessions)
            {
                var category = _unitOfWork.GetRepository<Category>().GetById(session.CategoryId);
                var trainer = _unitOfWork.GetRepository<Trainer>().GetById(session.TrainerId);
                var bookedCount = _unitOfWork.GetRepository<MemberSession>().GetAll()
                    .Count(ms => ms.SessionId == session.Id);

                var duration = session.EndDate - session.StartDate;

                viewModels.Add(new SessionForBookingViewModel
                {
                    Id = session.Id,
                    CategoryName = category?.CategoryName.ToString() ?? "Unknown",
                    Description = session.Description,
                    TrainerName = trainer?.Name ?? "Unknown",
                    Date = session.StartDate.ToString("MMM dd, yyyy"),
                    StartTime = session.StartDate.ToString("hh:mm tt"),
                    EndTime = session.EndDate.ToString("hh:mm tt"),
                    Duration = $"{duration.Hours:D2}:{duration.Minutes:D2}:{duration.Seconds:D2}",
                    Capacity = session.Capacity,
                    BookedCount = bookedCount,
                    Status = status
                });
            }

            return viewModels;
        }
        #endregion
    }
}
