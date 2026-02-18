using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.ExpenseViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;
using System.Threading.Tasks;

namespace GymManagmentPL.Controllers
{
    [Authorize(Policy = "AdminOrAbove")]
    public class ExpenseController : Controller
    {
        private readonly IExpenseService _expenseService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public ExpenseController(IExpenseService expenseService, IStringLocalizer<SharedResource> localizer)
        {
            _expenseService = expenseService;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index()
        {
            var expenses = await _expenseService.GetAllExpensesAsync();
            return View(expenses);
        }

        public IActionResult Create()
        {
            return View(new ExpenseViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ExpenseViewModel model)
        {
            if (ModelState.IsValid)
            {
                await _expenseService.AddExpenseAsync(model);
                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }
            
            var errors = string.Join("<br>", ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage));
            TempData["ErrorMessage"] = errors;
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id > 0)
            {
                await _expenseService.DeleteExpenseAsync(id);
                TempData["SuccessMessage"] = _localizer["DeleteSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
