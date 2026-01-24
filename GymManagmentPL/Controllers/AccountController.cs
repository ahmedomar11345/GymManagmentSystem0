using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.AccountViewModel;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace GymManagmentPL.Controllers
{
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(IAccountService accountService, SignInManager<ApplicationUser> signInManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
        }
        //Login
        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(LoginViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var User = _accountService.ValidateUser(model);
            if (User is null)
            {
                ModelState.AddModelError("InvalidLogin", "Invalid username or password ");
                return View(model);
            }
            var Result = _signInManager.PasswordSignInAsync(User, model.Password, model.RememberMe, false).Result;
            if (Result.IsNotAllowed)
                ModelState.AddModelError("InvalidLogin", "You are not allowed to login. Please contact support.");

            if (Result.IsLockedOut)
                ModelState.AddModelError("InvalidLogin", "Your account is locked out. Please try again later.");

            if (Result.Succeeded)
                return RedirectToAction("Index", "Home");
            return View(model);
        }
        //Logout
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync().GetAwaiter().GetResult();
            return RedirectToAction(nameof(Login));
        }

        //AccessDenied
        public ActionResult AccessDenied()
        {
            return View();
        }
    }
}
