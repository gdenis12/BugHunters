using Microsoft.AspNetCore.Identity;
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
                PasswordHash = Convert.ToBase64String(SecurityHelper.HashPassword(dto.Member.PlainPassword)),
                Role = "Parent",
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

        // Update parent including their Member and relationships with students
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

            // Update Member data if provided
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
                    parent.Member.PasswordHash = Convert.ToBase64String(SecurityHelper.HashPassword(dto.Member.PlainPassword));
            }

            // Update relationships with students only if the list is provided
            if (dto.StudentIds is not null)
            {
                // Clear existing relationships and add new ones
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
