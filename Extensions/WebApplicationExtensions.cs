using ClosedXML.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend.Interfaces;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.DTOs;
using System.Diagnostics;
using System.Security.Claims;
using System.Text;

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

            //Create new event
            app.MapPost("/events",
            [Authorize(Roles = "Administrator")]
            (IEventService service, CreateEventDto dto) =>
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

            app.MapPut("/events/{id}",
            [Authorize(Roles = "Administrator")]
            (IEventService service, int id, UpdateEventDto dto) =>
            {
                var success = service.Update(id, dto);
                return success ? Results.Ok() : Results.NotFound();
            })
            .WithSummary("Update event by ID")
            .WithDescription("Updates an existing event using its ID and provided details. Returns 404 if not found.");


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


            // GET submissions by eventId
            app.MapGet("/submits/{eventId}",
            [Authorize]
            (ISubmitService service, int eventId) =>
            {
                var submits = service.GetSubmitsByEventId(eventId);
                return submits.Count == 0
                    ? Results.NotFound()
                    : Results.Ok(submits);
            })
            .WithSummary("Get submit for authenticated user")
            .WithDescription("Fetches a submission for the authenticated user by event ID. Returns 404 if the submission does not exist.");

            // Create new submit for authenticated user
            app.MapPost("/submits/{eventId}",
            [Authorize]
            (int eventId, CreateSubmitDto dto, ISubmitService service, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var created = service.Create(eventId, userId, dto);
                return created
                    ? Results.Created($"/submits/{eventId}", dto)
                    : Results.Conflict("A submission already exists for this user and event.");
            })
            .WithSummary("Create new submit for authenticated user")
            .WithDescription("Creates a new submit record for the authenticated user. Returns 409 if one already exists.");

            // PUT endpoint
            app.MapPut("/submits/{eventId}",
            [Authorize]
            (int eventId, UpdateSubmitDto dto, ISubmitService service, ClaimsPrincipal user) =>
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
            .WithDescription("Updates all submissions for the authenticated user in the specified event. Returns 404 if not found.");

            // Remove user submission from event
            app.MapDelete("/submits/{eventId}",
            [Authorize]
            (int eventId, ISubmitService service, ClaimsPrincipal user) =>
            {
                var userId = user.FindFirstValue(ClaimTypes.NameIdentifier) ?? user.FindFirstValue("sub");
                if (string.IsNullOrEmpty(userId))
                    return Results.Unauthorized();

                var success = service.RemoveUserFromEvent(eventId, userId);
                return success ? Results.Ok() : Results.NotFound();
            })
            .WithSummary("Remove current user's submission from event")
            .WithDescription("Removes the current user's submission from the specified event.");

            //Admin delete
            app.MapDelete("/submissions/{eventId}/{userId}",
            [Authorize(Roles = "Administrator")]
            (int eventId, string userId, ISubmitService service, ClaimsPrincipal user) =>
            {
                var success = service.RemoveUserFromEvent(eventId, userId);
                return success ? Results.Ok() : Results.NotFound();
            })
            .WithSummary("Remove user submission from event by admin")
            .WithDescription("Allows an admin to remove a user's submission from a specific event by providing the event ID and user ID.");

            // User me
            app.MapGet("/users/me",
            [Authorize]
            async 
            (HttpContext httpContext, 
            UserManager<User> userManager, 
            RoleManager<IdentityRole> roleManager) =>
            {
                var principal = httpContext.User;

                var userId = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;

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
            }).WithSummary("Gets information for the current user")
            .WithDescription("Gives UserID, Email, Role, Created At date and Updated At date. If the user doesn't have a role, it assigns the role \"User\".");

            //enpoint to get all users
            app.MapGet("/users/info",
            [Authorize(Roles = "Administrator")]
            async(UserManager<User> manager) =>
            {
                try
                {
                    var users = await manager.Users.ToListAsync();

                    var userInfos = new List<object>();

                    foreach (var user in users)
                    {
                        var roles = await manager.GetRolesAsync(user);

                        userInfos.Add(new
                        {
                            ID = user.Id,
                            Email = user.Email,
                            Roles = roles,
                            CreatedAT = user.CreatedAt,
                            UpdatedAT = user.UpdatedAt
                        });
                    }

                    return Results.Ok(userInfos);
                }
                catch (Exception ex)
                {
                    return Results.Problem("Error message: " + ex);
                }
            }).WithSummary("Gets a user list.")
            .WithDescription("Returns the ID, Email, Created at date and Updated At date for every user.");

            //enpoint-admin makes other users administrators
            app.MapPost("/users/admin/{id}",
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
			}).WithSummary("Admin adds new admins.")
            .WithDescription("Only Amins can add new admins as it selects them by ID.");

            //endpoint-admin removes other admins from administrators
            app.MapDelete("/users/admin/{id}",
            [Authorize(Roles = "Administrator")]
            async (UserManager<User> manager, string id) =>
            {
                var users = await manager.Users.ToListAsync();

                var user_to_remove = users.FirstOrDefault(users => users.Id.Equals(id));

                if (user_to_remove == null)
                {
                    return Results.NotFound("User not found");
                }

                const string seededAdminEmail = "admin@example.com";

                if (user_to_remove.Email == seededAdminEmail)
                {
                    return Results.BadRequest("Cannot remove the original administrator.");
                }

                var result = await manager.RemoveFromRoleAsync(user_to_remove!, "Administrator");

                        if (result.Succeeded)
                        {
                            await manager.AddToRoleAsync(user_to_remove!, "User"); // Ensure the user has a default role
                            return Results.Ok("User has been removed from administrators.");
                        }
                        else
                        {
                            await manager.AddToRoleAsync(user_to_remove!, "Administrator"); // Ensure the user has his role back
                  return Results.BadRequest("Failed to remove user from administrators.");
                }
            }).WithSummary("Removes admin.")
            .WithDescription("Only admins remove other admins which are selected by ID as once the admin role is removed the user gets the role 'User'.");

            // Export submits of a certain event as a .csv file
            app.MapGet("/csv/{eventId}",
            [Authorize(Roles = "Administrator")]
            (HttpContext httpContext,
            int eventId,
            ISubmitService submitService,
            IEventService eventService) =>
            {
                if (!eventService.Exists(eventId))
                {
                    return Results.NotFound($"Event was not found.");
                }

                var data = submitService.GetSubmitsByEventId(eventId);

                // Get all unique submission names (header columns)
                var submitNames = data
                    .SelectMany(d => d.Submissions ?? [])
                    .Select(s => s.Name ?? "")
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                var csvBuilder = new StringBuilder();

                // Header
                csvBuilder.Append("Date");
                foreach (var name in submitNames)
                {
                    csvBuilder.Append($",\"{name.Replace("\"", "\"\"")}\"");
                    Debug.WriteLine(name);
                }
                csvBuilder.AppendLine();

                // Rows
                foreach (var summary in data)
                {
                    var date = summary.Date?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";
                    csvBuilder.Append($"\"{date}\"");

                    var submissionsDict = (summary.Submissions ?? []).ToDictionary(
                        s => s.Name ?? "",
                        s => s.Options != null ? string.Join("; \n", s.Options.Select(o => o.Replace("\"", "\"\""))) : ""
                    );

                    foreach (var name in submitNames)
                    {
                        if (submissionsDict.TryGetValue(name, out var options))
                            csvBuilder.Append($",\"{options}\"");
                        else
                            csvBuilder.Append(","); // empty if not answered
                    }

                    csvBuilder.AppendLine();
                }

                var utf8WithBom = new UTF8Encoding(encoderShouldEmitUTF8Identifier: true);
                var csvString = csvBuilder.ToString();
                var csvBytes = utf8WithBom.GetBytes(csvString);

                httpContext.Response.Headers.ContentType = "text/csv; charset=utf-8";
                httpContext.Response.Headers.ContentDisposition = "attachment; filename=\"submissions.csv\"";

                return Results.File(csvBytes, "text/csv; charset=utf-8");

            }).WithSummary("Exports all submits for a certain event in .csv file.")
            .WithDescription("Exports all submits for a certain event in .csv file. If the event doesn't exist it return code 404.");

            // Export submits of a event as a .xlsx file

            app.MapGet("/xlsx/{eventId}",
            [Authorize(Roles = "Administrator")]
            (HttpContext httpContext,
            int eventId,
            ISubmitService submitService,
            IEventService eventService) =>
            {
                if(!eventService.Exists(eventId))
                {
                    return Results.NotFound($"Event was not found.");
                }

                var data = submitService.GetSubmitsByEventId(eventId);

                // Get all unique submission names (columns)
                var submitNames = data
                    .SelectMany(d => d.Submissions ?? [])
                    .Select(s => s.Name ?? "")
                    .Distinct()
                    .OrderBy(name => name)
                    .ToList();

                using var workbook = new XLWorkbook();
                var worksheet = workbook.Worksheets.Add("Submissions");

                // Header row
                worksheet.Cell(1, 1).Value = "Date";
                for (int i = 0; i < submitNames.Count; i++)
                {
                    worksheet.Cell(1, i + 2).Value = submitNames[i];
                }

                int totalColumns = submitNames.Count + 1;
                var headerRange = worksheet.Range(1, 1, 1, totalColumns);
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightBlue;
                headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
                headerRange.Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

                // Data rows
                int row = 2;
                foreach (var summary in data)
                {
                    worksheet.Cell(row, 1).Value = summary.Date?.ToString("yyyy-MM-dd HH:mm:ss") ?? "";

                    var submissionsDict = (summary.Submissions ?? []).ToDictionary(
                    s => s.Name ?? "",
                    s => s.Options != null ? string.Join(Environment.NewLine, s.Options) : ""
);

                    for (int i = 0; i < submitNames.Count; i++)
                    {
                        var name = submitNames[i];
                        if (submissionsDict.TryGetValue(name, out var value))
                        {
                            var cell = worksheet.Cell(row, i + 2);
                            cell.Value = value;
                            cell.Style.Alignment.WrapText = true;
                        }
                        else
                        {
                            worksheet.Cell(row, i + 2).Value = "";
                        }
                    }

                    row++;
                }

                worksheet.Columns().AdjustToContents();

                // Apply borders to the whole table
                var usedRange = worksheet.RangeUsed();
                usedRange.Style.Border.OutsideBorder = XLBorderStyleValues.Thin;
                usedRange.Style.Border.InsideBorder = XLBorderStyleValues.Thin;

                using var stream = new MemoryStream();
                workbook.SaveAs(stream);
                stream.Seek(0, SeekOrigin.Begin);

                return Results.File(stream.ToArray(),
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    "submissions.xlsx");

            }).WithSummary("Exports all submits for a certain event in .xlsx file.")
            .WithDescription("Exports all submits for a certain event in .xlsx file. If the event doesn't exist it return code 404.");
        }
    }
}
