using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication1.Models
{
    public class Teacher
    {
        [Key]
        [ForeignKey("Member")]
        public int Id { get; set; }
        public required Member Member { get; set; }
    }
}
