using GymManagmentBLL.Service.Interfaces;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin , Admin")]
    public class HomeController : Controller
    {
        private readonly IAnalyticsService _analyticsService;

        public HomeController(IAnalyticsService analyticsService)
        {
            _analyticsService = analyticsService;
        }
        // Index() =>> action
        public ActionResult Index()
        {
            var data = _analyticsService.GetAnalyticsData();
            return View(data);
        }
    }
}
