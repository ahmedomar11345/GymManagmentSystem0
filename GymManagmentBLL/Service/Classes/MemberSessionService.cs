using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberSessionViewModel;
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
    public class MemberSessionService : IMemberSessionService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<MemberSessionService> _logger;

        public MemberSessionService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<MemberSessionService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<SessionForBookingViewModel>> GetUpcomingSessionsAsync()
        {
            var sessions = await _unitOfWork.SessionRepository.GetUpcomingSessionsAsync();
            return await MapSessionsToViewModelAsync(sessions, "Upcoming");
        }

        public async Task<IEnumerable<SessionForBookingViewModel>> GetOngoingSessionsAsync()
        {
            var sessions = await _unitOfWork.SessionRepository.GetOngoingSessionsAsync();
            return await MapSessionsToViewModelAsync(sessions, "Ongoing");
        }

        public async Task<SessionMembersViewModel?> GetMembersForUpcomingSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetWithTrainerAndCategoryAsync(sessionId);
            if (session == null || session.EndDate <= DateTime.Now) return null;

            return await FetchSessionWithMembersAsync(session);
        }

        public async Task<SessionMembersViewModel?> GetMembersForOngoingSessionAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetWithTrainerAndCategoryAsync(sessionId);
            if (session == null || session.EndDate <= DateTime.Now) return null;

            return await FetchSessionWithMembersAsync(session);
        }

        public async Task<(bool Success, string Message)> CreateBookingAsync(CreateBookingViewModel createBooking)
        {
            try
            {
                var now = DateTime.Now;
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(createBooking.SessionId);
                
                if (session == null) 
                {
                    return (false, "Session not found.");
                }

                // 1. Time Check
                if (now >= session.EndDate) 
                {
                    return (false, "This session has already ended.");
                }

                // 2. Active Membership Check
                var hasActivePlan = await _unitOfWork.MemberShipRepository.HasActiveMembershipAsync(createBooking.MemberId);
                if (!hasActivePlan) 
                {
                    return (false, "Member does not have an active membership plan.");
                }

                if (await _unitOfWork.MemberSessionRepository.IsAlreadyBookedAsync(createBooking.MemberId, createBooking.SessionId)) 
                {
                    return (false, "Member is already enrolled in this session.");
                }

                var bookedCount = await _unitOfWork.MemberSessionRepository.GetBookingCountAsync(createBooking.SessionId);
                
                var booking = _mapper.Map<MemberSession>(createBooking);
                
                if (bookedCount >= session.Capacity) 
                {
                    booking.Status = GymManagmentDAL.Entities.Enums.BookingStatus.Waitlisted;
                }
                else
                {
                    booking.Status = GymManagmentDAL.Entities.Enums.BookingStatus.Confirmed;
                }

                await _unitOfWork.MemberSessionRepository.AddAsync(booking);
                
                var success = await _unitOfWork.SaveChangesAsync() > 0;
                if (success) 
                {
                    return (true, booking.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Waitlisted 
                        ? "Session is full. You have been added to the waitlist." 
                        : "Member enrolled successfully.");
                }

                return (false, "Could not save booking to database. Please try again.");
            }
            catch (Exception ex)
            {
                // Catching specific DB update exceptions if they occur
                if (ex.InnerException?.Message.Contains("duplicate") == true || ex.Message.Contains("duplicate") == true)
                {
                    return (false, "This member is already booked for this session.");
                }

                _logger.LogError(ex, "CRITICAL: Database error during booking for member {MemberId} in session {SessionId}.", createBooking.MemberId, createBooking.SessionId);
                return (false, $"System Error: {ex.Message}");
            }
        }

        public async Task<bool> CancelBookingAsync(int memberId, int sessionId)
        {
            try
            {
                var booking = await _unitOfWork.MemberSessionRepository.GetByCompositeKeyAsync(memberId, sessionId);
                if (booking == null) return false;

                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                if (session == null || session.EndDate <= DateTime.Now) return false;

                var wasConfirmed = booking.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Confirmed;
                _unitOfWork.MemberSessionRepository.Delete(booking);
                
                var success = await _unitOfWork.SaveChangesAsync() > 0;

                // Auto-promote from Waitlist if a confirmed spot was opened
                if (success && wasConfirmed)
                {
                    var nextInWaitlist = (await _unitOfWork.MemberSessionRepository
                        .GetAllAsync(ms => ms.SessionId == sessionId && ms.Status == GymManagmentDAL.Entities.Enums.BookingStatus.Waitlisted))
                        .OrderBy(ms => ms.CreatedAt)
                        .FirstOrDefault();

                    if (nextInWaitlist != null)
                    {
                        nextInWaitlist.Status = GymManagmentDAL.Entities.Enums.BookingStatus.Confirmed;
                        _unitOfWork.MemberSessionRepository.Update(nextInWaitlist);
                        await _unitOfWork.SaveChangesAsync();
                        
                        // TODO: Send Email Notification to nextInWaitlist.Member
                    }
                }

                return success;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to cancel booking for member {MemberId} in session {SessionId}", memberId, sessionId);
                return false;
            }
        }

        public async Task<bool> MarkAttendanceAsync(int memberId, int sessionId)
        {
            try
            {
                var booking = await _unitOfWork.MemberSessionRepository.GetByCompositeKeyAsync(memberId, sessionId);
                if (booking == null || booking.IsAttended || booking.Status != GymManagmentDAL.Entities.Enums.BookingStatus.Confirmed) return false;

                var now = DateTime.Now;
                var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
                if (session == null || session.StartDate > now || session.EndDate <= now) return false;

                booking.IsAttended = true;
                booking.UpdatedAt = now;

                _unitOfWork.MemberSessionRepository.Update(booking);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to mark attendance for member {MemberId} in session {SessionId}", memberId, sessionId);
                return false;
            }
        }

        public async Task<IEnumerable<MemberForBookingSelectViewModel>> GetMembersWithActiveMembershipForDropDownAsync(int sessionId)
        {
            try
            {
                var now = DateTime.Now;

                // 1. Get IDs of members with ANY active membership (inclusive of expiry date)
                // Optimized: Fetch only IDs instead of full entities
                var activeIds = await _unitOfWork.MemberShipRepository.GetActiveMemberIdsAsync();

                if (!activeIds.Any()) 
                {
                    _logger.LogInformation("No members with active memberships found for session booking {Id}", sessionId);
                    return Enumerable.Empty<MemberForBookingSelectViewModel>();
                }

                // 2. Get IDs of members already booked for this session
                var bookedIds = (await _unitOfWork.MemberSessionRepository.GetAllAsync(ms => ms.SessionId == sessionId))
                               .Select(ms => ms.MemberId).ToList();

                // 3. Filter members who are active AND not already booked
                var eligibleIds = activeIds.Except(bookedIds).ToList();

                if (!eligibleIds.Any()) return Enumerable.Empty<MemberForBookingSelectViewModel>();

                // 4. Get the full member objects for the dropdown
                var members = await _unitOfWork.MemberRepository.GetAllAsync(m => eligibleIds.Contains(m.Id));

                return _mapper.Map<IEnumerable<MemberForBookingSelectViewModel>>(members.OrderBy(m => m.Name));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error populating member dropdown for session {Id}", sessionId);
                return Enumerable.Empty<MemberForBookingSelectViewModel>();
            }
        }

        public async Task<string?> GetSessionNameAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetByIdAsync(sessionId);
            if (session == null) return null;
            var cat = await _unitOfWork.GetRepository<Category>().GetByIdAsync(session.CategoryId);
            return $"{cat?.CategoryName} - {session.Description}";
        }

        public async Task<SessionMembersViewModel?> GetSessionMembersAsync(int sessionId)
        {
            var session = await _unitOfWork.SessionRepository.GetWithTrainerAndCategoryAsync(sessionId);
            if (session == null) return null;

            return await FetchSessionWithMembersAsync(session);
        }

        #region Helper Methods
        private async Task<SessionMembersViewModel> FetchSessionWithMembersAsync(Session session)
        {
            var bookings = await _unitOfWork.MemberSessionRepository.GetSessionBookingsWithMembersAsync(session.Id);
            
            var bookingVms = bookings.Select(b => new BookingViewModel
            {
                MemberId = b.MemberId,
                MemberName = b.Member?.Name ?? "Unknown",
                IsAttended = b.IsAttended,
                BookingDate = b.CreatedAt.ToString("g"),
                Status = b.Status.ToString()
            }).ToList();

            return new SessionMembersViewModel
            {
                SessionId = session.Id,
                SessionName = session.SessionCategory?.CategoryName.ToString() ?? "Unknown",
                Bookings = bookingVms
            };
        }

        private async Task<IEnumerable<SessionForBookingViewModel>> MapSessionsToViewModelAsync(IEnumerable<Session> sessions, string status)
        {
            var result = new List<SessionForBookingViewModel>();
            var now = DateTime.Now;
            foreach (var s in sessions)
            {
                var duration = s.EndDate - s.StartDate;
                var midpoint = s.StartDate.Add(duration / 2);
                var bookedCount = await _unitOfWork.MemberSessionRepository.GetBookingCountAsync(s.Id);
                var waitlistCount = await _unitOfWork.MemberSessionRepository.GetWaitlistCountAsync(s.Id);

                // Re-calculate business rule for CanBook flag
                bool isFinished = now >= s.EndDate;
                bool canBookTimeWise = !isFinished; 

                result.Add(new SessionForBookingViewModel
                {
                    Id = s.Id,
                    CategoryName = s.SessionCategory?.CategoryName.ToString() ?? "Unknown",
                    Description = s.Description,
                    TrainerName = s.SessionTrainer?.Name ?? "Unknown",
                    Date = s.StartDate.ToString("MMM dd, yyyy"),
                    StartTime = s.StartDate.ToString("hh:mm tt"),
                    EndTime = s.EndDate.ToString("hh:mm tt"),
                    Duration = duration.ToString(@"hh\:mm"),
                    Capacity = s.Capacity,
                    BookedCount = bookedCount,
                    WaitlistCount = waitlistCount,
                    Status = status,
                    CanBook = canBookTimeWise 
                });
            }
            return result;
        }
        #endregion
    }
}
