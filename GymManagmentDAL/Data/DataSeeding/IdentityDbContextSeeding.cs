using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Data.DataSeeding
{
    public static class IdentityDbContextSeeding
    {
        public static async Task<bool> SeedData(RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager)
        {
			try
			{
                var HasUser = userManager.Users.Any();
                var HasRole = roleManager.Roles.Any();
                if (HasRole && HasUser) return false;
                if (!HasRole)
                {
                    var Roles = new List<IdentityRole>() {
                        new () {Name = "SuperAdmin"},
                        new () {Name = "Admin"},
                    };
                    foreach(var role in Roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name!))
                        {
                           await roleManager.CreateAsync(role);
                        }
                    }
                }

                if (!HasUser)
                {
                    var MainAdmin = new ApplicationUser()
                    {
                        FirstName = "Ahmed",
                        LastName = "Omar",
                        UserName = "AhmedOmar",
                        Email = "ahmedomar@gmail.com",
                        PhoneNumber = "01029897898",

                    };
                    var createResult = await userManager.CreateAsync(MainAdmin,"99Pass@ord#");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(MainAdmin, "SuperAdmin");
                    }
                    else
                    {
                        
                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        throw new Exception(errors);
                    }

                    var Admin = new ApplicationUser()
                    {
                        FirstName = "Omar",
                        LastName = "Ali",
                        UserName = "OmarAli",
                        Email = "omarali@gmail.com",
                        PhoneNumber = "01029898989",

                    };
                    var createResult2 = await userManager.CreateAsync(Admin, "99Pass@ord#");
                    if (createResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(Admin, "Admin");
                    }
                    else
                    {

                        var errors = string.Join(", ", createResult.Errors.Select(e => e.Description));
                        throw new Exception(errors);
                    }

                }
                return true;
            }
			catch (Exception ex)
			{
                Console.WriteLine($"Seed Faild {ex}");

				return false;
			}
        }



    }
}
