using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using WebApplication1.Models;

namespace WebApplication1.endpoint
{
    public static class EventEndpoints
    {
        public static void MapEventEndpoints(this WebApplication app)
        {
            // Get all events
            app.MapGet("/api/events", async (AppDbContext db) =>
                await db.Events
                    .Include(e => e.EventType)
                    .Include(e => e.Members)
                    .Include(e => e.Task)
                    .ToListAsync());

            // Get event by id
            app.MapGet("/api/events/{id}", async (int id, AppDbContext db) =>
                await db.Events
                    .Include(e => e.EventType)
                    .Include(e => e.Members)
                    .Include(e => e.Task)
                    .FirstOrDefaultAsync(e => e.Id == id)
                    is Event ev
                    ? Results.Ok(ev)
                    : Results.NotFound());

            // Create an event using a DTO
            app.MapPost("/api/events", async ([FromBody] EventCreationDto dto, AppDbContext db) =>
            {
                var ev = new Event
                {
                    EventTypeId = dto.EventTypeId,
                    Name = dto.Name,
                    DateOfStart = dto.DateOfStart,
                    DurationInMinutes = dto.DurationInMinutes,
                    Content = dto.Content,
                    VenueOrLink = dto.VenueOrLink,
                    Members = new List<Member>()
                };

                db.Events.Add(ev);
                await db.SaveChangesAsync();
                return Results.Created($"/api/events/{ev.Id}", ev);
            });

           // Update event using a DTO
            app.MapPut("/api/events/{id}", async (int id, [FromBody] EventUpdateDto dto, AppDbContext db) =>
            {
                var ev = await db.Events.FindAsync(id);
                if (ev is null) return Results.NotFound();

                // If the field is not empty or not equal to the default value, we update
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    ev.Name = dto.Name;
                if (dto.EventTypeId != default)
                    ev.EventTypeId = dto.EventTypeId;
                if (dto.DateOfStart != default)  // Default (Datetime) == 01.01.0001, can be interpreted as "not transmitted"
                    ev.DateOfStart = dto.DateOfStart;
                if (dto.DurationInMinutes != default)
                    ev.DurationInMinutes = dto.DurationInMinutes;
                if (dto.Content is not null)
                    ev.Content = dto.Content;
                if (dto.VenueOrLink is not null)
                    ev.VenueOrLink = dto.VenueOrLink;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });


            // Delete an event
            app.MapDelete("/api/events/{id}", async (int id, AppDbContext db) =>
            {
                var ev = await db.Events.FindAsync(id);
                if (ev is null) return Results.NotFound();

                db.Events.Remove(ev);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // -------------------------------------------
            // Endpoints to manage a Task as a child element of an Event
            // All operations are performed at the route /api/events/{eventId}/task
            // -------------------------------------------------------------------------------------

            // GET: Get the task associated with the event
            app.MapGet("/api/events/{eventId}/task", async (int eventId, AppDbContext db) =>
            {
                var ev = await db.Events
                    .Include(e => e.Task)
                    .FirstOrDefaultAsync(e => e.Id == eventId);
                if (ev is null) return Results.NotFound($"Event {eventId} not found");

                return ev.Task is not null
                    ? Results.Ok(ev.Task)
                    : Results.NotFound($"Task for event {eventId} not found");
            });

            // POST: Create a task for the event, if it doesn't already exist
            app.MapPost("/api/events/{eventId}/task", async (int eventId, [FromBody] TaskDto dto, AppDbContext db) =>
            {
                var ev = await db.Events.Include(e => e.Task)
                                        .FirstOrDefaultAsync(e => e.Id == eventId);
                if (ev is null) return Results.NotFound($"Event {eventId} not found");

                if (ev.Task is not null)
                    return Results.BadRequest("Task already exists for this event. Use PUT to update.");

                // Create a new task and establish a relationship
                var task = new Models.Task
                {
                    Id = eventId, // By agreement, the key key coincides with the event key
                    Name = dto.Name,
                    DateOfEnding = dto.DateOfEnding,
                    Content = dto.Content,
                    Event = ev
                };
                ev.Task = task;
                db.Tasks.Add(task);
                await db.SaveChangesAsync();
                return Results.Created($"/api/events/{eventId}/task", task);
            });

            // PUT: Update the task for the event using a DTO
            app.MapPut("/api/events/{eventId}/task", async (int eventId, [FromBody] TaskDto dto, AppDbContext db) =>
            {
                var ev = await db.Events
                    .Include(e => e.Task)
                    .FirstOrDefaultAsync(e => e.Id == eventId);
                if (ev is null) return Results.NotFound($"Event {eventId} not found");

                if (ev.Task is null)
                    return Results.NotFound("Task not found for this event.");

                if (!string.IsNullOrWhiteSpace(dto.Name))
                    ev.Task.Name = dto.Name;
                if (dto.DateOfEnding != default)
                    ev.Task.DateOfEnding = dto.DateOfEnding;
                if (dto.Content is not null)
                    ev.Task.Content = dto.Content;

                await db.SaveChangesAsync();
                return Results.NoContent();
            });

            // DELETE: Delete the task associated with the event
            app.MapDelete("/api/events/{eventId}/task", async (int eventId, AppDbContext db) =>
            {
                var ev = await db.Events
                    .Include(e => e.Task)
                    .FirstOrDefaultAsync(e => e.Id == eventId);
                if (ev is null) return Results.NotFound($"Event {eventId} not found");

                if (ev.Task is null) return Results.NotFound("Task not found for this event.");

                db.Tasks.Remove(ev.Task);
                ev.Task = null;
                await db.SaveChangesAsync();
                return Results.Ok();
            });
        }
    }
}
