using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;

namespace NemetschekEventManagerBackend.Extensions
{
    public static class WebApplicationExtensions
    {
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

            // Create event
            app.MapPost("/events", async (IEventService service, HttpContext http) =>
            {
                var newEvent = await http.Request.ReadFromJsonAsync<Event>();

                if (newEvent == null || string.IsNullOrWhiteSpace(newEvent.Name))
                    return Results.BadRequest("Invalid event data.");

                var success = service.Create(
                    newEvent.Name,
                    newEvent.Description!,
                    newEvent.Date,
                    newEvent.SignUpEndDate,
                    newEvent.Location!
                );

                return success ? Results.Ok() : Results.BadRequest("Failed to create event");
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

        }
    }
}
