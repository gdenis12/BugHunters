using WebApplication1.Models;
using WebApplication1;
using Microsoft.EntityFrameworkCore;

public static class EventTypeEndpoints
{
    public static void MapEventTypeEndpoints(this WebApplication app)
    {
        app.MapGet("/api/eventtypes", async (AppDbContext db) =>
            await db.EventTypes.ToListAsync());

        app.MapGet("/api/eventtypes/{id}", async (int id, AppDbContext db) =>
            await db.EventTypes.FindAsync(id) is EventType eventType
            ? Results.Ok(eventType)
            : Results.NotFound());

        app.MapPost("/api/eventtypes", async (EventType eventType, AppDbContext db) =>
        {
            db.EventTypes.Add(eventType);
            await db.SaveChangesAsync();
            return Results.Created($"/api/eventtypes/{eventType.Id}", eventType);
        });

        app.MapPut("/api/eventtypes/{id}", async (int id, EventType updated, AppDbContext db) =>
        {
            var eventType = await db.EventTypes.FindAsync(id);
            if (eventType is null) return Results.NotFound();

            eventType.EventTypeName = updated.EventTypeName;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapDelete("/api/eventtypes/{id}", async (int id, AppDbContext db) =>
        {
            var eventType = await db.EventTypes.FindAsync(id);
            if (eventType is null) return Results.NotFound();

            db.EventTypes.Remove(eventType);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
