using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NemetschekEventManagerBackend;
using NemetschekEventManagerBackend.Models;
using NemetschekEventManagerBackend.Models.JSON;
using System;

public static class POSTEndpoint
{
    public static void MapTodoEndpoints(this WebApplication app)
    {
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
        });
    }
}
