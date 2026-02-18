using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.AccountViewModel;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace GymManagmentPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEmailService _emailService;
        private readonly IMemoryCache _cache;
        private readonly IPasswordHasher<ApplicationUser> _passwordHasher;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public AccountController(
            IAccountService accountService, 
            SignInManager<ApplicationUser> _signInManager, 
            UserManager<ApplicationUser> userManager,
            IEmailService emailService,
            IMemoryCache cache,
            IPasswordHasher<ApplicationUser> passwordHasher,
            IStringLocalizer<SharedResource> localizer)
        {
            _accountService = accountService;
            this._signInManager = _signInManager;
            _userManager = userManager;
            _emailService = emailService;
            _cache = cache;
            _passwordHasher = passwordHasher;
            _localizer = localizer;
        }

        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));
            return View(user);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> UpdateProfile(EditProfileViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return RedirectToAction(nameof(Profile));
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.PhoneNumber = model.PhoneNumber;

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
            }
            else
            {
                TempData["ErrorMessage"] = _localizer["OperationFailed"].Value;
            }

            return RedirectToAction(nameof(Profile));
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return View("Settings", model);
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
            if (result.Succeeded)
            {
                await _signInManager.RefreshSignInAsync(user);
                TempData["SuccessMessage"] = _localizer["UpdateSuccess"].Value;
                return RedirectToAction(nameof(Settings));
            }

            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return View("Settings", model);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SendEmailOTP(ChangeEmailViewModel model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(Settings));

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            var otp = new Random().Next(100000, 999999).ToString();
            
            // Hash OTP before storing in database for maximum security
            user.OtpCode = _passwordHasher.HashPassword(user, otp);
            user.OtpExpiryTime = DateTime.Now.AddMinutes(10);
            user.PendingEmail = model.NewEmail;
            await _userManager.UpdateAsync(user);

            try
            {
                var subject = _localizer["EmailVerificationSubject"];
                var bodyTemplate = _localizer["EmailVerificationBody"].Value;
                var body = string.Format(bodyTemplate, $"<b style='font-size: 24px; color: #0d6efd;'>{otp}</b>");
                
                // Wrap in a styled div for better presentation
                var finalBody = $"<div dir='{(Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar") ? "rtl" : "ltr")}' style='text-align: {(Thread.CurrentThread.CurrentCulture.Name.StartsWith("ar") ? "right" : "left")};'><h3>{_localizer["VerifyYourEmail"]}</h3><p>{body}</p></div>";

                await _emailService.SendEmailAsync(model.NewEmail, subject, finalBody);
                
                ViewData["VerificationEmail"] = model.NewEmail;
                return View("VerifyOTP", new VerifyOtpViewModel { Email = model.NewEmail });
            }
            catch (Exception)
            {
                TempData["ErrorMessage"] = _localizer["EmailSendError"].Value;
                return RedirectToAction(nameof(Settings));
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> VerifyEmailOTP(VerifyOtpViewModel model)
        {
            if (!ModelState.IsValid) return View("VerifyOTP", model);

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return RedirectToAction(nameof(Login));

            // Verify hashed OTP from database
            var verificationResult = user.OtpCode != null 
                ? _passwordHasher.VerifyHashedPassword(user, user.OtpCode, model.OtpCode) 
                : PasswordVerificationResult.Failed;

            if (verificationResult == PasswordVerificationResult.Success && user.OtpExpiryTime > DateTime.Now && user.PendingEmail == model.Email)
            {
                var token = await _userManager.GenerateChangeEmailTokenAsync(user, model.Email);
                var result = await _userManager.ChangeEmailAsync(user, model.Email, token);
                
                if (result.Succeeded)
                {
                    await _userManager.SetUserNameAsync(user, model.Email);
                    
                    // Clear OTP fields after success
                    user.OtpCode = null;
                    user.OtpExpiryTime = null;
                    user.PendingEmail = null;
                    await _userManager.UpdateAsync(user);

                    await _signInManager.RefreshSignInAsync(user);
                    TempData["SuccessMessage"] = _localizer["EmailUpdatedSuccess"].Value;
                    return RedirectToAction(nameof(Settings));
                }
                
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, _localizer["InvalidOTP"]);
            }

            return View("VerifyOTP", model);
        }

        [Authorize]
        public IActionResult Settings()
        {
            return View(new ChangePasswordViewModel());
        }

        [AllowAnonymous]
        public IActionResult Login()
        {
            if (User.Identity?.IsAuthenticated == true) return RedirectToAction("Index", "Home");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = string.Join("<br>", ModelState.Values
                    .SelectMany(v => v.Errors)
                    .Select(e => e.ErrorMessage));
                TempData["ErrorMessage"] = errors;
                return View(model);
            }

            var user = await _accountService.ValidateUserAsync(model);
            if (user is null)
            {
                TempData["ErrorMessage"] = _localizer["InvalidLogin"].Value;
                ModelState.AddModelError("InvalidLogin", _localizer["InvalidLogin"]);
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, true); // lockoutOnFailure: true for security
            
            if (result.Succeeded)
            {
                return RedirectToAction("Index", "Home");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError("InvalidLogin", _localizer["AccountLockedOut"]);
            }
            else if (result.IsNotAllowed)
            {
                ModelState.AddModelError("InvalidLogin", _localizer["SignInNotAllowed"]);
            }
            else
            {
                ModelState.AddModelError("InvalidLogin", _localizer["InvalidSignInAttempt"]);
            }
            
            return View(model);
        }

        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction(nameof(Login));
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
