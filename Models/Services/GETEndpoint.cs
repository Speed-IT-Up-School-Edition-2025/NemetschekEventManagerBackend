using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using System;
public static class GETEndpoint
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
        // GET all submissions endpoint

        // returns a list of all Submit records in the database
        app.MapGet("/api/submits", async (EventDbContext db) =>
        {
            // Fetch all submissions from the database asynchronously
            var submissions = await db.Submits.ToListAsync();

            // Return HTTP 200 OK with the list of submissions
            return Results.Ok(submissions);
        });

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
        });
    }
}
