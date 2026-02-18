using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using GymManagmentDAL.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly INotificationService _notificationService;

        public AttendanceService(IUnitOfWork unitOfWork, INotificationService notificationService)
        {
            _unitOfWork = unitOfWork;
            _notificationService = notificationService;
        }

        public async Task<(bool Success, string Message)> ProcessScanAsync(string accessKey)
        {
            // 1. Check if it's a Trainer
            var trainer = await _unitOfWork.TrainerRepository.GetAllAsync(t => t.AccessKey == accessKey);
            var trainerEntity = trainer.FirstOrDefault();
            
            if (trainerEntity != null)
            {
                return await HandleTrainerCheckAsync(trainerEntity);
            }

            // 2. Check if it's a Member
            var member = await _unitOfWork.MemberRepository.GetAllAsync(m => m.AccessKey == accessKey);
            var memberEntity = member.FirstOrDefault();

            if (memberEntity != null)
            {
                return await HandleMemberCheckAsync(memberEntity);
            }

            return (false, "Invalid QR Code / كود غير صالح");
        }

        private async Task<(bool Success, string Message)> HandleTrainerCheckAsync(Trainer trainer)
        {
            var today = DateTime.Today;
            var now = DateTime.Now;

            // Find if there's an open attendance (CheckIn but no CheckOut) for today
            var attendance = (await _unitOfWork.GetRepository<TrainerAttendance>()
                .GetAllAsync(a => a.TrainerId == trainer.Id && a.Date.Date == today && a.CheckOutTime == null))
                .FirstOrDefault();

            if (attendance == null)
            {
                // Action: Check-In
                var newAttendance = new TrainerAttendance
                {
                    TrainerId = trainer.Id,
                    Date = today,
                    CheckInTime = now,
                    DelayMinutes = CalculateDelay(now.TimeOfDay, trainer.ShiftStart)
                };

                _unitOfWork.GetRepository<TrainerAttendance>().Add(newAttendance);
                await _unitOfWork.SaveChangesAsync();

                // Notify Admin
                await _notificationService.CreateNotificationAsync(
                    "Trainer Arrival",
                    $"{trainer.Name} has checked in. Delay: {newAttendance.DelayMinutes} mins.",
                    $"/Trainer/Details/{trainer.Id}",
                    null,
                    "info"
                );

                string msg = $"Check-In Successful: {trainer.Name}. Delay: {newAttendance.DelayMinutes} mins.";
                string msgAr = $"تم تسجيل حضور المدرب: {trainer.Name}. التأخير: {newAttendance.DelayMinutes} دقيقة.";
                return (true, $"{msg} / {msgAr}");
            }
            else
            {
                // Action: Check-Out
                attendance.CheckOutTime = now;
                var duration = now - (attendance.CheckInTime ?? now);
                attendance.TotalHours = Math.Round(duration.TotalHours, 2);
                attendance.OvertimeMinutes = CalculateOvertime(now.TimeOfDay, trainer.ShiftEnd);

                _unitOfWork.GetRepository<TrainerAttendance>().Update(attendance);
                await _unitOfWork.SaveChangesAsync();

                string msg = $"Check-Out Successful: {trainer.Name}. Total Hours: {attendance.TotalHours}";
                string msgAr = $"تم تسجيل انصراف المدرب: {trainer.Name}. إجمالي الساعات: {attendance.TotalHours}";
                return (true, $"{msg} / {msgAr}");
            }
        }

        private async Task<(bool Success, string Message)> HandleMemberCheckAsync(Member member)
        {
            var now = DateTime.Now;

            // 1. Check for active or frozen membership
            var memberships = await _unitOfWork.GetRepository<MemberShip>().GetAllAsync(ms => ms.MemberId == member.Id);
            var activeMembership = memberships.FirstOrDefault(ms => (ms.Status == "Active" || ms.Status == "Frozen") && ms.EndDate >= now);

            if (activeMembership == null)
            {
                return (false, $"Membership Expired/Inactive for {member.Name} / اشتراك منتهي أو غير نشط");
            }

            if (activeMembership.Status == "Frozen")
            {
                 return (false, $"Membership is currently frozen for {member.Name} (Until: {activeMembership.FreezeEndDate:dd/MM/yyyy}) / العضوية مجمدة حالياً حتى {activeMembership.FreezeEndDate:dd/MM/yyyy}");
            }

            // 2. Log General Check-In (Entrance)
            var checkIn = new CheckIn
            {
                MemberId = member.Id,
                CheckInTime = now,
                Notes = "Unified QR Scanner Check-in"
            };
            await _unitOfWork.GetRepository<CheckIn>().AddAsync(checkIn);

            // 3. Mark attendance for today's sessions if any
            var todaySessions = await _unitOfWork.MemberSessionRepository.GetAllAsync(ms => ms.MemberId == member.Id && ms.CreatedAt.Date == DateTime.Today);
            foreach (var session in todaySessions)
            {
                session.IsAttended = true;
                _unitOfWork.MemberSessionRepository.Update(session);
            }

            await _unitOfWork.SaveChangesAsync();

            // 4. Notify Admin
            await _notificationService.CreateNotificationAsync(
                "Member Arrival",
                $"{member.Name} has just checked in.",
                $"/Member/Details/{member.Id}",
                null,
                "info"
            );

            return (true, $"Welcome {member.Name}! Access Granted. / أهلاً {member.Name}! تفضل بالدخول.");
        }

        private int CalculateDelay(TimeSpan checkIn, TimeSpan shiftStart)
        {
            if (checkIn > shiftStart)
            {
                return (int)(checkIn - shiftStart).TotalMinutes;
            }
            return 0;
        }

        private int CalculateOvertime(TimeSpan checkOut, TimeSpan shiftEnd)
        {
            if (checkOut > shiftEnd)
            {
                return (int)(checkOut - shiftEnd).TotalMinutes;
            }
            return 0;
        }

        public async Task<IEnumerable<CheckInViewModel>> GetRecentCheckInsAsync(int count = 10)
        {
            var checkIns = (await _unitOfWork.GetRepository<CheckIn>().GetAllAsync())
                            .OrderByDescending(c => c.CheckInTime)
                            .Take(count);
            
            var result = new List<CheckInViewModel>();
            foreach (var c in checkIns)
            {
                var member = await _unitOfWork.MemberRepository.GetByIdAsync(c.MemberId);
                result.Add(new CheckInViewModel
                {
                    Id = c.Id,
                    MemberId = c.MemberId,
                    MemberName = member?.Name ?? "Unknown",
                    MemberPhoto = member?.Photo,
                    CheckInTime = c.CheckInTime.ToString("yyyy-MM-dd HH:mm:ss"),
                    Notes = c.Notes
                });
            }
            return result;
        }

        public async Task<IEnumerable<CheckInViewModel>> GetMemberAttendanceHistoryAsync(int memberId)
        {
            var checkIns = (await _unitOfWork.GetRepository<CheckIn>().GetAllAsync(c => c.MemberId == memberId))
                            .OrderByDescending(c => c.CheckInTime);

            var member = await _unitOfWork.MemberRepository.GetByIdAsync(memberId);
            return checkIns.Select(c => new CheckInViewModel
            {
                Id = c.Id,
                MemberId = c.MemberId,
                MemberName = member?.Name ?? "Unknown",
                MemberPhoto = member?.Photo,
                CheckInTime = c.CheckInTime.ToString("yyyy-MM-dd HH:mm:ss"),
                Notes = c.Notes
            });
        }

        public async Task<IEnumerable<DailyAttendanceViewModel>> GetAttendanceHistoryAsync(DateTime? date = null)
        {
            DateTime filterDate = date ?? DateTime.Today;
            
            // If the user provided a specific date, we just want that day.
            // If date was null, we might want to show a range starting from today backwards, 
            // but the user specifically said "the default should be today's date" and "be able to select the day I want to see".
            // So if date is provided, we filter EXACTLY for that day. 
            // If date is null, we show today.
            
            var checkIns = await _unitOfWork.GetRepository<CheckIn>().GetAllAsync(c => c.CheckInTime.Date == filterDate.Date);

            var grouped = checkIns.GroupBy(c => c.CheckInTime.Date)
                                  .OrderByDescending(g => g.Key);

            var result = new List<DailyAttendanceViewModel>();
            foreach (var group in grouped)
            {
                var dailyModel = new DailyAttendanceViewModel
                {
                    Date = group.Key,
                    DayName = group.Key.ToString("dddd", new System.Globalization.CultureInfo("ar-EG")),
                    Count = group.Count()
                };

                var list = new List<CheckInViewModel>();
                foreach (var item in group.OrderByDescending(x => x.CheckInTime))
                {
                    var member = await _unitOfWork.MemberRepository.GetByIdAsync(item.MemberId);
                    list.Add(new CheckInViewModel
                    {
                        Id = item.Id,
                        MemberId = item.MemberId,
                        MemberName = member?.Name ?? "Unknown",
                        MemberPhoto = member?.Photo,
                        CheckInTime = item.CheckInTime.ToString("hh:mm tt"),
                        Notes = item.Notes
                    });
                }
                dailyModel.CheckIns = list;
                result.Add(dailyModel);
            }

            // If no data for that day, we still want to return an empty model for that date so the view can show "No records"
            if (!result.Any() && date.HasValue)
            {
                result.Add(new DailyAttendanceViewModel 
                { 
                    Date = filterDate, 
                    DayName = filterDate.ToString("dddd", new System.Globalization.CultureInfo("ar-EG")),
                    Count = 0 
                });
            }

            return result;
        }

        public async Task<IEnumerable<TrainerAttendance>> GetDailyReportAsync(DateTime date)
        {
            return await _unitOfWork.GetRepository<TrainerAttendance>()
                .GetAllAsync(a => a.Date.Date == date.Date);
        }

        public async Task<IEnumerable<TrainerAttendance>> GetMonthlyReportAsync(int trainerId, int year, int month)
        {
            return await _unitOfWork.GetRepository<TrainerAttendance>()
                .GetAllAsync(a => a.TrainerId == trainerId && a.Date.Year == year && a.Date.Month == month);
        }

        public async Task<IEnumerable<MemberAttendanceSummaryViewModel>> GetMonthlyMemberAttendanceSummaryAsync(int year, int month)
        {
            var checkIns = await _unitOfWork.GetRepository<CheckIn>().GetAllAsync(c => c.CheckInTime.Year == year && c.CheckInTime.Month == month);

            var summaries = checkIns.GroupBy(c => c.MemberId)
                                    .Select(async g => {
                                        var member = await _unitOfWork.MemberRepository.GetByIdAsync(g.Key);
                                        var lastCheckIn = g.Max(c => c.CheckInTime);
                                        
                                        // Get plan name (logic might depend on how memberships are stored)
                                        var memberships = await _unitOfWork.GetRepository<MemberShip>()
                                            .GetAllAsync(ms => ms.MemberId == g.Key);
                                        var activeMembership = memberships.FirstOrDefault(ms => ms.Status == "Active");
                                            
                                        var planName = "N/A";
                                        if (activeMembership != null)
                                        {
                                            var plan = await _unitOfWork.GetRepository<Plane>().GetByIdAsync(activeMembership.PlanId);
                                            planName = plan?.Name ?? "N/A";
                                        }

                                        return new MemberAttendanceSummaryViewModel
                                        {
                                            MemberId = g.Key,
                                            MemberName = member?.Name ?? "Unknown",
                                            MemberPhoto = member?.Photo,
                                            AttendanceCount = g.Count(),
                                            PlanName = planName,
                                            LastAttendance = lastCheckIn.ToString("yyyy-MM-dd HH:mm")
                                        };
                                    });

            var result = await Task.WhenAll(summaries);
            return result.OrderByDescending(s => s.AttendanceCount);
        }
        public async Task<UnifiedAttendanceViewModel> GetUnifiedAttendanceAsync(DateTime date)
        {
            var filterDate = date.Date;
            
            // 1. Members
            var memberCheckIns = await _unitOfWork.GetRepository<CheckIn>()
                .GetAllAsync(c => c.CheckInTime.Date == filterDate);
            
            var memberList = new List<CheckInViewModel>();
            foreach (var item in memberCheckIns.OrderByDescending(x => x.CheckInTime))
            {
                var member = await _unitOfWork.MemberRepository.GetByIdAsync(item.MemberId);
                memberList.Add(new CheckInViewModel
                {
                    Id = item.Id,
                    MemberId = item.MemberId,
                    MemberName = member?.Name ?? "Unknown",
                    MemberPhoto = member?.Photo,
                    CheckInTime = item.CheckInTime.ToString("hh:mm tt"),
                    Notes = item.Notes
                });
            }

            // 2. Trainers
            var trainerAttendances = await _unitOfWork.GetRepository<TrainerAttendance>()
                .GetAllAsync(a => a.Date.Date == filterDate);
            
            var trainerList = new List<TrainerAttendanceViewModel>();
            foreach (var item in trainerAttendances.OrderByDescending(x => x.CheckInTime))
            {
                var trainer = await _unitOfWork.TrainerRepository.GetByIdAsync(item.TrainerId);
                trainerList.Add(new TrainerAttendanceViewModel
                {
                    Id = item.Id,
                    TrainerId = item.TrainerId,
                    TrainerName = trainer?.Name ?? "Unknown",
                    TrainerPhoto = trainer?.Photo,
                    CheckInTime = item.CheckInTime?.ToString("hh:mm tt"),
                    CheckOutTime = item.CheckOutTime?.ToString("hh:mm tt"),
                    TotalHours = item.TotalHours,
                    DelayMinutes = item.DelayMinutes,
                    Status = item.CheckOutTime == null ? "In" : "Out",
                    Notes = item.Notes
                });
            }

            return new UnifiedAttendanceViewModel
            {
                SelectedDate = filterDate,
                DayName = filterDate.ToString("dddd", new System.Globalization.CultureInfo("ar-EG")),
                TotalMemberCheckIns = memberList.Count,
                TotalTrainerCheckIns = trainerList.Count,
                MemberAttendance = memberList,
                TrainerAttendance = trainerList
            };
        }
        public async Task<IEnumerable<TrainerMonthlySummaryViewModel>> GetMonthlyTrainerAttendanceSummaryAsync(int year, int month)
        {
            var trainerAttendances = await _unitOfWork.GetRepository<TrainerAttendance>()
                .GetAllAsync(a => a.Date.Year == year && a.Date.Month == month);

            var grouped = trainerAttendances.GroupBy(a => a.TrainerId);
            var summaries = new List<TrainerMonthlySummaryViewModel>();

            foreach (var group in grouped)
            {
                var trainer = await _unitOfWork.TrainerRepository.GetByIdAsync(group.Key);
                if (trainer == null) continue;

                var lastEntry = group.OrderByDescending(x => x.Date).FirstOrDefault();

                summaries.Add(new TrainerMonthlySummaryViewModel
                {
                    TrainerId = trainer.Id,
                    TrainerName = trainer.Name,
                    TrainerPhoto = trainer.Photo,
                    AttendanceCount = group.Count(),
                    TotalHours = group.Sum(x => x.TotalHours),
                    TotalDelayMinutes = group.Sum(x => x.DelayMinutes),
                    LastAttendance = lastEntry?.Date.ToString("yyyy-MM-dd") ?? "N/A"
                });
            }

            return summaries.OrderByDescending(s => s.AttendanceCount);
        }
    }
}
