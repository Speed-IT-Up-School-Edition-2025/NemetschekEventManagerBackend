using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Seeders
{
    public static class AdminSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
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
