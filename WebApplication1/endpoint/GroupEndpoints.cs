using Microsoft.EntityFrameworkCore;
using WebApplication1;
using WebApplication1.Models;
using WebApplication1.DTO;
using Microsoft.AspNetCore.Mvc;

public static class GroupEndpoints
{
    public static void MapGroupEndpoints(this WebApplication app)
    {
        app.MapGet("/api/groups", async (AppDbContext db) =>
            await db.Groups
                .Include(g => g.Teacher)
                    .ThenInclude(t => t.Member)
                .Include(g => g.Students)
                    .ThenInclude(s => s.Member)
                .ToListAsync());

        app.MapGet("/api/groups/{id}", async (int id, AppDbContext db) =>
            await db.Groups
                .Include(g => g.Teacher)
                    .ThenInclude(t => t.Member)
                .Include(g => g.Students)
                    .ThenInclude(s => s.Member)
                .FirstOrDefaultAsync(g => g.Id == id) is Group group
                ? Results.Ok(group)
                : Results.NotFound());

        // Create group
        app.MapPost("/api/groups", async (GroupCreationDto dto, AppDbContext db) =>
        {
            var teacher = await db.Teachers.FindAsync(dto.TeacherId);
            if (teacher is null) return Results.BadRequest("Teacher not found");

            var group = new Group
            {
                Name = dto.Name,
                TeacherId = dto.TeacherId,
                Teacher = teacher,
                Students = new List<Student>()
            };

            db.Groups.Add(group);
            await db.SaveChangesAsync();
            return Results.Created($"/api/groups/{group.Id}", group);
        });

        app.MapPost("/api/groups/{groupId}/students/{studentId}", async (int groupId, int studentId, AppDbContext db) =>
        {
            var group = await db.Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group is null)
                return Results.NotFound($"Group {groupId} not found");

            var student = await db.Students.FindAsync(studentId);
            if (student is null)
                return Results.NotFound($"Student {studentId} not found");

            if (group.Students.Any(s => s.Id == studentId))
                return Results.BadRequest("Student already in the group");

            group.Students.Add(student);
            await db.SaveChangesAsync();

            return Results.Ok(group);
        });

        app.MapDelete("/api/groups/{groupId}/students/{studentId}", async (int groupId, int studentId, AppDbContext db) =>
        {
            var group = await db.Groups
                .Include(g => g.Students)
                .FirstOrDefaultAsync(g => g.Id == groupId);

            if (group is null)
                return Results.NotFound($"Group {groupId} not found");

            var student = group.Students.FirstOrDefault(s => s.Id == studentId);
            if (student is null)
                return Results.NotFound($"Student {studentId} not found in the group");

            group.Students.Remove(student);
            await db.SaveChangesAsync();

            return Results.Ok();
        });


        app.MapPut("/api/groups/{id}", async (int id, [FromBody] GroupUpdateDto dto, AppDbContext db) =>
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return Results.NotFound();

            if (!string.IsNullOrWhiteSpace(dto.Name))
                group.Name = dto.Name;
            if (dto.TeacherId != default)
            {
                var teacher = await db.Teachers.FindAsync(dto.TeacherId);
                if (teacher is null) return Results.BadRequest("Teacher not found");
                group.TeacherId = dto.TeacherId;
                group.Teacher = teacher;  // Update the link if required
            }

            await db.SaveChangesAsync();
            return Results.NoContent();
        });

        app.MapDelete("/api/groups/{id}", async (int id, AppDbContext db) =>
        {
            var group = await db.Groups.FindAsync(id);
            if (group is null) return Results.NotFound();

            db.Groups.Remove(group);
            await db.SaveChangesAsync();
            return Results.Ok();
        });
    }
}
