using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Utilities;

public static class ParentEndpoints
{
    public static void MapParentEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/parents", async (AppDbContext db) =>
            await db.Parents.Include(p => p.Member)
                            .Include(p => p.Students)
                            .ToListAsync());

        routes.MapGet("/parents/{id}", async (int id, AppDbContext db) =>
            await db.Parents.Include(p => p.Member)
                            .Include(p => p.Students)
                            .FirstOrDefaultAsync(p => p.Id == id)
                is Parent parent
                    ? Results.Ok(parent)
                    : Results.NotFound());

        routes.MapPost("/parents", async ([FromBody] ParentCreationDto dto, AppDbContext db) =>
        {
            var member = new Member
            {
                Surname = dto.Member.Surname,
                Name = dto.Member.Name,
                Phone = dto.Member.Phone,
                Email = dto.Member.Email,
                Password = SecurityHelper.HashPassword(dto.Member.PlainPassword),
                Events = new List<Event>()
            };

            var parent = new Parent
            {
                ParentTypeId = dto.ParentTypeId,
                Member = member
            };

            if (dto.StudentIds is not null && dto.StudentIds.Any())
            {
                var students = await db.Students
                                         .Where(s => dto.StudentIds.Contains(s.Id))
                                         .ToListAsync();
                foreach (var student in students)
                {
                    parent.Students.Add(student);
                }
            }

            db.Parents.Add(parent);
            await db.SaveChangesAsync();
            return Results.Created($"/parents/{parent.Id}", parent);
        });

        // Обновляем parent с изменением его Member и связей со студентами
        routes.MapPut("/parents/{id}", async (int id, [FromBody] ParentUpdateDto dto, AppDbContext db) =>
        {
            var parent = await db.Parents
                                 .Include(p => p.Member)
                                 .Include(p => p.Students)
                                 .FirstOrDefaultAsync(p => p.Id == id);
            if (parent is null)
                return Results.NotFound();

            if (dto.ParentTypeId != default)
                parent.ParentTypeId = dto.ParentTypeId;

            // Обновляем данные Member, если они переданы
            if (dto.Member is not null)
            {
                if (!string.IsNullOrWhiteSpace(dto.Member.Surname))
                    parent.Member.Surname = dto.Member.Surname;
                if (!string.IsNullOrWhiteSpace(dto.Member.Name))
                    parent.Member.Name = dto.Member.Name;
                if (!string.IsNullOrWhiteSpace(dto.Member.Phone))
                    parent.Member.Phone = dto.Member.Phone;
                if (!string.IsNullOrWhiteSpace(dto.Member.Email))
                    parent.Member.Email = dto.Member.Email;
                if (!string.IsNullOrWhiteSpace(dto.Member.PlainPassword))
                    parent.Member.Password = SecurityHelper.HashPassword(dto.Member.PlainPassword);
            }

            // Обновляем связи со студентами, только если список передан
            if (dto.StudentIds is not null)
            {
                // Очищаем связи и добавляем новые
                parent.Students.Clear();
                if (dto.StudentIds.Any())
                {
                    var students = await db.Students
                                             .Where(s => dto.StudentIds.Contains(s.Id))
                                             .ToListAsync();
                    foreach (var student in students)
                    {
                        parent.Students.Add(student);
                    }
                }
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        routes.MapDelete("/parents/{id}", async (int id, AppDbContext db) =>
        {
            var parent = await db.Parents.FindAsync(id);
            if (parent is null)
                return Results.NotFound();

            db.Parents.Remove(parent);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
