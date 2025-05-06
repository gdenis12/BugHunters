using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class CreateEventDto
    {
        [Required]
        public int EventTypeId { get; set; }

        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public DateTime DateOfStart { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int DurationInMinutes { get; set; }

        public string? Content { get; set; }

        public bool IsContentHidden { get; set; }

        public string? VenueOrLink { get; set; }

        public List<int>? ParticipantIds { get; set; }

        public CreateTaskDto? Task { get; set; }
    }

    public class UpdateEventDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        public DateTime? DateOfStart { get; set; }

        [Range(1, int.MaxValue)]
        public int? DurationInMinutes { get; set; }

        public string? Content { get; set; }

        public bool? IsContentHidden { get; set; }

        public string? VenueOrLink { get; set; }

        public List<int>? ParticipantIds { get; set; }
    }

    public class EventResponseDto
    {
        [Required]
        public bool IsAttending { get; set; }
    }

    public class EventCommentDto
    {
        [Required]
        [StringLength(1000)]
        public required string Content { get; set; }
    }

    public class CreateTaskDto
    {
        [Required]
        [StringLength(100)]
        public required string Name { get; set; }

        [Required]
        public DateTime DateOfEnding { get; set; }

        public string? Content { get; set; }
    }
}
