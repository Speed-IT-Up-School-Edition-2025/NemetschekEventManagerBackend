using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.Extensions.DependencyInjection;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Extensions;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Seeding;
using System;

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

app.ConfigureRoleBasedAuthorization();

// API endpoints
app.MapEventEndpoints();


app.Run();