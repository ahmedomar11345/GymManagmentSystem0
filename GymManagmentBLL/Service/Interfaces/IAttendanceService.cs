using GymManagmentDAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Interfaces
{
    public interface IAttendanceService
    {
        Task<(bool Success, string Message)> ProcessScanAsync(string accessKey);
        Task<IEnumerable<TrainerAttendance>> GetDailyReportAsync(DateTime date);
        Task<IEnumerable<TrainerAttendance>> GetMonthlyReportAsync(int trainerId, int year, int month);
        Task<IEnumerable<CheckInViewModel>> GetRecentCheckInsAsync(int count = 10);
        Task<IEnumerable<CheckInViewModel>> GetMemberAttendanceHistoryAsync(int memberId);
        Task<IEnumerable<DailyAttendanceViewModel>> GetAttendanceHistoryAsync(DateTime? date = null);
        Task<IEnumerable<MemberAttendanceSummaryViewModel>> GetMonthlyMemberAttendanceSummaryAsync(int year, int month);
        Task<IEnumerable<TrainerMonthlySummaryViewModel>> GetMonthlyTrainerAttendanceSummaryAsync(int year, int month);
        Task<UnifiedAttendanceViewModel> GetUnifiedAttendanceAsync(DateTime date);
    }

    public class CheckInViewModel
    {
        public int Id { get; set; }
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string? MemberPhoto { get; set; }
        public string CheckInTime { get; set; } = null!;
        public string? Notes { get; set; }
    }

    public class DailyAttendanceViewModel
    {
        public DateTime Date { get; set; }
        public string DayName { get; set; } = null!;
        public int Count { get; set; }
        public IEnumerable<CheckInViewModel> CheckIns { get; set; } = new List<CheckInViewModel>();
    }

    public class MemberAttendanceSummaryViewModel
    {
        public int MemberId { get; set; }
        public string MemberName { get; set; } = null!;
        public string? MemberPhoto { get; set; }
        public int AttendanceCount { get; set; }
        public string PlanName { get; set; } = null!;
        public string LastAttendance { get; set; } = null!;
    }

    public class TrainerAttendanceViewModel
    {
        public int Id { get; set; }
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = null!;
        public string? TrainerPhoto { get; set; }
        public string? CheckInTime { get; set; }
        public string? CheckOutTime { get; set; }
        public double TotalHours { get; set; }
        public int DelayMinutes { get; set; }
        public string? Status { get; set; }
        public string? Notes { get; set; }
    }

    public class TrainerMonthlySummaryViewModel
    {
        public int TrainerId { get; set; }
        public string TrainerName { get; set; } = null!;
        public string? TrainerPhoto { get; set; }
        public int AttendanceCount { get; set; }
        public double TotalHours { get; set; }
        public int TotalDelayMinutes { get; set; }
        public string LastAttendance { get; set; } = null!;
    }

    public class UnifiedAttendanceViewModel
    {
        public DateTime SelectedDate { get; set; }
        public string DayName { get; set; } = null!;
        public int TotalMemberCheckIns { get; set; }
        public int TotalTrainerCheckIns { get; set; }
        public IEnumerable<CheckInViewModel> MemberAttendance { get; set; } = new List<CheckInViewModel>();
        public IEnumerable<TrainerAttendanceViewModel> TrainerAttendance { get; set; } = new List<TrainerAttendanceViewModel>();
    }
}
