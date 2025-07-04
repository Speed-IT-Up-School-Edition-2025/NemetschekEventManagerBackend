using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<EventDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
