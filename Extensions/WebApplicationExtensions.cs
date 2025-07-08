using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using Swashbuckle.AspNetCore.Filters;
using System.Security.Claims;

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

            // GET a single submission by eventId and userId
            // Get submit for authenticated user
            app.MapGet("/submits/{eventId}", async (int eventId, EventDbContext db, ClaimsPrincipal user) =>
            {
                // Get authenticated user ID
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

                if (string.IsNullOrEmpty(authenticatedUserId))
                    return Results.Unauthorized();

                // Find submit by composite key (EventId, UserId)
                var submit = await db.Submits.FindAsync(eventId, authenticatedUserId);

                if (submit == null)
                    return Results.NotFound();

                return Results.Ok(submit);
            })
                .WithSummary("Get submit for authenticated user")
                .WithDescription("Fetches a submission for the authenticated user by event ID." +
                " Returns 404 if the submission does not exist.")
                .RequireAuthorization();
          
            // Create new submit for authenticated user
            app.MapPost("/submits", async ([FromBody] Submit newSubmit, EventDbContext db, ClaimsPrincipal user) =>
            {
                if (newSubmit is null)
                    return Results.BadRequest("Invalid submit data.");

                // Get authenticated user ID
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

                if (string.IsNullOrEmpty(authenticatedUserId))
                    return Results.Unauthorized();

                // Set user ID from authentication token
                newSubmit.UserId = authenticatedUserId;

                // Check if submit already exists
                var exists = await db.Set<Submit>().AnyAsync(s =>
                    s.EventId == newSubmit.EventId && s.UserId == newSubmit.UserId);

                if (exists)
                    return Results.Conflict("A submission for this user and event already exists.");

                // Add to DB
                db.Submits.Add(newSubmit);
                await db.SaveChangesAsync();
              
                return Results.Created($"/submits/{newSubmit.EventId}", newSubmit);
            })
                .WithSummary("Create new submit for authenticated user")
                .WithDescription("Creates a new submit record for the authenticated user." +
                " Returns 409 if a submission already exists for the user and event.")
                .RequireAuthorization();

            // PUT endpoint
            app.MapPut("submits/{eventId}", async (int eventId, Submission submissionToUpdate, EventDbContext db, ClaimsPrincipal user) =>
            {
                // Enhanced authentication check
                if (user?.Identity?.IsAuthenticated != true)
                {
                    return Results.Unauthorized();
                }

                // Get authenticated user ID from claims
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub"); // Fallback for different claim types

                if (string.IsNullOrEmpty(authenticatedUserId))
                {
                    return Results.Unauthorized();
                }

                // Get the submit entity by eventId and authenticated userId
                var submit = await db.Set<Submit>()
                    .FirstOrDefaultAsync(s => s.EventId == eventId && s.UserId == authenticatedUserId);

                if (submit is null)
                {
                    return Results.NotFound("Submit record not found");
                }

                if (submit.Submissions is null)
                {
                    return Results.Problem("No submissions available");
                }

                // Find the submission by Id
                var existingSubmission = submit.Submissions.FirstOrDefault(s => s.Id == submissionToUpdate.Id);
                if (existingSubmission is null)
                {
                    return Results.NotFound("Submission not found");
                }

                // Update only non-null properties
                if (submissionToUpdate.Name != null)
                {
                    existingSubmission.Name = submissionToUpdate.Name;
                }

                if (submissionToUpdate.Options != null)
                {
                    existingSubmission.Options = submissionToUpdate.Options;
                }

                // Mark the JSON property as modified
                db.Entry(submit).Property(s => s.Submissions).IsModified = true;

                try
                {
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (DbUpdateException ex)
                {
                    // Log the exception details here
                    return Results.Problem("Database update failed");
                }
            })
                .WithSummary("Update submission for authenticated user")
                .WithDescription("Updates a specific submission for the authenticated user in the specified event." +
                "Returns 404 if the submission or submit record is not found.")
                .RequireAuthorization();

            app.MapDelete("/submits/{id:int}", async (int id, EventDbContext db, ClaimsPrincipal user) =>
            {
                // Authentication check
                if (user?.Identity?.IsAuthenticated != true)
                    return Results.Unauthorized();

                // Get authenticated user ID
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

                if (string.IsNullOrEmpty(authenticatedUserId))
                    return Results.Unauthorized();

                // Find the event
                var ev = await db.Events.FindAsync(id);
                if (ev == null)
                    return Results.NotFound("Event not found");

                // Authorization - Only allow admins to delete events
                if (!user.IsInRole("Admin"))
                    return Results.Forbid();

                // Perform deletion
                try
                {
                    db.Events.Remove(ev);
                    await db.SaveChangesAsync();
                    return Results.NoContent();
                }
                catch (DbUpdateException ex)
                {
                    // Log error (ex.Message, ex.InnerException, etc.)
                    return Results.Problem("Failed to delete event due to database error");
                }
            })
                .WithSummary("Delete submits by ID")
                .WithDescription("Deletes a submission by its ID for the authenticated user." +
                " Returns 404 if the submission is not found.")
                .RequireAuthorization();

            // User me

            /*

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
            */

            app.MapGet("/user/me", async (
            HttpContext httpContext,
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager) =>
            {
                var principal = httpContext.User;

                if (!principal.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }

                // Get the user ID from JWT (use ClaimTypes.NameIdentifier if you're using standard JWT claims)
                var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (userId == null)
                    return Results.Unauthorized();

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return Results.NotFound("User not found");

                var roles = await userManager.GetRolesAsync(user);

                // If the user has no roles, assign "User"
                if (roles.Count == 0)
                {
                    if (!await roleManager.RoleExistsAsync("User"))
                    {
                        await roleManager.CreateAsync(new IdentityRole("User"));
                    }

                    var result = await userManager.AddToRoleAsync(user, "User");
                    if (!result.Succeeded)
                    {
                        return Results.Problem("Failed to assign default role.");
                    }

                    roles = await userManager.GetRolesAsync(user); // Re-fetch roles
                }

                return Results.Ok(new
                {
                    UserId = user.Id,
                    Email = user.Email,
                    Roles = roles
                });
            });
        }
    }
}
