using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.IO;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class SettingsController : Controller
    {
        private readonly IGymSettingsService _settingsService;
        private readonly IStringLocalizer<SharedResource> _localizer;
        private readonly IWebHostEnvironment _env;

        public SettingsController(IGymSettingsService settingsService,
            IStringLocalizer<SharedResource> localizer,
            IWebHostEnvironment env)
        {
            _settingsService = settingsService;
            _localizer = localizer;
            _env = env;
        }

        public async Task<IActionResult> Index()
        {
            var settings = await _settingsService.GetSettingsAsync();
            return View(settings);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Update(GymSettings settings, IFormFile? logoFile)
        {
            if (ModelState.IsValid)
            {
                // Handle logo upload
                if (logoFile != null && logoFile.Length > 0)
                {
                    var allowed = new[] { ".jpg", ".jpeg", ".png", ".svg", ".webp" };
                    var ext = Path.GetExtension(logoFile.FileName).ToLowerInvariant();
                    if (allowed.Contains(ext))
                    {
                        var uploadsDir = Path.Combine(_env.WebRootPath, "uploads", "logos");
                        Directory.CreateDirectory(uploadsDir);
                        var fileName = $"gym-logo{ext}";
                        var filePath = Path.Combine(uploadsDir, fileName);
                        using var stream = new FileStream(filePath, FileMode.Create);
                        await logoFile.CopyToAsync(stream);
                        settings.LogoUrl = $"/uploads/logos/{fileName}";
                    }
                }
                // else: keep existing LogoUrl (passed as hidden field)

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
