using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class ParentType
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(50)")]
        public required string ParentTypeName { get; set; }
    }
}
