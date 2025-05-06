using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class EventType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public required string EventTypeName { get; set; }
    }
}
