using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.WalkInViewModel;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class WalkInController : Controller
    {
        private readonly IWalkInService _walkInService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public WalkInController(IWalkInService walkInService, 
            IGymSettingsService gymSettingsService,
            IStringLocalizer<SharedResource> localizer)
        {
            _walkInService = walkInService;
            _gymSettingsService = gymSettingsService;
            _localizer = localizer;
        }

        // GET: /WalkIn
        public async Task<IActionResult> Index(string? search)
        {
            IEnumerable<WalkInBooking> bookings;
            if (!string.IsNullOrWhiteSpace(search))
                bookings = await _walkInService.SearchAsync(search);
            else
                bookings = await _walkInService.GetAllBookingsAsync();

            ViewBag.Search = search;
            return View(bookings);
        }

        // GET: /WalkIn/Create
        public async Task<IActionResult> Create(int? memberId)
        {
            var settings = await _gymSettingsService.GetSettingsAsync();
            var model = new CreateWalkInViewModel
            {
                PricePerSession = settings.SessionPrice,
                MemberId = memberId
            };
            ViewBag.Currency = settings.Currency;
            ViewBag.SessionPrice = settings.SessionPrice;
            return View(model);
        }

        // POST: /WalkIn/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateWalkInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var settings = await _gymSettingsService.GetSettingsAsync();
                ViewBag.Currency = settings.Currency;
                ViewBag.SessionPrice = settings.SessionPrice;
                return View(model);
            }

            var booking = await _walkInService.CreateBookingAsync(model);
            TempData["Success"] = _localizer["WalkInSuccess"].Value;
            return RedirectToAction("Receipt", new { id = booking.Id });
        }

        // GET: /WalkIn/Receipt/5
        public async Task<IActionResult> Receipt(int id)
        {
            var booking = await _walkInService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            var settings = await _gymSettingsService.GetSettingsAsync();
            ViewBag.GymSettings = settings;
            return View(booking);
        }

        // GET: /WalkIn/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = await _walkInService.GetBookingByIdAsync(id);
            if (booking == null) return NotFound();

            var settings = await _gymSettingsService.GetSettingsAsync();
            ViewBag.Currency = settings.Currency;
            return View(booking);
        }

        // POST: /WalkIn/UseSession/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UseSession(int id)
        {
            var success = await _walkInService.UseSessionAsync(id);
            if (!success)
                TempData["Error"] = _localizer["WalkInError"].Value;
            else
                TempData["Success"] = _localizer["SessionSuccess"].Value;

            return RedirectToAction("Details", new { id });
        }
    }
}
