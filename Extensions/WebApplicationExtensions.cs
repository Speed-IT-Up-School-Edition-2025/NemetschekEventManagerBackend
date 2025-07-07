using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Extensions
{
    public static class WebApplicationExtensions
    {
        //Swagger configuration
        public static void ConfigureSwagger(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nemetschek Event API V1");
                options.DocumentTitle = "Nemetschek Event API UI";
                options.RoutePrefix = "docs"; // Swagger UI at https://localhost:<port>/docs
            });
        }
        // Map Identity API endpoints
        public static void MapEventEndpoints(this WebApplication app)
        {
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
            })
            .WithSummary("Create a new event")
            .WithDescription("Creates a new event with the provided details.");

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

        }
    }
}
