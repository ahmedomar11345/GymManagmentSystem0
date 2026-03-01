using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.ViewModels.AnalyticsViewModel
{
    public class AnalyticsViewModel
    {
        public int TotalMembers { get; set; }
        public int ActiveMembers { get; set; }
        public int TotalTrainers { get; set; }
        public int UpcomingSessions { get; set; }
        public int OngoingSessions { get; set; }
        public int CompletedSessions { get; set; }
        
        public decimal TotalRevenue { get; set; }
        public decimal MonthlyRevenue { get; set; }
        public double MemberGrowthPercentage { get; set; }
        public Dictionary<string, decimal> RevenueLast6Months { get; set; } = new();
        public Dictionary<string, int> MemberRegistrationGrowth { get; set; } = new();
        public int SelectedRangeMonths { get; set; } = 6;
        public string SelectedRangeLabel { get; set; } = "Last 6 Months";
        public DateTime? SelectedStartDate { get; set; }
        public DateTime? SelectedEndDate { get; set; }
        
        public List<ExpiringMemberViewModel> ExpiringSoonMembers { get; set; } = new();
        public List<RecentActivityViewModel> RecentActivities { get; set; } = new();
    }

    public class ExpiringMemberViewModel
    {
        public int MemberId { get; set; }
        public required string MemberName { get; set; }
        public required string PlanName { get; set; }
        public DateTime EndDate { get; set; }
        public int DaysRemaining { get; set; }
    }

    public class RecentActivityViewModel
    {
        public required string Action { get; set; }
        public required string EntityName { get; set; }
        public DateTime Timestamp { get; set; }
        public required string UserEmail { get; set; }
    }
}
