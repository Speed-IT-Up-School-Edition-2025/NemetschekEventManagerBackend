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

app.Run();