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
            // Получить все события
            app.MapGet("/api/events", async (AppDbContext db) =>
                await db.Events
                    .Include(e => e.EventType)
                    .Include(e => e.Members)
                    .Include(e => e.Task)
                    .ToListAsync());

            // Получить событие по id
            app.MapGet("/api/events/{id}", async (int id, AppDbContext db) =>
                await db.Events
                    .Include(e => e.EventType)
                    .Include(e => e.Members)
                    .Include(e => e.Task)
                    .FirstOrDefaultAsync(e => e.Id == id)
                    is Event ev
                    ? Results.Ok(ev)
                    : Results.NotFound());

            // Создать событие через DTO
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

            // Обновить событие через DTO
            app.MapPut("/api/events/{id}", async (int id, [FromBody] EventUpdateDto dto, AppDbContext db) =>
            {
                var ev = await db.Events.FindAsync(id);
                if (ev is null) return Results.NotFound();

                // Если поле не пустое или не равно значению по умолчанию, обновляем
                if (!string.IsNullOrWhiteSpace(dto.Name))
                    ev.Name = dto.Name;
                if (dto.EventTypeId != default)
                    ev.EventTypeId = dto.EventTypeId;
                if (dto.DateOfStart != default)  // default(DateTime) == 01.01.0001, можно трактовать как «не передано»
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


            // Удалить событие
            app.MapDelete("/api/events/{id}", async (int id, AppDbContext db) =>
            {
                var ev = await db.Events.FindAsync(id);
                if (ev is null) return Results.NotFound();

                db.Events.Remove(ev);
                await db.SaveChangesAsync();
                return Results.Ok();
            });

            // -------------------------------------------
            // Эндпоинты для управления задачей (Task) как дочерним элементом события
            // Все операции выполняются по маршруту /api/events/{eventId}/task
            // -------------------------------------------

            // GET: Получить задачу, связанную с событием
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

            // POST: Создать задачу для события, если её ещё нет
            app.MapPost("/api/events/{eventId}/task", async (int eventId, [FromBody] TaskDto dto, AppDbContext db) =>
            {
                var ev = await db.Events.Include(e => e.Task)
                                        .FirstOrDefaultAsync(e => e.Id == eventId);
                if (ev is null) return Results.NotFound($"Event {eventId} not found");

                if (ev.Task is not null)
                    return Results.BadRequest("Task already exists for this event. Use PUT to update.");

                // Создаём новую задачу и устанавливаем связь
                var task = new Models.Task
                {
                    Id = eventId, // По соглашению ключ задачи совпадает с ключом события
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

            // PUT: Обновить задачу для события через DTO
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

            // DELETE: Удалить задачу, связанную с событием
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
