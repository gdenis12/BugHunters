using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class EventComment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Event")]
        public int EventId { get; set; }
        public Event Event { get; set; }

        [Required]
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public Member Member { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(1000)")]
        public required string Content { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }
    }
}
