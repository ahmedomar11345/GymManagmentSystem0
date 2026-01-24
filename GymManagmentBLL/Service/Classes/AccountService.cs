using GymManagmentBLL.Service.Interfaces;
using GymManagmentBLL.ViewModels.AccountViewModel;
using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentBLL.Service.Classes
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountService(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }
        public ApplicationUser? ValidateUser(LoginViewModel loginViewModel)
        {
            var User = _userManager.FindByEmailAsync(loginViewModel.Email).Result;
            if(User is null ) return null;
            var IsPasswordValid = _userManager.CheckPasswordAsync(User, loginViewModel.Password).Result;
            return IsPasswordValid ? User : null;
        }
    }
}
