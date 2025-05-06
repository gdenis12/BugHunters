using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using WebApplication1.Models;
using BCrypt.Net;
using WebApplication1.DTO;
namespace WebApplication1.endpoint
{
    public class AuthEndpoints
    {
        public static void MapAuthEndpoints(this WebApplication app)
        {
            app.MapPost("/api/auth/register", async (AppDbContext db, RegisterDto dto) =>
            {
                if (await db.Members.AnyAsync(m => m.Email == dto.Email))
                {
                    return Results.BadRequest("Користувач з таким email вже існує");
                }

                var member = new Member
                {
                    Surname = dto.Surname,
                    Name = dto.Name,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
                    Role = dto.Role,
                    Events = new List<Event>()
                };

                db.Members.Add(member);
                await db.SaveChangesAsync();

                switch (dto.Role.ToLower())
                {
                    case "parent":
                        db.Parents.Add(new Parent
                        {
                            Id = member.Id,
                            Member = member,
                            ParentTypeId = 1
                        });
                        break;
                    case "student":
                        db.Students.Add(new Student
                        {
                            Id = member.Id,
                            Member = member,
                            BirthDay = dto.BirthDay ?? DateTime.MinValue,
                            GroupId = null
                        });
                        break;
                    case "teacher":
                        db.Teachers.Add(new Teacher
                        {
                            Id = member.Id,
                            Member = member
                        });
                        break;
                    default:
                        return Results.BadRequest("Невідома роль");
                }

                await db.SaveChangesAsync();
                return Results.Ok(new { message = "Успішна реєстрація" });
            });

            app.MapPost("/api/auth/login", async (AppDbContext db, LoginDto dto, IConfiguration config) =>
            {
                var member = await db.Members.FirstOrDefaultAsync(m => m.Email == dto.Email);
                if (member == null || !BCrypt.Net.BCrypt.Verify(dto.Password, member.PasswordHash))
                {
                    return Results.BadRequest("Невірна пошта або пароль");
                }

                // Формування claims для JWT:
                var claims = new List<Claim>
                {
                    new Claim(JwtRegisteredClaimNames.Sub, member.Id.ToString()),
                    new Claim(JwtRegisteredClaimNames.Email, member.Email),
                    new Claim("role", member.Role)
                };

                // Отримуємо секретний ключ з конфігурації:
                var jwtSecret = config["JWT:Secret"] ?? "my_super_secret_key_12345";
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
                var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

                // Строк дії токену - наприклад, 1 година:
                var expiry = DateTime.UtcNow.AddHours(1);

                var token = new JwtSecurityToken(
                    issuer: null,
                    audience: null,
                    claims: claims,
                    expires: expiry,
                    signingCredentials: creds
                );

                var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

                return Results.Ok(new
                {
                    token = jwtToken,
                    user = new
                    {
                        member.Id,
                        member.Email,
                        member.Role,
                        member.Name,
                        member.Surname
                    }
                });
            });

            // Endpoint для отримання даних залогіненого користувача через JWT
            app.MapGet("/api/auth/me", async (HttpContext http, AppDbContext db) =>
            {
                if (!http.User.Identity?.IsAuthenticated ?? true)
                {
                    return Results.Unauthorized();
                }

                // Читаємо claim "sub" як ID користувача:
                var idClaim = http.User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
                if (idClaim == null || !int.TryParse(idClaim.Value, out var memberId))
                {
                    return Results.BadRequest(new { message = "Невірний токен" });
                }

                var member = await db.Members.FindAsync(memberId);
                if (member == null)
                {
                    return Results.NotFound(new { message = "Користувача не знайдено" });
                }

                return Results.Ok(new
                {
                    member.Id,
                    member.Email,
                    member.Role,
                    member.Name,
                    member.Surname,
                    member.Phone
                });
            }).RequireAuthorization();
        }
    }
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}

namespace WebApplication1.DTO
{
    public class RegisterDto
    {
        public string Surname { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Role { get; set; }
        public DateTime? BirthDay { get; set; } // якщо потрібно для student
    }
}
