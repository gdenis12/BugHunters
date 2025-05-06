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
        // If the password is not provided, keep the current one
        public string? PlainPassword { get; set; }
    }

    public class ParentUpdateDto
    {
        public int ParentTypeId { get; set; }
        // Update system member data (Member)
        public MemberUpdateDto? Member { get; set; }
        // Update relationships: list of student IDs associated with the parent
        public List<int>? StudentIds { get; set; }
    }

    public class StudentUpdateDto
    {
        public DateTime BirthDay { get; set; }
        public int? GroupId { get; set; }
        // Update system member data (Member)
        public MemberUpdateDto? Member { get; set; }
        // Update relationships: list of parent IDs associated with the student
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
