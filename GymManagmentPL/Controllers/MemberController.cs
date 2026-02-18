using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.ViewModels.MemberViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using ClosedXML.Excel;
using System.IO;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class MemberController : Controller
    {
        private readonly IMemberService _memberService;
        private readonly IAttendanceService _attendanceService;
        private readonly IPlanService _planService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly ILogger<MemberController> _logger;
        private readonly IEmailService _emailService;

        public MemberController(IMemberService memberService, 
            IAttendanceService attendanceService, 
            IPlanService planService, 
            IStringLocalizer<SharedResource> localizer,
            ILogger<MemberController> logger,
            IEmailService emailService)
        {
            _memberService = memberService;
            _attendanceService = attendanceService;
            _planService = planService;
            _localizer = localizer;
            _logger = logger;
            _emailService = emailService;
        }

        public async Task<IActionResult> ExportToExcel()
        {
            try
            {
                var members = await _memberService.GetAllMembersAsync();

                using (var workbook = new XLWorkbook())
                {
                    var worksheet = workbook.Worksheets.Add("Members");
                    var currentRow = 1;

                    // Headers
                    worksheet.Cell(currentRow, 1).Value = "ID";
                    worksheet.Cell(currentRow, 2).Value = "Name";
                    worksheet.Cell(currentRow, 3).Value = "Email";
                    worksheet.Cell(currentRow, 4).Value = "Phone";
                    worksheet.Cell(currentRow, 5).Value = "Gender";
                    worksheet.Cell(currentRow, 6).Value = "Date of Birth";
                    worksheet.Cell(currentRow, 7).Value = "Join Date";

                    // Styling Headers
                    var headerRange = worksheet.Range(1, 1, 1, 7);
                    headerRange.Style.Font.Bold = true;
                    headerRange.Style.Fill.BackgroundColor = XLColor.BabyBlue;
                    headerRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                    // Data
                    foreach (var member in members)
                    {
                        currentRow++;
                        worksheet.Cell(currentRow, 1).Value = member.Id;
                        worksheet.Cell(currentRow, 2).Value = member.Name;
                        worksheet.Cell(currentRow, 3).Value = member.Email;
                        worksheet.Cell(currentRow, 4).Value = member.Phone;
                        worksheet.Cell(currentRow, 5).Value = member.Gender;
                        worksheet.Cell(currentRow, 6).Value = member.DateOfBirth;
                        worksheet.Cell(currentRow, 7).Value = member.JoinDate.ToShortDateString();
                    }

                    worksheet.Columns().AdjustToContents();

                    using (var stream = new MemoryStream())
                    {
                        workbook.SaveAs(stream);
                        var content = stream.ToArray();

                        return File(
                            content,
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            $"GymMembers_{DateTime.Now:yyyyMMdd}.xlsx");
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting members to Excel");
                TempData["ErrorMessage"] = "Failed to export data.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var pagedMembers = await _memberService.GetMembersPagedAsync(pageNumber, pageSize, searchTerm);
            ViewData["SearchTerm"] = searchTerm;
            return View(pagedMembers);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }

            member.CheckInHistory = await _attendanceService.GetMemberAttendanceHistoryAsync(id);
            return View(member);
        }

        public async Task<IActionResult> Create()
        {
            var model = new CreateMemberViewModel
            {
                Plans = await _planService.GetAllPlansAsync()
            };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateMemberViewModel createMember)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;

                createMember.Plans = await _planService.GetAllPlansAsync();
                return View(createMember);
            }

            var result = await _memberService.CreateMemberAsync(createMember);
            if (result)
            {
                TempData["SuccessMessage"] = createMember.PlanId.HasValue
                    ? _localizer["CreateSuccess"].Value
                    : _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["InvalidInput"].Value;
            createMember.Plans = await _planService.GetAllPlansAsync();
            return View(createMember);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var member = await _memberService.GetMemberToUpdateAsync(id);
            if (member is null)
            {
                TempData["ErrorMessage"] = "Member not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, MemberToUpdateViewModel memberToUpdate)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return View(memberToUpdate);
            }

            var result = await _memberService.UpdateMemberAsync(id, memberToUpdate);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            return View(memberToUpdate);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var result = await _memberService.RemoveMemberAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["DeleteSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> HealthrecordDetails(int id)
        {
            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member == null) return NotFound();
            
            return View(member);
        }

        [HttpGet]
        public async Task<IActionResult> HealthProgress(int id)
        {
            var member = await _memberService.GetMemberDetailsAsync(id);
            if (member == null) return NotFound();

            var progress = await _memberService.GetMemberHealthProgressAsync(id);
            ViewBag.Progress = progress;
            return View(member);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddHealthProgress(HealthProgressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _memberService.AddHealthProgressAsync(model);
                if (result)
                {
                    TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                }
                else
                {
                    TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
                }
            }
            else
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
            }
            return RedirectToAction(nameof(HealthProgress), new { id = model.MemberId });
        }
    }
}
