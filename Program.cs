using Microsoft.AspNetCore.Identity;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Extensions;
using NemetschekEventManagerBackend.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddAppDbContext(builder.Configuration)
    .AddAppIdentity()
    .AddAppSwagger()
    .SetupMailer(builder.Configuration);

var app = builder.Build();

/*
using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    await RoleSeeder.SeedAsync(roleManager);
}
*/

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