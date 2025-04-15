using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Parent
    {
        [Key]
        [ForeignKey("Member")]
        public int Id { get; set; }
        public required Member Member { get; set; }

        [ForeignKey("ParentType")]
        public int ParentTypeId { get; set; }
        public ParentType ParentType { get; set; }

        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
