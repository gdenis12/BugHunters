using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Models
{
    public class TaskCompletion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Task")]
        public int TaskId { get; set; }
        public Task Task { get; set; }

        [Required]
        [ForeignKey("Member")]
        public int MemberId { get; set; }
        public Member Member { get; set; }

        [Required]
        public bool IsCompleted { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime? CompletedAt { get; set; }
    }
}
