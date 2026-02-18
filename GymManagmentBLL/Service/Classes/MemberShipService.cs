using AutoMapper;
using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.MemberShipViewModel;
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
    public class MemberShipService : IMemberShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;
        private readonly ILogger<MemberShipService> _logger;

        public MemberShipService(IUnitOfWork unitOfWork, IMapper mapper, INotificationService notificationService, ILogger<MemberShipService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _notificationService = notificationService;
            _logger = logger;
        }

        public async Task<IEnumerable<MemberShipViewModel>> GetAllActiveMemberShipsAsync()
        {
            var activeMemberShips = await _unitOfWork.MemberShipRepository.GetAllActiveWithDetailsAsync();
            if (!activeMemberShips.Any()) return Enumerable.Empty<MemberShipViewModel>();

            return activeMemberShips.Select(ms => new MemberShipViewModel
            {
                MemberId = ms.MemberId,
                MemberName = ms.Member?.Name ?? "Unknown",
                MemberPhoto = ms.Member?.Photo,
                PlanId = ms.PlanId,
                PlanName = ms.Plan?.Name ?? "Unknown",
                StartDate = ms.CreatedAt.ToString("MMM dd, yyyy"),
                EndDate = ms.EndDate.ToString("MMM dd, yyyy"),
                Status = ms.Status,
                DaysRemaining = (int)(ms.EndDate - DateTime.Now).TotalDays
            });
        }

        public async Task<PagedResult<MemberShipViewModel>> GetActiveMemberShipsPagedAsync(int pageNumber, int pageSize, string? searchTerm = null)
        {
            var pagedResult = await _unitOfWork.MemberShipRepository.GetPagedActiveWithDetailsAsync(pageNumber, pageSize, searchTerm);
            return new PagedResult<MemberShipViewModel>
            {
                Items = pagedResult.Items.Select(ms => new MemberShipViewModel
                {
                    MemberId = ms.MemberId,
                    MemberName = ms.Member?.Name ?? "Unknown",
                    MemberPhoto = ms.Member?.Photo,
                    PlanId = ms.PlanId,
                    PlanName = ms.Plan?.Name ?? "Unknown",
                    StartDate = ms.CreatedAt.ToString("MMM dd, yyyy"),
                    EndDate = ms.EndDate.ToString("MMM dd, yyyy"),
                    Status = ms.Status,
                    DaysRemaining = (int)(ms.EndDate - DateTime.Now).TotalDays
                }),
                TotalCount = pagedResult.TotalCount,
                PageNumber = pagedResult.PageNumber,
                PageSize = pagedResult.PageSize
            };
        }

        public async Task<IEnumerable<MemberShipViewModel>> GetExpiringMemberShipsAsync(int withinDays = 7)
        {
            var expiringMemberShips = await _unitOfWork.MemberShipRepository.GetExpiringWithinAsync(withinDays);
            return expiringMemberShips.Select(ms => new MemberShipViewModel
            {
                MemberId = ms.MemberId,
                MemberName = ms.Member?.Name ?? "Unknown",
                PlanId = ms.PlanId,
                PlanName = ms.Plan?.Name ?? "Unknown",
                StartDate = ms.CreatedAt.ToString("MMM dd, yyyy"),
                EndDate = ms.EndDate.ToString("MMM dd, yyyy"),
                Status = "Expiring Soon",
                DaysRemaining = (int)(ms.EndDate - DateTime.Now).TotalDays
            });
        }

        public async Task<bool> CreateMemberShipAsync(CreateMemberShipViewModel createMemberShip)
        {
            try
            {
                var member = await _unitOfWork.MemberRepository.GetByIdAsync(createMemberShip.MemberId);
                if (member is null) return false;

                var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(createMemberShip.PlanId);
                if (plan is null || !plan.IsActive) return false;

                if (await _unitOfWork.MemberShipRepository.HasActiveMembershipAsync(createMemberShip.MemberId)) return false;

                var memberShip = new MemberShip
                {
                    MemberId = createMemberShip.MemberId,
                    PlanId = createMemberShip.PlanId,
                    CreatedAt = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(plan.DurationDays)
                };

                await _unitOfWork.MemberShipRepository.AddAsync(memberShip);
                var isSaved = await _unitOfWork.SaveChangesAsync() > 0;

                if (isSaved)
                {
                    await _notificationService.CreateNotificationAsync(
                        "New Membership",
                        $"A new {plan.Name} membership has been created for {member.Name}.",
                        $"/Member/Details/{member.Id}",
                        null,
                        "success"
                    );
                }

                return isSaved;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating membership for member {MemberId}", createMemberShip.MemberId);
                return false;
            }
        }

        public async Task<bool> CancelMemberShipAsync(int memberId, int planId)
        {
            try
            {
                var memberShip = await _unitOfWork.MemberShipRepository.GetByCompositeKeyAsync(memberId, planId);
                if (memberShip is null || memberShip.EndDate < DateTime.Now.Date) return false;

                // Business Rule: Prevent cancellation if member has future bookings
                var hasFutureBookings = await _unitOfWork.MemberSessionRepository.ExistsAsync(ms => 
                    ms.MemberId == memberId && ms.Session.StartDate > DateTime.Now);

                if (hasFutureBookings)
                {
                    _logger.LogWarning("Cancellation blocked: Member {Id} has future session bookings.", memberId);
                    return false;
                }

                _unitOfWork.MemberShipRepository.Delete(memberShip);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling membership for member {MemberId}", memberId);
                return false;
            }
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersForDropDownAsync()
        {
            var members = await _unitOfWork.MemberRepository.GetAllAsync();
            if (members is null || !members.Any()) return Enumerable.Empty<MemberSelectViewModel>();
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(members);
        }

        public async Task<IEnumerable<MemberSelectViewModel>> GetMembersWithoutActiveMembershipAsync()
        {
            var now = DateTime.Now;
            var memberships = await _unitOfWork.MemberShipRepository.GetAllAsync(ms => ms.EndDate > now);
            var membersWithActive = memberships.Select(ms => ms.MemberId).Distinct().ToHashSet();
            var availableMembers = await _unitOfWork.MemberRepository.GetAllAsync(m => !membersWithActive.Contains(m.Id));
            return _mapper.Map<IEnumerable<MemberSelectViewModel>>(availableMembers);
        }

        public async Task<IEnumerable<PlanSelectViewModel>> GetActivePlansForDropDownAsync()
        {
            var plans = await _unitOfWork.GetRepository<Plane>().GetAllAsync(p => p.IsActive);
            if (plans is null || !plans.Any()) return Enumerable.Empty<PlanSelectViewModel>();
            return _mapper.Map<IEnumerable<PlanSelectViewModel>>(plans);
        }

        public async Task<bool> FreezeMemberShipAsync(int memberId, int planId, int durationDays)
        {
            try
            {
                var membership = await _unitOfWork.MemberShipRepository.GetByCompositeKeyAsync(memberId, planId);
                if (membership == null || membership.IsFrozen || membership.EndDate < DateTime.Now)
                    return false;

                membership.IsFrozen = true;
                membership.FreezeStartDate = DateTime.Now;
                membership.FreezeEndDate = DateTime.Now.AddDays(durationDays);
                membership.EndDate = membership.EndDate.AddDays(durationDays);

                _unitOfWork.MemberShipRepository.Update(membership);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error freezing membership for member {MemberId}", memberId);
                return false;
            }
        }

        public async Task<bool> UnfreezeMemberShipAsync(int memberId, int planId)
        {
            try
            {
                var membership = await _unitOfWork.MemberShipRepository.GetByCompositeKeyAsync(memberId, planId);
                if (membership == null || !membership.IsFrozen)
                    return false;

                // If unfreezing early, we might want to subtract the remaining freeze days from EndDate
                if (membership.FreezeEndDate.HasValue && membership.FreezeEndDate.Value > DateTime.Now)
                {
                    var remainingFreezeDays = (int)(membership.FreezeEndDate.Value - DateTime.Now).TotalDays;
                    if (remainingFreezeDays > 0)
                    {
                        membership.EndDate = membership.EndDate.AddDays(-remainingFreezeDays);
                    }
                }

                membership.IsFrozen = false;
                membership.FreezeStartDate = null;
                membership.FreezeEndDate = null;

                _unitOfWork.MemberShipRepository.Update(membership);
                return await _unitOfWork.SaveChangesAsync() > 0;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error unfreezing membership for member {MemberId}", memberId);
                return false;
            }
        }

        public async Task<MemberShipViewModel?> GetMemberShipDetailsAsync(int memberId, int planId)
        {
            var membership = await _unitOfWork.MemberShipRepository.GetByCompositeKeyAsync(memberId, planId);
            if (membership == null) return null;

            return new MemberShipViewModel
            {
                MemberId = membership.MemberId,
                MemberName = membership.Member?.Name ?? "Unknown",
                MemberPhoto = membership.Member?.Photo,
                PlanId = membership.PlanId,
                PlanName = membership.Plan?.Name ?? "Unknown",
                StartDate = membership.CreatedAt.ToString("MMM dd, yyyy"),
                EndDate = membership.EndDate.ToString("MMM dd, yyyy"),
                Status = membership.Status,
                DaysRemaining = (int)(membership.EndDate - DateTime.Now).TotalDays
            };
        }
    }
}
