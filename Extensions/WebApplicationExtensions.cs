using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Seeding;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using Swashbuckle.AspNetCore.Filters;


namespace NemetschekEventManagerBackend.Extensions
{
    public static class WebApplicationExtensions
    {
        public static void ConfigureRoleBasedAuthorization(this WebApplication app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
        }

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
            app.MapGet("/events", (IEventService service) =>
            {
                return service.GetEvents();
            })
                .WithSummary("Get all events")
                .WithDescription("Fetches all events from the database without any filters.");

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
            })
                .WithSummary("Update event by ID")
                .WithDescription("Updates an existing event using its ID and provided details. Returns a 404 error if the event is not found.");

            // Update full event (via model binding)
            app.MapPut("/events", (IEventService service, Event ev) =>
            {
                service.UpdateEvent(ev);
                return Results.Ok();
            })
                .WithSummary("Update full event")
                .WithDescription("Updates an entire event object using model binding, replacing the existing event details.");


            // Delete event by ID
            app.MapDelete("/events/{id}", (IEventService service, int id) =>
            {
                var success = service.RemoveById(id);
                return success ? Results.Ok() : Results.NotFound();
            })
                .WithSummary("Delete event by ID")
                .WithDescription("Deletes an event using its unique ID. If the event is not found, returns a 404 error.");

            //// SUBMIT ENDPOINTS

            // GET all submissions endpoint

            // returns a list of all Submit records in the database
            app.MapGet("/api/submits", async (EventDbContext db) =>
            {
                // Fetch all submissions from the database asynchronously
                var submissions = await db.Submits.ToListAsync();

                // Return HTTP 200 OK with the list of submissions
                return Results.Ok(submissions);
            }).WithSummary("Get all submissions")
            .WithDescription("Gets all submissions as it sortes them. If the submission is not found will return error 404.");

            // GET a single submission by eventId and userId
            // URL: /api/submits/{eventId}/{userId}
            app.MapGet("/api/submits/{eventId}/{userId}", async (int eventId, string userId, EventDbContext db) =>
            {
                // Find the submit entry by composite key (EventId, UserId)
                var submit = await db.Submits.FindAsync(eventId, userId);

                // If not found, return 404 Not Found
                if (submit == null)
                    return Results.NotFound();

                // Return HTTP 200 OK with the found submission
                return Results.Ok(submit);
            }).WithSummary("Get submission by event ID and user ID")
            .WithDescription("Returns all the submissions that are sorted by the unique ID of the event and the user. If not found will return error 404.");

            // Create a new Submit
            app.MapPost("/api/submits", async ([FromBody] Submit newSubmit, EventDbContext db) =>
            {
                if (newSubmit is null)
                    return Results.BadRequest("Invalid submit data.");

                // check if it already exists 
                var exists = await db.Set<Submit>().AnyAsync(s =>
                    s.EventId == newSubmit.EventId && s.UserId == newSubmit.UserId);

                if (exists)
                    return Results.Conflict("A submission for this user and event already exists.");

                // Add to DB
                db.Submits.Add(newSubmit);
                await db.SaveChangesAsync();

                return Results.Created($"/api/submits/{newSubmit.EventId}/{newSubmit.UserId}", newSubmit);
            }).WithSummary("Creates new submission")
            .WithDescription("Creates new submission as it uses instance of a submission");

            // PUT endpoint
            app.MapPut("/api/submits/{eventId}/{userId}", async (int eventId, string userId, Submission submissionToUpdate, EventDbContext db) =>
            {
                var submit = await db.Set<Submit>()
                    .Include(s => s.Submissions)
                    .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == userId);

                if (submit is null)
                    return Results.NotFound();

                // Find the submission to update by Id
                var existingSubmission = submit.Submissions?.FirstOrDefault(s => s.Id == submissionToUpdate.Id);
                if (existingSubmission is null)
                    return Results.NotFound();

                existingSubmission.Name = submissionToUpdate.Name;
                existingSubmission.Options = submissionToUpdate.Options;

                await db.SaveChangesAsync();

                return Results.NoContent();
            }).WithSummary("Creates new submission using specific ID")
            .WithDescription("Creates new submission using provided details such as (event ID and user ID)");

            app.MapDelete("/api/events/{id:int}", async (int id, EventDbContext db) =>
            {
                var ev = await db.Events.FindAsync(id);
                if (ev == null)
                {
                    return Results.NotFound();
                }

                db.Events.Remove(ev);
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).WithSummary("Deletes a submission")
            .WithDescription("Deletes a submission by using the unique ID");
        }


    }
}
