using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreadIt.Models
{
    public class Subscribe
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string? UserId { get; set; }
        public int? CategoryId { get; set; }
        public virtual AppUser? User { get; set; }
        public virtual Category? Category { get; set; }


    }
}
