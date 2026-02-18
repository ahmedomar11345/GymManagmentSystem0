using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.ViewModels.MemberShipViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Localization;
using ClosedXML.Excel;
using System.IO;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MemberShipController : Controller
    {
        private readonly IMemberShipService _memberShipService;
        private readonly IMemberService _memberService;
        private readonly IPlanService _planService;
        private readonly IEmailService _emailService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly ILogger<MemberShipController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public MemberShipController(
            IMemberShipService memberShipService, 
            IMemberService memberService,
            IPlanService planService,
            IEmailService emailService,
            IGymSettingsService gymSettingsService,
            IStringLocalizer<SharedResource> localizer,
            ILogger<MemberShipController> logger)
        {
            _memberShipService = memberShipService;
            _memberService = memberService;
            _planService = planService;
            _emailService = emailService;
            _gymSettingsService = gymSettingsService;
            _localizer = localizer;
            _logger = logger;
        }

        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var memberships = await _memberShipService.GetAllActiveMemberShipsAsync();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("ActiveMemberships");
                    var currentRow = 1;

                    // Headers
                    worksheet.Cell(currentRow, 1).Value = "Member Name";
                    worksheet.Cell(currentRow, 2).Value = "Plan Name";
                    worksheet.Cell(currentRow, 3).Value = "Start Date";
                    worksheet.Cell(currentRow, 4).Value = "End Date";
                    worksheet.Cell(currentRow, 5).Value = "Status";
                    worksheet.Cell(currentRow, 6).Value = "Days Remaining";

                    // Styling Headers
                    var headerRange = worksheet.Range(1, 1, 1, 6);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.BabyBlue;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Data
                    foreach (var ms in memberships)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = ms.MemberName;
                        worksheet.Cell(currentRow, 2).Value = ms.PlanName;
                        worksheet.Cell(currentRow, 3).Value = ms.StartDate;
                        worksheet.Cell(currentRow, 4).Value = ms.EndDate;
                        worksheet.Cell(currentRow, 5).Value = ms.Status;
                        worksheet.Cell(currentRow, 6).Value = ms.DaysRemaining;
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"ActiveMemberships_{DateTime.Now:yyyyMMdd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting memberships to Excel");
                TempData["ErrorMessage"] = "Failed to export data.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var pagedResult = await _memberShipService.GetActiveMemberShipsPagedAsync(pageNumber, pageSize, searchTerm);
            var expiringCount = (await _memberShipService.GetExpiringMemberShipsAsync(7)).Count();
            ViewData["ExpiringCount"] = expiringCount;
            ViewData["SearchTerm"] = searchTerm;
            return View(pagedResult);
        }

        public async Task<IActionResult> Expiring(int days = 7)
        {
            var expiring = await _memberShipService.GetExpiringMemberShipsAsync(days);
            ViewData["Days"] = days;
            return View(expiring);
        }

        public async Task<IActionResult> Create()
        {
            await LoadDropDownsAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberShipViewModel createMemberShip)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                await LoadDropDownsAsync();
                return View(createMemberShip);
            }

            var result = await _memberShipService.CreateMemberShipAsync(createMemberShip);
            if (result)
            {
                // Send Receipt Email
                try
                {
                    var member = await _memberService.GetMemberDetailsAsync(createMemberShip.MemberId);
                    var plan = await _planService.GetPlanByIdAsync(createMemberShip.PlanId);

                    if (member != null && !string.IsNullOrEmpty(member.Email) && plan != null)
                    {
                        bool isArabic = createMemberShip.SendInArabic;
                        var endDate = DateTime.Now.AddDays(plan.DurationDays);
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();

                        string receiptTemplate = EmailTemplates.MembershipReceipt(
                            member.Name, 
                            plan.Name, 
                            plan.Price, 
                            endDate, 
                            plan.DurationDays,
                            gymSettings.GymName, 
                            gymSettings.Phone, 
                            gymSettings.Address, 
                            gymSettings.Email, 
                            isArabic);

                        await _emailService.SendEmailAsync(member.Email, isArabic ? "تفعيل الاشتراك - إيصال الدفع" : "Membership Activated - Payment Receipt", receiptTemplate);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send membership receipt to member {Id}", createMemberShip.MemberId);
                }

                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            await LoadDropDownsAsync();
            return View(createMemberShip);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int memberId, int planId)
        {
            var result = await _memberShipService.CancelMemberShipAsync(memberId, planId);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["CancelSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Freeze(int memberId, int planId, int durationDays)
        {
            if (durationDays <= 0)
            {
                TempData["ErrorMessage"] = _localizer["DurationRange", 1, 30].Value; // Simple fallback or add key
                return RedirectToAction(nameof(Index));
            }

            var result = await _memberShipService.FreezeMemberShipAsync(memberId, planId, durationDays);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unfreeze(int memberId, int planId)
        {
            var result = await _memberShipService.UnfreezeMemberShipAsync(memberId, planId);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Receipt(int memberId, int planId)
        {
            var membership = await _memberShipService.GetMemberShipDetailsAsync(memberId, planId);
            if (membership == null)
            {
                return NotFound();
            }

            // Get plan details for price and duration
            var plan = await _planService.GetPlanByIdAsync(planId);
            ViewBag.Plan = plan;

            // Get member details for contact info
            var member = await _memberService.GetMemberDetailsAsync(memberId);
            ViewBag.Member = member;

            return View(membership);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendReceiptEmail(int memberId, int planId)
        {
            try
            {
                var member = await _memberService.GetMemberDetailsAsync(memberId);
                var plan = await _planService.GetPlanByIdAsync(planId);

                if (member != null && !string.IsNullOrEmpty(member.Email) && plan != null)
                {
                    bool isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                    var membership = await _memberShipService.GetMemberShipDetailsAsync(memberId, planId);
                    
                    if (membership != null)
                    {
                        var endDate = DateTime.Parse(membership.EndDate);
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();

                        string receiptTemplate = EmailTemplates.MembershipReceipt(
                            member.Name, 
                            plan.Name, 
                            plan.Price, 
                            endDate, 
                            plan.DurationDays,
                            gymSettings.GymName, 
                            gymSettings.Phone, 
                            gymSettings.Address, 
                            gymSettings.Email, 
                            isArabic);

                        await _emailService.SendEmailAsync(member.Email, isArabic ? "إيصال دفع اشتراك النادي الرياضي" : "Gym Membership Payment Receipt", receiptTemplate);
                        
                        TempData["SuccessMessage"] = isArabic ? "تم إرسال الفاتورة إلى البريد الإلكتروني بنجاح." : "Receipt has been sent to the member's email.";
                    }
                }
                else
                {
                    TempData["ErrorMessage"] = "Failed to send email. Member email or plan details missing.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error sending receipt email for member {Id}", memberId);
                TempData["ErrorMessage"] = "An error occurred while sending the email.";
            }

            return RedirectToAction(nameof(Receipt), new { memberId, planId });
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SendExpiryReminders(int days = 7)
        {
            var expiring = await _memberShipService.GetExpiringMemberShipsAsync(days);
            int count = 0;

            foreach (var ship in expiring)
            {
                try
                {
                    // Assuming ViewModel has Email and PlanName. Let's verify or use memberService to be safe.
                    var member = await _memberService.GetMemberDetailsAsync(ship.MemberId);
                    if (member != null && !string.IsNullOrEmpty(member.Email))
                    {
                        bool isArabic = System.Globalization.CultureInfo.CurrentUICulture.Name.StartsWith("ar");
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        var remainingDays = (DateTime.Parse(ship.EndDate) - DateTime.Now).Days;
                        string alertTemplate = EmailTemplates.ExpirationAlert(member.Name, gymSettings.GymName, gymSettings.Phone, gymSettings.Address, gymSettings.Email, ship.PlanName, remainingDays, isArabic);
                        await _emailService.SendEmailAsync(member.Email, isArabic ? "تنبيه: اقترب موعد انتهاء اشتراكك" : "Reminder: Your membership expires soon", alertTemplate);
                        count++;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send expiration reminder to member {Id}", ship.MemberId);
                }
            }

            TempData["SuccessMessage"] = $"{count} reminder emails sent successfully.";
            return RedirectToAction(nameof(Expiring), new { days });
        }

        private async Task LoadDropDownsAsync()
        {
            var members = await _memberShipService.GetMembersWithoutActiveMembershipAsync();
            var plans = await _memberShipService.GetActivePlansForDropDownAsync();
            
            ViewBag.Members = new SelectList(members, "Id", "Name");
            ViewBag.Plans = new SelectList(plans, "Id", "Name");
        }
    }
}
