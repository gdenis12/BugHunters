using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WebApplication1.Models
{
    public class Member
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(50)")]
        public required string Surname { get; set; }

        [Required]
        [Column(TypeName = "NVARCHAR(50)")]
        public required string Name { get; set; }

        [Required]
        [Column(TypeName = "CHAR(15)")]
        public required string Phone { get; set; }

        [Required, EmailAddress] 
        [Column(TypeName = "VARCHAR(100)")]
        public required string Email { get; set; }

        [Required]
        [Column(TypeName = "varbinary(64)")]
        public required byte[] Password { get; set; }

        [JsonIgnore]
        public Teacher? Teacher { get; set; }
        [JsonIgnore]
        public Student? Student { get; set; }
        [JsonIgnore]
        public Parent? Parent { get; set; }

        public required ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
