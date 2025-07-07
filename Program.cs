using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);
// Register serviceс
builder.Services.AddScoped<IEventService, EventService>();

builder.Services.AddDbContext<EventDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
// Configure the HTTP request pipeline.
builder.Services.AddAuthorization();
// Add Identity services
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<EventDbContext>();

var app = builder.Build();

app.MapIdentityApi<IdentityUser>();

// Get all events
app.MapGet("/events", (IEventService service) =>
{
    return service.GetEvents();
});

// Get events with filters (optional parameters)
app.MapGet("/events/search", (
    IEventService service,
    string? searchName,
    DateTime? date,
    bool? activeOnly) =>
{
    return service.GetEvents(searchName!, date, activeOnly);
});

// Get event by ID
app.MapGet("/events/{id}", (IEventService service, int id) =>
{
    var ev = service.GetEventById(id);
    return ev is not null ? Results.Ok(ev) : Results.NotFound();
});

// Create event
app.MapPost("/events", (IEventService service, Event newEvent) =>
{
    if (string.IsNullOrWhiteSpace(newEvent.Name))
        return Results.BadRequest("Event name is required.");

    var success = service.Create(
        newEvent.Name,
        newEvent.Description ?? string.Empty,
        newEvent.Date,
        newEvent.SignUpEndDate,
        newEvent.Location ?? string.Empty
    );

    return success ? Results.Ok("Event created successfully.") : Results.BadRequest("Failed to create event.");
});

// Update event by ID (primitive params)
app.MapPut("/events/{id}", (
    IEventService service,
    int id,
    string name,
    string description,
    DateTime? date,
    DateTime? signUpEndDate,
    string location) =>
{
    var success = service.Update(id, name, description, date, signUpEndDate, location);
    return success ? Results.Ok() : Results.NotFound();
});

// Update full event (via model binding)
app.MapPut("/events", (IEventService service, Event ev) =>
{
    service.UpdateEvent(ev);
    return Results.Ok();
});

// Delete event by ID
app.MapDelete("/events/{id}", (IEventService service, int id) =>
{
    var success = service.RemoveById(id);
    return success ? Results.Ok() : Results.NotFound();
});

app.Run();