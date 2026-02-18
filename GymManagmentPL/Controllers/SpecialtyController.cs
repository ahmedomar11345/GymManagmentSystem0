using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class SpecialtyController : Controller
    {
        private readonly ITrainerSpecialtyService _specialtyService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public SpecialtyController(ITrainerSpecialtyService specialtyService, IStringLocalizer<SharedResource> localizer)
        {
            _specialtyService = specialtyService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var specialties = await _specialtyService.GetAllSpecialtiesAsync();
            return View(specialties);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TrainerSpecialty specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty.Name))
            {
                TempData["ErrorMessage"] = _localizer["Required"].Value;
                return RedirectToAction(nameof(Index));
            }

            var result = await _specialtyService.CreateSpecialtyAsync(specialty);
            
            if (result)
                TempData["SuccessMessage"] = _localizer["SpecialtyCreated"].Value;
            else
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TrainerSpecialty specialty)
        {
            if (string.IsNullOrWhiteSpace(specialty.Name))
            {
                TempData["ErrorMessage"] = _localizer["Required"].Value;
                return RedirectToAction(nameof(Index));
            }

            var result = await _specialtyService.UpdateSpecialtyAsync(specialty);
            
            if (result)
                TempData["SuccessMessage"] = _localizer["SpecialtyUpdated"].Value;
            else
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;

            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _specialtyService.DeleteSpecialtyAsync(id);
            if (result)
                TempData["SuccessMessage"] = _localizer["SpecialtyDeleted"].Value;
            else
                TempData["ErrorMessage"] = _localizer["DeleteSpecialtyConfirm"].Value;

            return RedirectToAction(nameof(Index));
        }
    }
}
