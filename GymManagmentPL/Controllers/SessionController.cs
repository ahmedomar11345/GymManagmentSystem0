using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.ViewModels.SessionViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize]
    public class SessionController : Controller
    {
        private readonly ISessionService _sessionService;
        private readonly ITrainerService _trainerService;
        private readonly IMemberService _memberService;
        private readonly IMemberSessionService _memberSessionService;
        private readonly IEmailService _emailService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly ILogger<SessionController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SessionController(
            ISessionService sessionService, 
            ITrainerService trainerService, 
            IMemberService memberService,
            IMemberSessionService memberSessionService,
            IEmailService emailService, 
            IGymSettingsService gymSettingsService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<SessionController> logger)
        {
            _sessionService = sessionService;
            _trainerService = trainerService;
            _memberService = memberService;
            _memberSessionService = memberSessionService;
            _emailService = emailService;
            _gymSettingsService = gymSettingsService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var pagedResult = await _sessionService.GetSessionsPagedAsync(pageNumber, pageSize, searchTerm);
            ViewData["SearchTerm"] = searchTerm;
            return View(pagedResult);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();
            
            var session = await _sessionService.GetSessionByIdAsync(id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(session);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Create()
        {
            await LoadDropDownsAsync();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateSessionViewModel createSession)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                await LoadDropDownsAsync();
                return View(createSession);
            }

            var result = await _sessionService.CreateSessionAsync(createSession);
            if (result)
            {
                // Notify Trainer
                try
                {
                    var trainer = await _trainerService.GetTrainerDetailsAsync(createSession.TrainerId);
                    if (trainer != null && !string.IsNullOrEmpty(trainer.Email))
                    {
                        bool isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        string assignmentTemplate = EmailTemplates.SessionAssignment(trainer.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, createSession.Description ?? "Gym Session", createSession.StartDate, isArabic);
                        await _emailService.SendEmailAsync(trainer.Email, isArabic ? "تكليف بجلسة تدريبية جديدة" : "New Training Session Assigned", assignmentTemplate);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send assignment email to Trainer for session {Session}", createSession.Description);
                }

                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["InvalidInput"].Value;
            await LoadDropDownsAsync();
            return View(createSession);
        }

        [Authorize(Roles = "SuperAdmin,Admin")]
        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var session = await _sessionService.GetSessionToUpdateAsync(id);
            if (session == null)
            {
                TempData["ErrorMessage"] = "Session not edit-able (already started) or not found.";
                return RedirectToAction(nameof(Index));
            }

            await LoadDropDownsAsync();
            return View(session);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdateSessionViewModel updateSession)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                await LoadDropDownsAsync();
                return View(updateSession);
            }

            var result = await _sessionService.UpdateSessionAsync(id, updateSession);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            await LoadDropDownsAsync();
            return View(updateSession);
        }

        [HttpPost]
        [Authorize(Roles = "SuperAdmin,Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            // Capture details BEFORE deletion for notifications
            var session = await _sessionService.GetSessionByIdAsync(id);
            var bookedData = await _memberSessionService.GetMembersForUpcomingSessionAsync(id);

            var result = await _sessionService.RemoveSessionAsync(id);
            if (result)
            {
                if (session != null)
                {
                    try
                    {
                        var now = DateTime.Now;
                        var isUpcoming = session.StartDate > now;

                        // Only notify if the session was upcoming (not completed)
                        if (isUpcoming)
                        {
                            // Notify Trainer
                            var trainer = await _trainerService.GetTrainerDetailsAsync(session.TrainerId);
                            if (trainer != null && !string.IsNullOrEmpty(trainer.Email))
                            {
                                bool isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                                var gymSettings = await _gymSettingsService.GetSettingsAsync();
                                await _emailService.SendEmailAsync(trainer.Email, isArabic ? "عاجل: إلغاء جلسة تدريبية" : "Urgent: Training Session Cancelled", 
                                    EmailTemplates.SessionCancelled(trainer.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, session.Description, session.StartDate, isArabic));
                            }

                            // Notify Members
                            if (bookedData != null && bookedData.Bookings.Any())
                            {
                                foreach (var booking in bookedData.Bookings)
                                {
                                    var member = await _memberService.GetMemberDetailsAsync(booking.MemberId);
                                    if (member != null && !string.IsNullOrEmpty(member.Email))
                                    {
                                        bool isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                                        await _emailService.SendEmailAsync(member.Email, isArabic ? "إشعار بإلغاء جلسة تدريبية" : "Training Session Cancellation Notice", 
                                            EmailTemplates.SessionCancelled(member.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, session.Description, session.StartDate, isArabic));
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error sending cancellation emails for session {Id}", id);
                    }
                }

                TempData["SuccessMessage"] = "Session deleted and participants notified.";
            }
            else
            {
                TempData["ErrorMessage"] = "Cannot delete session. It may have started or is ongoing.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Members(int id)
        {
            if (id <= 0) return BadRequest();

            var members = await _memberSessionService.GetSessionMembersAsync(id);
            if (members == null)
            {
                TempData["ErrorMessage"] = "Session not found.";
                return RedirectToAction(nameof(Index));
            }

            return View(members);
        }

        private async Task LoadDropDownsAsync()
        {
            var categories = await _sessionService.GetCategoryForDropDownAsync();
            var localizedCategories = categories.Select(c => new { 
                Id = c.Id, 
                Name = _localizer[c.Name].Value 
            });

            ViewBag.Categories = new SelectList(localizedCategories, "Id", "Name");
            ViewBag.Trainers = new SelectList(await _sessionService.GetTrainerForDropDownAsync(), "Id", "Name");
        }
    }
}
