namespace NemetschekEventManagerBackend.Models.Services
{
    public class PUTEndpoint
    {
        public static void MapTodoEndpoints(this WebApplication app)
        {
            // PUT endpoint
            app.MapPut("/todoitems/{id}", async (int id, Sub todoToUpdate, TodoDb db) =>
            {
                var todo = await db.Todos.FindAsync(id);

                if (todo is null)
                    return Results.NotFound();

                todo.Name = todoToUpdate.Name;
                todo.IsComplete = todoToUpdate.IsComplete;

                await db.SaveChangesAsync();

                return Results.NoContent();
            });
        }
}
