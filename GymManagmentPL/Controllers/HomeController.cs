using GymManagmentBLL.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace GymManagmentPL.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;
        private readonly ILogger<HomeController> _logger;

        public HomeController(IAnalyticsService analyticsService, ILogger<HomeController> logger)
        {
            _analyticsService = analyticsService;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int months = 6, DateTime? startDate = null, DateTime? endDate = null)
        {
            var data = await _analyticsService.GetAnalyticsDataAsync(months, startDate, endDate);
            return View(data);
        }

        [AllowAnonymous]
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            var exceptionHandlerPathFeature = HttpContext.Features.Get<IExceptionHandlerPathFeature>();
            
            if (exceptionHandlerPathFeature?.Error != null)
            {
                _logger.LogError(exceptionHandlerPathFeature.Error, "Unhandled exception at {Path}", exceptionHandlerPathFeature.Path);
            }

            return View(new Models.ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
