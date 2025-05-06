using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.DTO;
using WebApplication1.Models;
using WebApplication1.Utilities;

public static class StudentEndpoints
{
    public static void MapStudentEndpoints(this IEndpointRouteBuilder routes)
    {
        routes.MapGet("/students", async (AppDbContext db) =>
            await db.Students.Include(s => s.Member)
                             .Include(s => s.Parents)
                             .ToListAsync());

        routes.MapGet("/students/{id}", async (int id, AppDbContext db) =>
            await db.Students.Include(s => s.Member)
                             .Include(s => s.Parents)
                             .FirstOrDefaultAsync(s => s.Id == id)
                is Student student
                    ? Results.Ok(student)
                    : Results.NotFound());

        routes.MapPost("/students", async ([FromBody] StudentCreationDto dto, AppDbContext db) =>
        {
            var member = new Member
            {
                Surname = dto.Member.Surname,
                Name = dto.Member.Name,
                Phone = dto.Member.Phone,
                Email = dto.Member.Email,
                PasswordHash = Convert.ToBase64String(SecurityHelper.HashPassword(dto.Member.PlainPassword)),
                Role = "Student",
                Events = new List<Event>()
            };

            var student = new Student
            {
                BirthDay = dto.BirthDay,
                GroupId = dto.GroupId,
                Member = member
            };

            db.Students.Add(student);
            await db.SaveChangesAsync();
            return Results.Created($"/students/{student.Id}", student);
        });

        // Update student including their Member and relationships with parents
        routes.MapPut("/students/{id}", async (int id, [FromBody] StudentUpdateDto dto, AppDbContext db) =>
        {
            var student = await db.Students
                                  .Include(s => s.Member)
                                  .Include(s => s.Parents)
                                  .FirstOrDefaultAsync(s => s.Id == id);
            if (student is null)
                return Results.NotFound();

            if (dto.BirthDay != default)
                student.BirthDay = dto.BirthDay;
            // Update GroupId if a value is provided (if GroupId is nullable, you can check for null)
            if (dto.GroupId is not null)
                student.GroupId = dto.GroupId;

            if (dto.Member is not null)
            {
                if (!string.IsNullOrWhiteSpace(dto.Member.Surname))
                    student.Member.Surname = dto.Member.Surname;
                if (!string.IsNullOrWhiteSpace(dto.Member.Name))
                    student.Member.Name = dto.Member.Name;
                if (!string.IsNullOrWhiteSpace(dto.Member.Phone))
                    student.Member.Phone = dto.Member.Phone;
                if (!string.IsNullOrWhiteSpace(dto.Member.Email))
                    student.Member.Email = dto.Member.Email;
                if (!string.IsNullOrWhiteSpace(dto.Member.PlainPassword))
                    student.Member.PasswordHash = Convert.ToBase64String(SecurityHelper.HashPassword(dto.Member.PlainPassword));
            }

            if (dto.ParentIds is not null)
            {
                // Clear existing relationships and add new ones
                student.Parents.Clear();
                if (dto.ParentIds.Any())
                {
                    var parents = await db.Parents
                                            .Where(p => dto.ParentIds.Contains(p.Id))
                                            .ToListAsync();
                    foreach (var parent in parents)
                    {
                        student.Parents.Add(parent);
                    }
                }
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        routes.MapDelete("/students/{id}", async (int id, AppDbContext db) =>
        {
            var student = await db.Students.FindAsync(id);
            if (student is null)
                return Results.NotFound();

            db.Students.Remove(student);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
