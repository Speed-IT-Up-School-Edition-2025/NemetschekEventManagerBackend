using NemetschekEventManagerBackend.Models.JSON;
using Microsoft.EntityFrameworkCore;

namespace NemetschekEventManagerBackend.Models.Services
{

    public static class PUTEndpoint
    {
        public static void MapTodoEndpoints(this WebApplication app)
        {
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
            });
        }
    }

}
