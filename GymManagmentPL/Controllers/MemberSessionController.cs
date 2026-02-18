using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.ViewModels.MemberSessionViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize]
    public class MemberSessionController : Controller
    {
        private readonly IMemberSessionService _memberSessionService;
        private readonly IMemberService _memberService;
        private readonly IEmailService _emailService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MemberSessionController(
            IMemberSessionService memberSessionService, 
            IMemberService memberService, 
            IEmailService emailService, 
            IGymSettingsService gymSettingsService,
            IStringLocalizer<SharedResource> localizer)
        {
            _memberSessionService = memberSessionService;
            _memberService = memberService;
            _emailService = emailService;
            _gymSettingsService = gymSettingsService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var upcomingSessions = await _memberSessionService.GetUpcomingSessionsAsync();
            var ongoingSessions = await _memberSessionService.GetOngoingSessionsAsync();

            ViewBag.UpcomingSessions = upcomingSessions;
            ViewBag.OngoingSessions = ongoingSessions;

            return View();
        }

        public async Task<IActionResult> GetMembersForUpcomingSession(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Index));
            }

            var sessionMembers = await _memberSessionService.GetMembersForUpcomingSessionAsync(id);
            if (sessionMembers is null)
            {
                TempData["ErrorMessage"] = "Session not found or is not upcoming.";
                return RedirectToAction(nameof(Index));
            }

            return View(sessionMembers);
        }

        public async Task<IActionResult> GetMembersForOngoingSession(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Index));
            }

            var sessionMembers = await _memberSessionService.GetMembersForOngoingSessionAsync(id);
            if (sessionMembers is null)
            {
                TempData["ErrorMessage"] = "Session not found or is not ongoing.";
                return RedirectToAction(nameof(Index));
            }

            return View(sessionMembers);
        }

        public async Task<IActionResult> Create(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid session ID.";
                return RedirectToAction(nameof(Index));
            }

            var sessions = await _memberSessionService.GetUpcomingSessionsAsync();
            var session = sessions.FirstOrDefault(s => s.Id == id) ?? (await _memberSessionService.GetOngoingSessionsAsync()).FirstOrDefault(s => s.Id == id);

            if (session == null || !session.CanBook)
            {
                TempData["ErrorMessage"] = "Enrollment is no longer available for this session (it may have ended or reached full capacity).";
                return RedirectToAction(nameof(Index));
            }

            await LoadMembersDropDownAsync(id);
            ViewBag.SessionId = id;
            ViewBag.SessionName = session.Description;

            return View(new CreateBookingViewModel { SessionId = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateBookingViewModel createBooking)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;

                var sessions = await _memberSessionService.GetUpcomingSessionsAsync();
                var session = sessions.FirstOrDefault(s => s.Id == createBooking.SessionId) ?? (await _memberSessionService.GetOngoingSessionsAsync()).FirstOrDefault(s => s.Id == createBooking.SessionId);
                
                await LoadMembersDropDownAsync(createBooking.SessionId);
                ViewBag.SessionId = createBooking.SessionId;
                ViewBag.SessionName = session?.Description ?? "Session";
                
                return View(createBooking);
            }

            var result = await _memberSessionService.CreateBookingAsync(createBooking);
            if (result.Success)
            {
                // Send session confirmation email
                try
                {
                    var member = await _memberService.GetMemberDetailsAsync(createBooking.MemberId);
                    var sessionName = await _memberSessionService.GetSessionNameAsync(createBooking.SessionId);
                    
                    if (member != null && !string.IsNullOrEmpty(member.Email))
                    {
                        bool isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string sessionTemplate = EmailTemplates.BookingConfirmation(member.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, sessionName ?? "Gym Session", DateTime.Now, isArabic);
                        await _emailService.SendEmailAsync(member.Email, isArabic ? "تأكيد حجز الجلسة" : "Session Booking Confirmation", sessionTemplate);
                    }
                }
                catch (Exception) { /* Log error if needed */ }

                TempData["SuccessMessage"] = _localizer["BookingSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value; // Ideally we'd map result.Message too, but localizer is safer for now or we trust result.Message
                
                var sessionName = await _memberSessionService.GetSessionNameAsync(createBooking.SessionId);
                if (sessionName != null) ViewBag.SessionName = sessionName;
                await LoadMembersDropDownAsync(createBooking.SessionId);
                
                return View(createBooking);
            }

            // Determine where to redirect based on session status
            var ongoingList = await _memberSessionService.GetOngoingSessionsAsync();
            if (ongoingList.Any(s => s.Id == createBooking.SessionId))
            {
                return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = createBooking.SessionId });
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = createBooking.SessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int memberId, int sessionId)
        {
            var result = await _memberSessionService.CancelBookingAsync(memberId, sessionId);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["CancelSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }

            // Determine where to redirect based on session status
            var ongoingList = await _memberSessionService.GetOngoingSessionsAsync();
            if (ongoingList.Any(s => s.Id == sessionId))
            {
                return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
            }

            return RedirectToAction(nameof(GetMembersForUpcomingSession), new { id = sessionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAttendance(int memberId, int sessionId)
        {
            var result = await _memberSessionService.MarkAttendanceAsync(memberId, sessionId);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }

            return RedirectToAction(nameof(GetMembersForOngoingSession), new { id = sessionId });
        }

        private async Task LoadMembersDropDownAsync(int sessionId)
        {
            var members = await _memberSessionService.GetMembersWithActiveMembershipForDropDownAsync(sessionId);
            ViewBag.Members = new SelectList(members, "Id", "Name");
        }
    }
}
