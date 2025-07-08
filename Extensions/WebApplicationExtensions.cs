using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using Swashbuckle.AspNetCore.Filters;
using System.Data;
using System.Security.Claims;
using System.Linq;

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
            app.MapGet("/events/search",
            [Authorize]
            (IEventService service,
                string? searchName,
                DateTime? date,
                bool? activeOnly) =>
            {
                return service.GetEvents(searchName!, date, activeOnly);
            })
                .WithSummary("Search for events")
                .WithDescription("Fetches events based on optional filters like event name, date, and whether the event is still active.");

            // Get event by ID
            app.MapGet("/events/{id}",
            [Authorize]
            (IEventService service, int id) =>
            {
                var ev = service.GetEventById(id);
                return ev is not null ? Results.Ok(ev) : Results.NotFound();
            })
                .WithSummary("Get event by ID")
                .WithDescription("Fetches an event by its unique identifier (ID). If the event doesn't exist, returns a 404 error.");

            app.MapPost("/events",
            [Authorize(Roles = "Administrator")]
            (IEventService service, Event newEvent) =>
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
            app.MapPut("/events/{id}",
            [Authorize(Roles = "Administrator")]
            (
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
            app.MapPut("/events",
            [Authorize(Roles = "Administrator")]
            (IEventService service, Event ev) =>
            {
                service.UpdateEvent(ev);
                return Results.Ok();
            })
                .WithSummary("Update full event")
                .WithDescription("Updates an entire event object using model binding, replacing the existing event details.");


            // Delete event by ID
            app.MapDelete("/events/{id}",
            [Authorize(Roles = "Administrator")]
            (IEventService service, int id) =>
            {
                var success = service.RemoveById(id);
                return success ? Results.Ok() : Results.NotFound();
            })
                .WithSummary("Delete event by ID")
                .WithDescription("Deletes an event using its unique ID. If the event is not found, returns a 404 error.");

            //// SUBMIT ENDPOINTS

            // GET a single submission by eventId and userId
            // Get submit for authenticated user
            app.MapGet("/submits/{eventId}",
            [Authorize]
            async (int eventId, EventDbContext db, ClaimsPrincipal user) =>
            {
                // Get authenticated user ID
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

                // Find submit by composite key (EventId, UserId)
                var submit = await db.Submits.FindAsync(eventId, authenticatedUserId);

                if (submit == null)
                    return Results.NotFound();

                return Results.Ok(submit);
            })
                .WithSummary("Get submit for authenticated user")
                .WithDescription("Fetches a submission for the authenticated user by event ID." +
                " Returns 404 if the submission does not exist.");

            // Create new submit for authenticated user
            app.MapPost("/submits",
            [Authorize]
            async ([FromBody] Submit newSubmit, EventDbContext db, ClaimsPrincipal user) =>
            {
                if (newSubmit is null)
                    return Results.BadRequest("Invalid submit data.");

                // Get authenticated user ID
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

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
                " Returns 409 if a submission already exists for the user and event."); ;

            // PUT endpoint
            app.MapPut("submits/{eventId}",
            [Authorize]
            async (int eventId, Submission submissionToUpdate, EventDbContext db, ClaimsPrincipal user) =>
            {
                // Get authenticated user ID from claims
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

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
                "Returns 404 if the submission or submit record is not found.");

            app.MapDelete("/submits/{id}",
            [Authorize]
            async (int id, EventDbContext db, ClaimsPrincipal user) =>
            {
                // Get authenticated user ID
                var authenticatedUserId = user.FindFirstValue(ClaimTypes.NameIdentifier)
                                          ?? user.FindFirstValue("sub");

                // Find the event
                var sub = await db.Submits.FindAsync(id);
                if (sub == null)
                    return Results.NotFound("Event not found");

                // Perform deletion
                try
                {
                    db.Submits.Remove(sub);
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
                " Returns 404 if the submission is not found.");

            // Get user info

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

                var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

                if (userId == null)
                    return Results.Unauthorized();

                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                    return Results.NotFound("User not found");

                // Set times only if set to null
                if (user.CreatedAt == null)
                {
                    user.CreatedAt = DateTime.UtcNow;
                }

                if (user.UpdatedAt == null)
                {
                    user.UpdatedAt = DateTime.UtcNow;
                }

                // Save changes to DB
                await userManager.UpdateAsync(user);

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
                    Roles = roles,
                    CreatedAt = user.CreatedAt,
                    UpdatedAt = user.UpdatedAt
                });
            }).WithSummary("Creates roles")
            .WithDescription("Creates roles as if role is not set to be 'administrator' it set to default => 'user'");

            //enpoint to get all users ID and emails
            app.MapGet("/users/info",
            [Authorize(Roles = "Administrator")]
            async(UserManager<User> manager) =>
            {
                try
                {
                    var users = await manager.Users.ToListAsync();

                    var userInfos = users.Select(info => new
                    {
                        ID = info.Id,
                        Email = info.Email,
                        CreatedAT = info.CreatedAt,
                        UpdatedAT = info.UpdatedAt,
					}).ToList();

                    return Results.Ok(userInfos);
                }
                catch (Exception ex)
                {
                    return Results.Problem("Error message: " + ex);
                }
            }).WithSummary("Gets users info")
            .WithDescription("Returns the ID, Email, Created at date and Updated At date for every user despite their role");

            //enpoint-admin makes other users administrators
            app.MapPost("/users/make-admin/{id}",
            [Authorize(Roles = "Administrator")]
            async (UserManager<User> userManager, string id) =>
            {
                try
                {
                    var users = await userManager.Users.ToListAsync();

                    var user_to_admin = users.FirstOrDefault(users => users.Id.Equals(id));

                    if (user_to_admin == null)
                    {
                        return Results.NotFound("User not found");
                    }

                    await userManager.RemoveFromRoleAsync(user_to_admin, "User"); // Remove default role if exists
					var result = await userManager.AddToRoleAsync(user_to_admin, "Administrator");

                    if (result.Succeeded)
                    {
                        return Results.Ok("User has been made an administrator.");
                    }
                    else
                    {
						await userManager.AddToRoleAsync(user_to_admin, "User"); // Ensure the user has a default role
						return Results.BadRequest("Failed to make user an administrator.");
					}

                }
                catch (Exception ex)
                {
                    return Results.Problem("Error message: " + ex);
                }
			}).WithSummary("Admin adds new admins")
            .WithDescription("Only Amins can add new admins as it selects them by ID");

            //endpoint-admin removes other admins from administrators
            app.MapDelete("/users/remove-admin/{id}",
            [Authorize(Roles = "Administrator")]
            async (UserManager<User> manager, string id) =>
            {
                var users = await manager.Users.ToListAsync();

                var user_to_remove = users.FirstOrDefault(users => users.Id.Equals(id));

                if (user_to_remove == null)
                {
                    return Results.NotFound("User not found");
				}

				var result = await manager.RemoveFromRoleAsync(user_to_remove!, "Administrator");

                if (result.Succeeded)
                {
                    await manager.AddToRoleAsync(user_to_remove!, "User"); // Ensure the user has a default role
					return Results.Ok("User has been removed from administrators.");
                }
                else
                {
                    await manager.AddToRoleAsync(user_to_remove!, "Aministrator"); // Ensure the user has his role back
					return Results.BadRequest("Failed to remove user from administrators.");
				}
			}).WithSummary("Removes admin")
            .WithDescription("Only admins remove other admins which are selected by ID as once the admin role is removed the user gets the role 'user'");
		}
	}
}
