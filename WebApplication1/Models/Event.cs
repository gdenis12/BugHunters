using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Event
    {
        [Key]
public int Id { get; set; }

[ForeignKey("EventType")]
public int EventTypeId { get; set; }

public EventType EventType { get; set; }

[Required]
[Column(TypeName = "NVARCHAR(100)")]
public required string Name { get; set; }

[Column(TypeName = "DATETIME")]
public DateTime DateOfStart { get; set; }

[Range(1, int.MaxValue, ErrorMessage = "Duration must be greater than 0")]
public int DurationInMinutes { get; set; }

public string Content { get; set; }

[Required]
public bool IsContentHidden { get; set; }

public string VenueOrLink { get; set; }

[Required]
[ForeignKey("Creator")]
public int CreatorId { get; set; }
public Member Creator { get; set; }

public required ICollection<Member> Members { get; set; } = new List<Member>();

public ICollection<EventResponse> Responses { get; set; } = new List<EventResponse>();

public ICollection<EventComment> Comments { get; set; } = new List<EventComment>();

public Task? Task { get; set; }

[Column(TypeName = "DATETIME")]
public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

public DateTime? UpdatedAt { get; set; }
    }
}
