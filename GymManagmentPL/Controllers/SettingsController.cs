using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SettingsController : Controller
    {
        private readonly IGymSettingsService _settingsService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SettingsController(IGymSettingsService settingsService, IStringLocalizer<SharedResource> localizer)
        {
            _settingsService = settingsService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(GymSettings settings)
        {
            if (ModelState.IsValid)
            {
                await _settingsService.UpdateSettingsAsync(settings);
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            var errors = string.Join("<br>", ModelState.Values
                 .SelectMany(v => v.Errors)
                 .Select(e => e.ErrorMessage));
            TempData["ErrorMessage"] = errors;
            
            return View("Index", settings);
        }
    }
}
