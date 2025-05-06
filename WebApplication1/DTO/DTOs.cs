namespace WebApplication1.DTO
{
    public class MemberCreationDto
    {
        public string Surname { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PlainPassword { get; set; } = null!;
    }

    public class TeacherCreationDto
    {
        public MemberCreationDto Member { get; set; } = null!;
    }

    public class StudentCreationDto
    {
        public DateTime BirthDay { get; set; }
        public int? GroupId { get; set; }
        public MemberCreationDto Member { get; set; } = null!;
    }

    public class ParentCreationDto
    {
        public int ParentTypeId { get; set; }
        public MemberCreationDto Member { get; set; } = null!;
        public List<int>? StudentIds { get; set; } 
    }

    public class MemberUpdateDto
    {
        public string Surname { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        // Если пароль не передаётся, оставляем текущий
        public string? PlainPassword { get; set; }
    }

    public class ParentUpdateDto
    {
        public int ParentTypeId { get; set; }
        // Обновление данных члена системы (Member)
        public MemberUpdateDto? Member { get; set; }
        // Обновляем связи: список Id студентов, с которыми связан родитель
        public List<int>? StudentIds { get; set; }
    }

    public class StudentUpdateDto
    {
        public DateTime BirthDay { get; set; }
        public int? GroupId { get; set; }
        // Обновление данных члена системы (Member)
        public MemberUpdateDto? Member { get; set; }
        // Обновляем связи: список Id родителей, с которыми связан ученик
        public List<int>? ParentIds { get; set; }
    }

    public class GroupCreationDto
    {
        public string Name { get; set; } = null!;
        public int TeacherId { get; set; }
    }

    public class GroupUpdateDto
    {
        public string Name { get; set; } = null!;
        public int TeacherId { get; set; }
    }

    public class EventCreationDto
    {
        public int EventTypeId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfStart { get; set; }
        public int DurationInMinutes { get; set; }
        public string Content { get; set; } = string.Empty;
        public string VenueOrLink { get; set; } = string.Empty;
    }

    public class EventUpdateDto
    {
        public int EventTypeId { get; set; }
        public string Name { get; set; } = null!;
        public DateTime DateOfStart { get; set; }
        public int DurationInMinutes { get; set; }
        public string Content { get; set; } = string.Empty;
        public string VenueOrLink { get; set; } = string.Empty;
    }

    
}
