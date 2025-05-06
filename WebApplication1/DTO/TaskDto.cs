using System.ComponentModel.DataAnnotations;

namespace WebApplication1.DTO
{
    public class TaskDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime DateOfEnding { get; set; }
        public string Content { get; set; } = string.Empty;
        public bool IsCompleted { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class UpdateTaskDto
    {
        [StringLength(100)]
        public string? Name { get; set; }

        public DateTime? DateOfEnding { get; set; }

        public string? Content { get; set; }
    }

    public class TaskCompletionDto
    {
        [Required]
        public bool IsCompleted { get; set; }
    }
}
