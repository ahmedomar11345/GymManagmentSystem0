using GymManagmentDAL.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagmentDAL.Data.DataSeeding
{
    public static class IdentityDbContextSeeding
    {
        public static async Task<bool> SeedData(
            RoleManager<IdentityRole> roleManager, 
            UserManager<ApplicationUser> userManager,
            IConfiguration configuration)
        {
            try
            {
                var hasUsers = userManager.Users.Any();
                var hasRoles = roleManager.Roles.Any();
                
                if (hasRoles && hasUsers) 
                    return false;

                // Seed Roles
                if (!hasRoles)
                {
                    var roles = new List<IdentityRole>
                    {
                        new() { Name = "SuperAdmin" },
                        new() { Name = "Admin" },
                        new() { Name = "Trainer" },  // Future: Allow trainers to log in
                        new() { Name = "Member" }    // Future: Allow members to log in
                    };

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role.Name!))
                        {
                            await roleManager.CreateAsync(role);
                        }
                    }
                }

                // Seed Default Users
                if (!hasUsers)
                {
                    // Get password from configuration (secure)
                    var defaultPassword = configuration["AdminSettings:DefaultPassword"];
                    if (string.IsNullOrEmpty(defaultPassword))
                    {
                        // Fallback for development - NEVER use in production
                        defaultPassword = "Gym@Admin123!";
                        Console.WriteLine("WARNING: Using fallback password. Set AdminSettings:DefaultPassword in configuration for production.");
                    }

                    // Create SuperAdmin
                    var superAdmin = new ApplicationUser
                    {
                        FirstName = "Super",
                        LastName = "Admin",
                        UserName = "superadmin",
                        Email = "superadmin@gymmanagement.com",
                        PhoneNumber = "01000000001",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var superAdminResult = await userManager.CreateAsync(superAdmin, defaultPassword);
                    if (superAdminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(superAdmin, "SuperAdmin");
                        Console.WriteLine("SuperAdmin user created successfully.");
                    }
                    else
                    {
                        var errors = string.Join(", ", superAdminResult.Errors.Select(e => e.Description));
                        Console.WriteLine($"Failed to create SuperAdmin: {errors}");
                    }

                    // Create Admin
                    var admin = new ApplicationUser
                    {
                        FirstName = "System",
                        LastName = "Admin",
                        UserName = "admin",
                        Email = "admin@gymmanagement.com",
                        PhoneNumber = "01000000002",
                        EmailConfirmed = true,
                        PhoneNumberConfirmed = true
                    };

                    var adminResult = await userManager.CreateAsync(admin, defaultPassword);
                    if (adminResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                        Console.WriteLine("Admin user created successfully.");
                    }
                    else
                    {
                        var errors = string.Join(", ", adminResult.Errors.Select(e => e.Description));
                        Console.WriteLine($"Failed to create Admin: {errors}");
                    }
                }

                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Seed Failed: {ex.Message}");
                return false;
            }
        }
    }
}
