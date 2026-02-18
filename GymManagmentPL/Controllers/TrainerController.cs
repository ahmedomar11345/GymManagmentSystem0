using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.Service.Implementations;
using GymManagmentBLL.ViewModels.TrainerViewModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    [Authorize(Roles = "SuperAdmin,Admin")]
    public class TrainerController : Controller
    {
        private readonly ITrainerService _trainerService;
        private readonly IEmailService _emailService;
        private readonly IGymSettingsService _gymSettingsService;
        private readonly ITrainerSpecialtyService _specialtyService;
        private readonly IQRCodeService _qrCodeService;
        private readonly ILogger<TrainerController> _logger;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public TrainerController(ITrainerService trainerService, 
            IEmailService emailService, 
            IGymSettingsService gymSettingsService, 
            ITrainerSpecialtyService specialtyService,
            IQRCodeService qrCodeService,
            ILogger<TrainerController> logger,
            IStringLocalizer<SharedResource> localizer)
        {
            _trainerService = trainerService;
            _emailService = emailService;
            _gymSettingsService = gymSettingsService;
            _specialtyService = specialtyService;
            _qrCodeService = qrCodeService;
            _logger = logger;
            _localizer = localizer;
        }

        public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 10, string? searchTerm = null)
        {
            var model = await _trainerService.GetTrainersPagedAsync(pageNumber, pageSize, searchTerm);
            ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
            ViewData["SearchTerm"] = searchTerm;
            
            // For the Tabbed View, we might need all specialties
            
            return View(model);
        }

        public async Task<IActionResult> Create()
        {
            ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
            return View(new CreateTrainerViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateTrainerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
                return View(model);
            }

            var result = await _trainerService.CreateTrainerAsync(model);
            if (result)
            {
                try
                {
                    // Get the created trainer to access the AccessKey
                    var allTrainers = await _trainerService.GetAllTrainersAsync();
                    var createdTrainer = allTrainers.OrderByDescending(t => t.Id).FirstOrDefault(t => t.Email == model.Email);
                    
                    if (createdTrainer != null)
                    {
                        // Get detailed info for the QR
                        var trainerDetails = await _trainerService.GetTrainerDetailsAsync(createdTrainer.Id);
                        
                        bool isArabic = model.SendInArabic;
                        var gymSettings = await _gymSettingsService.GetSettingsAsync();
                        
                        // Generate QR Code using the AccessKey (this is what will be scanned)
                        // Note: Assuming AccessKey is available in a way we can get it. 
                        // Let's check TrainerViewModel if it has AccessKey.
                        
                        // Generating QR
                        var qrCodeBytes = _qrCodeService.GenerateQRCode(createdTrainer.AccessKey);
                        string cid = "trainer_qr_" + createdTrainer.Id;
                        
                        string welcomeTemplate = EmailTemplates.MemberQRCodeWithCID(
                            model.Name, 
                            gymSettings.GymName, 
                            gymSettings.Phone, 
                            gymSettings.Address, 
                            gymSettings.Email, 
                            cid, 
                            isArabic,
                            isArabic ? "مرحباً بك في الفريق! إليك رمز الدخول الخاص بك واللازم لتسجيل حضورك وانصرافك." 
                                     : "Welcome to the team! Here is your entry code required for your attendance check-in and check-out.");

                        await _emailService.SendEmailWithImageAsync(
                            model.Email, 
                            isArabic ? "مرحباً بك في الفريق - كود الدخول الخاص بك" : "Welcome to the Team - Your Access QR Code", 
                            welcomeTemplate, 
                            qrCodeBytes, 
                            cid);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send welcome QR email to Trainer {Email}", model.Email);
                }

                TempData["SuccessMessage"] = _localizer["CreateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["InvalidInput"].Value;
            ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            if (id <= 0) return BadRequest();

            var trainer = await _trainerService.GetTrainerDetailsAsync(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = _localizer["NotFound"].Value;
                return RedirectToAction(nameof(Index));
            }

            return View(trainer);
        }

        public async Task<IActionResult> Edit(int id)
        {
            if (id <= 0) return BadRequest();

            var trainer = await _trainerService.GetTrainerToUpdateAsync(id);
            if (trainer == null)
            {
                TempData["ErrorMessage"] = _localizer["NotFound"].Value;
                return RedirectToAction(nameof(Index));
            }
            ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
            return View(trainer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, TrainerToUpdateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
                return View(model);
            }

            var result = await _trainerService.UpdateTrainerDetailsAsync(model, id);
            if (result)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Index));
            }

            TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            ViewBag.Specialties = await _specialtyService.GetAllSpecialtiesAsync();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0) return BadRequest();

            var result = await _trainerService.RemoveTrainerAsync(id);
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
    }
}
