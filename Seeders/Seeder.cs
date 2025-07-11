using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend.Models;
using System;

namespace NemetschekEventManagerBackend.Seeders
{
    public class Seeder
    {
        private static readonly string[] Roles = new[] { "Administrator", "User" };

        public static async Task SeedAsync(
        IServiceProvider serviceProvider,
        IConfiguration configuration)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<EventDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await SeedRoles(roleManager);
            await SeedAdministrator(userManager, roleManager);
        }

        public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in Roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        public static async Task SeedAdministrator(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminEmail = "admin@example.com";
            const string adminPassword = "Admin123!";
            const string adminRole = "Administrator";

            if (!await roleManager.RoleExistsAsync(adminRole))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRole));
            }

            var admins = await userManager.GetUsersInRoleAsync(adminRole);
            if (admins.Count > 0)
                return;

            // Create default admin user
            var adminUser = new User
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);
            if (!result.Succeeded)
            {
                throw new Exception("Failed to create default admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
            }

            await userManager.AddToRoleAsync(adminUser, adminRole);
        }
    }
}
