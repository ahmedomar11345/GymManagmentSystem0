using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.AnalyticsViewModel;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly IUnitOfWork _unitOfWork;

        public AnalyticsService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<AnalyticsViewModel> GetAnalyticsDataAsync(int months = 6, DateTime? startDate = null, DateTime? endDate = null)
        {
            var now = DateTime.Now;
            var prevMonth = now.AddMonths(-1);

            var totalMembers = await _unitOfWork.GetRepository<Member>().CountAsync();
            var newMembersThisMonth = await _unitOfWork.MemberRepository.GetNewMembersCountAsync(now.Month, now.Year);
            var newMembersLastMonth = await _unitOfWork.MemberRepository.GetNewMembersCountAsync(prevMonth.Month, prevMonth.Year);

            double growthPercentage = 0;
            if (newMembersLastMonth > 0)
            {
                growthPercentage = ((double)(newMembersThisMonth - newMembersLastMonth) / newMembersLastMonth) * 100;
            }
            else if (newMembersThisMonth > 0)
            {
                growthPercentage = 100;
            }

            var vm = new AnalyticsViewModel
            {
                ActiveMembers = await _unitOfWork.MemberShipRepository.GetActiveCountAsync(),
                TotalMembers = totalMembers,
                TotalTrainers = await _unitOfWork.GetRepository<Trainer>().CountAsync(),
                UpcomingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate > now),
                OngoingSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.StartDate <= now && s.EndDate >= now),
                CompletedSessions = await _unitOfWork.GetRepository<Session>().CountAsync(s => s.EndDate < now),
                
                TotalRevenue = await _unitOfWork.MemberShipRepository.GetTotalRevenueAsync(),
                MonthlyRevenue = await _unitOfWork.MemberShipRepository.GetMonthlyRevenueAsync(now.Month, now.Year),
                MemberGrowthPercentage = Math.Round(growthPercentage, 1),
                SelectedRangeMonths = months,
                SelectedStartDate = startDate,
                SelectedEndDate = endDate
            };

            if (startDate.HasValue && endDate.HasValue)
            {
                vm.RevenueLast6Months = await _unitOfWork.MemberShipRepository.GetRevenueByRangeAsync(startDate.Value, endDate.Value);
                vm.MemberRegistrationGrowth = await _unitOfWork.MemberRepository.GetMemberGrowthByRangeAsync(startDate.Value, endDate.Value);
                vm.SelectedRangeLabel = $"{startDate.Value:MMM yyyy} - {endDate.Value:MMM yyyy}";
            }
            else
            {
                vm.RevenueLast6Months = await _unitOfWork.MemberShipRepository.GetRevenueLast6MonthsAsync(months);
                vm.MemberRegistrationGrowth = await _unitOfWork.MemberRepository.GetMemberGrowthLast6MonthsAsync(months);
                vm.SelectedRangeLabel = months == 12 ? "Last Year" : "Last 6 Months";
            }

            // Populate Expiring Soon (Urgent: 7 days)
            var expiringShips = await _unitOfWork.MemberShipRepository.GetExpiringWithinAsync(7);
            foreach (var ship in expiringShips)
            {
                var member = await _unitOfWork.MemberRepository.GetByIdAsync(ship.MemberId);
                var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(ship.PlanId);
                if (member != null && plan != null)
                {
                    vm.ExpiringSoonMembers.Add(new ExpiringMemberViewModel
                    {
                        MemberId = member.Id,
                        MemberName = member.Name,
                        PlanName = plan.Name,
                        EndDate = ship.EndDate,
                        DaysRemaining = (ship.EndDate.Date - DateTime.Now.Date).Days
                    });
                }
            }
            vm.ExpiringSoonMembers = vm.ExpiringSoonMembers.OrderBy(e => e.DaysRemaining).ToList();

            // Populate Recent Activities (Audit Logs) - Optimized with server-side ordering
            var logsPaged = await _unitOfWork.GetRepository<AuditLog>()
                .GetPagedAsync(1, 5, null, q => q.OrderByDescending(l => l.Timestamp));

            foreach (var log in logsPaged.Items)
            {
                vm.RecentActivities.Add(new RecentActivityViewModel
                {
                    Action = log.Action,
                    EntityName = log.EntityName,
                    Timestamp = log.Timestamp,
                    UserEmail = log.UserName
                });
            }

            return vm;
        }
    }
}
