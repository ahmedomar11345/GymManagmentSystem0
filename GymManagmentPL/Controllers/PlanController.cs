using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PlanController(IPlanService planService, IStringLocalizer<SharedResource> localizer)
        {
            _planService = planService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var plans = await _planService.GetAllPlansAsync();
            return View(plans);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreatePlanViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return View(model);
            }

            var result = await _planService.CreatePlanAsync(model);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan Id.";
                return RedirectToAction(nameof(Index));
            }
            var plan = await _planService.GetPlanByIdAsync(id);
            if (plan is null) 
            {
                TempData["ErrorMessage"] = "Plan Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan Id.";
                return RedirectToAction(nameof(Index));
            }
            var plan = await _planService.GetPlanToUpdateAsync(id);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan Not Found or Cannot be Edited (has active memberships).";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UpdatePlanViewModel updatePlan)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return View(updatePlan);
            }
            var result = await _planService.UpdatePlanAsync(id, updatePlan);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            
            TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            return View(updatePlan);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(int id)
        {
            var result = await _planService.ToggleStatusAsync(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Plan status changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change plan status. It may have active memberships.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
