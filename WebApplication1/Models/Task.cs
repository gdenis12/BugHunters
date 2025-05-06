using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Task
    {
        [Key]
        [ForeignKey("Event")]
        public int Id { get; set; }

        [JsonIgnore]
        public Event Event { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public required string Name { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime DateOfEnding { get; set; }

        [Column(TypeName = "NVARCHAR(MAX)")]
        public string Content { get; set; } = string.Empty;

        public bool IsCompleted { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? CompletedAt { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column(TypeName = "DATETIME")]
        public DateTime? UpdatedAt { get; set; }
    }
}
