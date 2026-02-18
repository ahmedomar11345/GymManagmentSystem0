using System;
using System.Collections.Generic;

namespace GymManagmentBLL.ViewModels.ReportViewModel
{
    public class FinancialReportViewModel
    {
        public decimal TotalRevenue { get; set; }
        public decimal TotalExpenses { get; set; } // We might need to assume a percentage or add an entity for expenses later
        public decimal NetProfit => TotalRevenue - TotalExpenses;

        public List<MonthlyRevenueViewModel> MonthlyRevenue { get; set; } = new();
        public List<PlanPopularityViewModel> TopPlans { get; set; } = new();
        
        public int ActiveMembersCount { get; set; }
        public int ExpiredMembersCount { get; set; }
        public int FrozenMembersCount { get; set; }

        public DateTime ReportGeneratedAt { get; set; } = DateTime.Now;
        public string ReportPeriod { get; set; } = "Yearly";
    }

    public class MonthlyRevenueViewModel
    {
        public string MonthName { get; set; } = null!;
        public decimal Revenue { get; set; }
        public int SubscriptionsCount { get; set; }
    }

    public class PlanPopularityViewModel
    {
        public string PlanName { get; set; } = null!;
        public int SubscriberCount { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
