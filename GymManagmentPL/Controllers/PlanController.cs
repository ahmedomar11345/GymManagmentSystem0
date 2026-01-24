using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.PlanViewModel;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentPL.Controllers
{
    public class PlanController : Controller
    {
        private readonly IPlanService _planService;

        public PlanController(IPlanService planService)
        {
            _planService = planService;
        }
        public IActionResult Index()
        {
            var plans = _planService.GetAllPlans();
            return View(plans);
        }
        public ActionResult Details(int id)
        {
            if(id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan Id.";
                return RedirectToAction(nameof(Index));
            }
            var plan = _planService.GetPlanById(id);
            if(plan is null) 
            {
                TempData["ErrorMessage"] = "Plan Not Found.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        public ActionResult Edit(int id)
        {
            if (id <= 0)
            {
                TempData["ErrorMessage"] = "Invalid Plan Id.";
                return RedirectToAction(nameof(Index));
            }
            var plan = _planService.GetPlanToUpdate(id);
            if (plan is null)
            {
                TempData["ErrorMessage"] = "Plan Not Found or Cannot be Edited.";
                return RedirectToAction(nameof(Index));
            }
            return View(plan);
        }

        [HttpPost]
        public ActionResult Edit([FromRoute]int id , UpdatePlanViewModel updatePlan)
        {
            if (!ModelState.IsValid)
            {
                ModelState.AddModelError("WrongData", "Please check the data you entered.");
                return View(updatePlan);
            }
            var result = _planService.UpdatePlan(id, updatePlan);
            if (result)
            {
                TempData["SuccessMessage"] = "Plan updated successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to update the plan.";
            }
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        public ActionResult Activate(int id)
        {
            var result = _planService.ToggleStatus(id);
            if (result)
            {
                TempData["SuccessMessage"] = "Plan status changed successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Failed to change plan status.";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
