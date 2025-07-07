namespace NemetschekEventManagerBackend.Models.Services
{
     public static class DELETEEndpoint
    {
        public static void MapDeleteEndpoint(WebApplication app)
        {
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
            });
        }
    }
}
