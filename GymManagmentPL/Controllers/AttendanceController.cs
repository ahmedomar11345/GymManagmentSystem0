using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin,Trainer")]
    public class AttendanceController : Controller
    {
        private readonly IAttendanceService _attendanceService;
        private readonly ITrainerService _trainerService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AttendanceController(IAttendanceService attendanceService, 
            ITrainerService trainerService,
            IStringLocalizer<SharedResource> localizer)
        {
            _attendanceService = attendanceService;
            _trainerService = trainerService;
            _localizer = localizer;
        }

        public IActionResult Scanner()
        {
            return View();
        }

        public class ScanRequest
        {
            public string AccessKey { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> ProcessScan([FromBody] ScanRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.AccessKey))
                return Json(new { success = false, message = "Empty Code" });

            var result = await _attendanceService.ProcessScanAsync(request.AccessKey);
            return Json(new { success = result.Success, message = result.Message });
        }

        public async Task<IActionResult> DailyReport(DateTime? date)
        {
            var reportDate = date ?? DateTime.Today;
            var report = await _attendanceService.GetDailyReportAsync(reportDate);
            ViewBag.Date = reportDate;
            return View(report);
        }

        public async Task<IActionResult> TrainerMonthlyReport(int trainerId, int? year, int? month)
        {
            var reportYear = year ?? DateTime.Today.Year;
            var reportMonth = month ?? DateTime.Today.Month;
            
            if (trainerId == 0)
            {
                ViewBag.Trainers = await _trainerService.GetAllTrainersAsync();
                return View();
            }

            var report = await _attendanceService.GetMonthlyReportAsync(trainerId, reportYear, reportMonth);
            var trainer = await _trainerService.GetTrainerDetailsAsync(trainerId);
            
            ViewBag.TrainerName = trainer?.Name;
            ViewBag.SelectedTrainerId = trainerId;
            ViewBag.Year = reportYear;
            ViewBag.Month = reportMonth;
            ViewBag.Trainers = await _trainerService.GetAllTrainersAsync();

            return View(report);
        }

        public async Task<IActionResult> History(DateTime? date)
        {
            var filterDate = date ?? DateTime.Today;
            var unifiedReport = await _attendanceService.GetUnifiedAttendanceAsync(filterDate);
            return View(unifiedReport);
        }

        public async Task<IActionResult> MonthlySummary(int? year, int? month)
        {
            var targetYear = year ?? DateTime.Today.Year;
            var targetMonth = month ?? DateTime.Today.Month;
            
            var summary = await _attendanceService.GetMonthlyMemberAttendanceSummaryAsync(targetYear, targetMonth);
            
            ViewBag.Year = targetYear;
            ViewBag.Month = targetMonth;
            
            return View(summary);
        }

        public async Task<IActionResult> TrainerMonthlySummary(int? year, int? month)
        {
            var targetYear = year ?? DateTime.Today.Year;
            var targetMonth = month ?? DateTime.Today.Month;
            
            var summary = await _attendanceService.GetMonthlyTrainerAttendanceSummaryAsync(targetYear, targetMonth);
            
            ViewBag.Year = targetYear;
            ViewBag.Month = targetMonth;
            
            return View(summary);
        }

        public async Task<IActionResult> ExportMonthlyAttendance(int year, int month)
        {
            var summary = await _attendanceService.GetMonthlyMemberAttendanceSummaryAsync(year, month);
            var csv = new StringBuilder();
            csv.AppendLine("Member ID,Member Name,Plan,Attendance Count,Last Seen");

            foreach (var item in summary)
            {
                csv.AppendLine($"{item.MemberId},{item.MemberName},{item.PlanName},{item.AttendanceCount},{item.LastAttendance}");
            }

            return File(Encoding.UTF8.GetBytes(csv.ToString()), "text/csv", $"Attendance_Report_{year}_{month}.csv");
        }

        public async Task<IActionResult> RecentHistory()
        {
            var history = await _attendanceService.GetRecentCheckInsAsync(10);
            return PartialView("_RecentCheckIns", history);
        }

        public async Task<IActionResult> LastCheckIn()
        {
            var history = await _attendanceService.GetRecentCheckInsAsync(1);
            return PartialView("_RecentCheckIns", history);
        }
    }
}
