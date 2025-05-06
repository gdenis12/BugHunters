using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Group
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(100)")]
        public required string Name { get; set; }

        [ForeignKey("Teacher")]
        public required int TeacherId { get; set; }
        public required Teacher Teacher { get; set; }

        [JsonIgnore]
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
