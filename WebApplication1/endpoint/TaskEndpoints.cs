using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using WebApplication1.DTO;
using WebApplication1.Models;
namespace WebApplication1.endpoint
{
    public class TaskEndpoints
    {
        public static void MapTaskEndpoints(this WebApplication app)
        {
            // Отримання всіх завдань з фільтрацією
            app.MapGet("/api/tasks", async (
                AppDbContext db,
                HttpContext http,
                bool? completed = null,
                DateTime? startDate = null,
                DateTime? endDate = null,
                string? search = null) =>
            {
                var userId = int.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = http.User.FindFirst("role")?.Value;

                var query = db.Tasks
                    .Include(t => t.Event)
                    .AsQueryable();

                // Фільтрація за статусом виконання
                if (completed.HasValue)
                    query = query.Where(t => t.IsCompleted == completed.Value);

                // Фільтрація за датою закінчення
                if (startDate.HasValue)
                    query = query.Where(t => t.DateOfEnding >= startDate.Value);
                if (endDate.HasValue)
                    query = query.Where(t => t.DateOfEnding <= endDate.Value);

                // Фільтрація за пошуком
                if (!string.IsNullOrEmpty(search))
                    query = query.Where(t => t.Name.Contains(search) || t.Content.Contains(search));

                // Фільтрація за доступом
                if (userRole != "teacher")
                {
                    query = query.Where(t => t.Event.CreatorId == userId || t.Event.Members.Any(m => m.Id == userId));
                }

                var tasks = await query
                    .OrderBy(t => t.DateOfEnding)
                    .Select(t => new TaskDto
                    {
                        Id = t.Id,
                        Name = t.Name,
                        DateOfEnding = t.DateOfEnding,
                        Content = t.Content,
                        IsCompleted = t.IsCompleted,
                        CompletedAt = t.CompletedAt,
                        CreatedAt = t.CreatedAt,
                        UpdatedAt = t.UpdatedAt
                    })
                    .ToListAsync();

                return Results.Ok(tasks);
            }).RequireAuthorization();

            // Отримання конкретного завдання
            app.MapGet("/api/tasks/{id}", async (int id, AppDbContext db, HttpContext http) =>
            {
                var userId = int.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = http.User.FindFirst("role")?.Value;

                var task = await db.Tasks
                    .Include(t => t.Event)
                        .ThenInclude(e => e.Members)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                    return Results.NotFound();

                // Перевірка доступу
                if (userRole != "teacher" && task.Event.CreatorId != userId && !task.Event.Members.Any(m => m.Id == userId))
                    return Results.Forbid();

                var result = new TaskDto
                {
                    Id = task.Id,
                    Name = task.Name,
                    DateOfEnding = task.DateOfEnding,
                    Content = task.Content,
                    IsCompleted = task.IsCompleted,
                    CompletedAt = task.CompletedAt,
                    CreatedAt = task.CreatedAt,
                    UpdatedAt = task.UpdatedAt
                };

                return Results.Ok(result);
            }).RequireAuthorization();

            // Оновлення завдання
            app.MapPut("/api/tasks/{id}", async (int id, UpdateTaskDto dto, AppDbContext db, HttpContext http) =>
            {
                var userId = int.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = http.User.FindFirst("role")?.Value;

                var task = await db.Tasks
                    .Include(t => t.Event)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                    return Results.NotFound();

                // Перевірка прав на редагування
                if (userRole != "teacher" && task.Event.CreatorId != userId)
                    return Results.Forbid();

                // Оновлення полів
                if (dto.Name != null)
                    task.Name = dto.Name;
                if (dto.DateOfEnding.HasValue)
                    task.DateOfEnding = dto.DateOfEnding.Value;
                if (dto.Content != null)
                    task.Content = dto.Content;

                task.UpdatedAt = DateTime.UtcNow;
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();

            // Позначення завдання як виконане/невиконане
            app.MapPost("/api/tasks/{id}/completion", async (int id, TaskCompletionDto dto, AppDbContext db, HttpContext http) =>
            {
                var userId = int.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

                var task = await db.Tasks
                    .Include(t => t.Event)
                        .ThenInclude(e => e.Members)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                    return Results.NotFound();

                // Перевірка чи користувач має доступ до завдання
                if (!task.Event.Members.Any(m => m.Id == userId) && task.Event.CreatorId != userId)
                    return Results.Forbid();

                task.IsCompleted = dto.IsCompleted;
                task.CompletedAt = dto.IsCompleted ? DateTime.UtcNow : null;
                task.UpdatedAt = DateTime.UtcNow;

                await db.SaveChangesAsync();
                return Results.Ok();
            }).RequireAuthorization();

            // Видалення завдання
            app.MapDelete("/api/tasks/{id}", async (int id, AppDbContext db, HttpContext http) =>
            {
                var userId = int.Parse(http.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");
                var userRole = http.User.FindFirst("role")?.Value;

                var task = await db.Tasks
                    .Include(t => t.Event)
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (task == null)
                    return Results.NotFound();

                // Перевірка прав на видалення
                if (userRole != "teacher" && task.Event.CreatorId != userId)
                    return Results.Forbid();

                db.Tasks.Remove(task);
                await db.SaveChangesAsync();

                return Results.NoContent();
            }).RequireAuthorization();
        }
    }
}
