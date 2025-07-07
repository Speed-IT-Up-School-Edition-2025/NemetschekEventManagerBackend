using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddAppServices()
    .AddAppDbContext(builder.Configuration)
    .AddAppIdentity()
    .AddAppSwagger();

var app = builder.Build();

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