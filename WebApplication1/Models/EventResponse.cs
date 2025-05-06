using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace WebApplication1.Models
{
    public class EventResponse
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
        public bool IsAttending { get; set; }

        [Column(TypeName = "DATETIME")]
        public DateTime ResponseDate { get; set; } = DateTime.UtcNow;
    }
}
