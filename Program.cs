using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Add services to the container.
builder.Services.AddDbContext<EventDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))); var app = builder.Build();
// Configure the HTTP request pipeline.
builder.Services.AddAuthorization();
// Add Identity services
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<EventDbContext>();

app.MapIdentityApi<IdentityUser>();

app.MapGet("/", () => "Hello World!");

app.Run();
