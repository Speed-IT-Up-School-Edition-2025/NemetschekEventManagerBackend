using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend.Extensions;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Seeders;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddAppDbContext(builder.Configuration)
    .AddAppIdentity()
    .AddAppSwagger()
    .SetupMailer(builder.Configuration);

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<User>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await AdminSeeder.SeedAsync(userManager, roleManager);
    await RoleSeeder.SeedAsync(roleManager);
}


// IN DEVELOPMENT STUFF HERE
if (app.Environment.IsDevelopment())
{
    //Swagger in DEV
    app.ConfigureSwagger();
}

// Use authentication & authorization
app.MapIdentityApi<User>();

// API endpoints
app.MapEventEndpoints();

app.Run();