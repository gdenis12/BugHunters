using WebApplication1.Models;
using WebApplication1;
using Microsoft.EntityFrameworkCore;
using WebApplication1.DTO;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using WebApplication1.Utilities;

public static class TeacherEndpoints
{
    public static void MapTeacherEndpoints(this WebApplication app)
    {
        app.MapGet("/api/teachers", async (AppDbContext db) =>
            await db.Teachers
                .Include(t => t.Member)
                .ToListAsync());

        app.MapGet("/api/teachers/{id}", async (int id, AppDbContext db) =>
            await db.Teachers
                .Include(t => t.Member)
                .FirstOrDefaultAsync(t => t.Id == id) is Teacher teacher
                ? Results.Ok(teacher)
                : Results.NotFound());

        app.MapPost("/api/teachers", async ([FromBody] TeacherCreationDto dto, AppDbContext db) =>
        {
            // Create a Member entity and hash the password
            var member = new Member
            {
                Surname = dto.Member.Surname,
                Name = dto.Member.Name,
                Phone = dto.Member.Phone,
                Email = dto.Member.Email,
                Password = SecurityHelper.HashPassword(dto.Member.PlainPassword),
                Events = new List<Event>()
            };

            var teacher = new Teacher
            {
                Member = member
            };

            db.Teachers.Add(teacher);
            await db.SaveChangesAsync();
            return Results.Created($"/api/teachers/{teacher.Id}", teacher);
        });

        app.MapPut("/api/teachers/{id}", async (int id, [FromBody] Teacher updated, AppDbContext db) =>
        {
            var teacher = await db.Teachers
                                   .Include(t => t.Member)
                                   .FirstOrDefaultAsync(t => t.Id == id);
            if (teacher is null)
                return Results.NotFound();

            if (!string.IsNullOrWhiteSpace(updated.Member.Name))
                teacher.Member.Name = updated.Member.Name;
            if (!string.IsNullOrWhiteSpace(updated.Member.Surname))
                teacher.Member.Surname = updated.Member.Surname;
            if (!string.IsNullOrWhiteSpace(updated.Member.Phone))
                teacher.Member.Phone = updated.Member.Phone;
            if (!string.IsNullOrWhiteSpace(updated.Member.Email))
                teacher.Member.Email = updated.Member.Email;
            // If updating teacher's password is required – can add a condition similar to the others

            await db.SaveChangesAsync();
            return Results.NoContent();
        });


        app.MapDelete("/api/teachers/{id}", async (int id, AppDbContext db) =>
        {
            var teacher = await db.Teachers.FindAsync(id);
            if (teacher is null) return Results.NotFound();

            db.Teachers.Remove(teacher);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
