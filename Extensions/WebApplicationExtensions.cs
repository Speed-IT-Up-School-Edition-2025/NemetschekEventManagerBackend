using Microsoft.AspNetCore.Authorization;
using NemetschekEventManagerBackend.Interfaces;
using System.Security.Claims;
using NemetschekEventManagerBackend.Models.DTOs;

namespace NemetschekEventManagerBackend.Extensions
{
    public static class WebApplicationExtensions
    {
        //Swagger configuration
        public static void ConfigureSwagger(this WebApplication app)
        {
            app.UseStaticFiles();
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Nemetschek Event API V1");
                options.InjectStylesheet("/css/swagger-darkTheme.css"); //setting dark theme for the swagger UI
                options.DocumentTitle = "Nemetschek Event API UI";
                options.RoutePrefix = "docs"; // Swagger UI at https://localhost:<port>/docs
            });
        }
        // Map Identity API endpoints
        public static void MapEventEndpoints(this WebApplication app)
        {
            ////EVENT ENDPOINTS

            // Get all events
            app.MapGet("/events",
            [Authorize]
            (IEventService service) =>
            {
                return Results.Ok(service.GetEvents());
            })
            .WithSummary("Get all events")
            .WithDescription("Fetches all events from the database, excluding fields and submissions.");

            // Get events with filters (optional parameters)
            app.MapGet("/events/search", (
                IEventService service,
                string? searchName,
                DateTime? date,
                bool? activeOnly) =>
            {
                return service.GetEvents(searchName!, date, activeOnly);
            })
                .WithSummary("Search for events")
                .WithDescription("Fetches events based on optional filters like event name, date, and whether the event is still active.");

            // Get event by ID
            app.MapGet("/events/{id}", (IEventService service, int id) =>
            {
                var ev = service.GetEventById(id);
                return ev is not null ? Results.Ok(ev) : Results.NotFound();
            })
                .WithSummary("Get event by ID")
                .WithDescription("Fetches an event by its unique identifier (ID). If the event doesn't exist, returns a 404 error.");

            //Create new event
            app.MapPost("/events", (IEventService service, CreateEventDto dto) =>
            {
                if (string.IsNullOrWhiteSpace(dto.Name))
                    return Results.BadRequest("Event name is required.");

                var newEvent = EventMapper.ToEntity(dto);

                var success = service.Create(newEvent);

                return success
                    ? Results.Created($"/events/{newEvent.Id}", new { newEvent.Id })
                    : Results.BadRequest("Failed to create event.");
            })
            .WithSummary("Create a new event")
            .WithDescription("Creates a new event with the provided details. The server sets CreatedAt and UpdatedAt.");


            // Update event by ID (primitive params)
            app.MapPut("/events/{id}", (IEventService service, int id, UpdateEventDto dto) =>
            {
                var success = service.Update(id, dto);
                return success ? Results.Ok() : Results.NotFound();
            })
            .WithSummary("Update event by ID")
            .WithDescription("Updates an existing event using its ID and provided details. Returns 404 if not found.");

            // Delete event by ID
            app.MapDelete("/events/{id}", (IEventService service, int id) =>
            {
                var success = service.RemoveById(id);
                return success ? Results.Ok() : Results.NotFound();
            })
                .WithSummary("Delete event by ID")
                .WithDescription("Deletes an event using its unique ID. If the event is not found, returns a 404 error.");

            //// SUBMIT ENDPOINTS

            // GET submissions by eventId
            app.MapGet("/submits/{eventId}", (ISubmitService service, int eventId) =>
            {
                var submits = service.GetSubmitsByEventId(eventId);
                return submits.Count == 0
                    ? Results.NotFound()
                    : Results.Ok(submits);
            })
            .WithSummary("Get submit for authenticated user")
            .WithDescription("Fetches a submission for the authenticated user by event ID. Returns 404 if the submission does not exist.")
            .RequireAuthorization();

            // Create new submit for authenticated user
            app.MapPost("/submits/{eventId}", (int eventId, CreateSubmitDto dto, ISubmitService service, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var created = service.Create(eventId, userId,dto);
                return created
                    ? Results.Created($"/submits/{eventId}", dto)
                    : Results.Conflict("A submission already exists for this user and event.");
            })
            .WithSummary("Create new submit for authenticated user")
            .WithDescription("Creates a new submit record for the authenticated user. Returns 409 if one already exists.")
            .RequireAuthorization();

            // PUT endpoint
            app.MapPut("/submits/{eventId}", (int eventId, UpdateSubmitDto dto, ISubmitService service, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var updated = service.UpdateSubmission(eventId, userId, dto);
                return updated
                    ? Results.NoContent()
                    : Results.NotFound("Submit record not found or update failed.");
            })
            .WithSummary("Update submission for authenticated user")
            .WithDescription("Updates all submissions for the authenticated user in the specified event. Returns 404 if not found.")
            .RequireAuthorization();

            // User me
            app.MapGet("/user/me", (HttpContext httpContext) =>
            {
                var user = httpContext.User;

                if (!user.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }

                var userId = user.FindFirst("oid")?.Value;
                var email = user.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                return Results.Ok(new
                {
                    UserId = userId,
                    Email = email
                });
            });
        }
    }
}
