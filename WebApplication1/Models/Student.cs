using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Student
    {
        [Key]
        [ForeignKey("Member")]
        public int Id { get; set; }
        public required Member Member { get; set; }

        [Column(TypeName = "DATE")]
        public required DateTime BirthDay { get; set; }

        [ForeignKey("Group")]
        public int? GroupId { get; set; } 
        public Group? Group { get; set; }

        [JsonIgnore]
        public ICollection<Parent> Parents { get; set; } = new List<Parent>();
    }
}
