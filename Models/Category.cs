using System.ComponentModel.DataAnnotations;

namespace ThreadIt.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required!")]
        public string Name { get; set; }

        public virtual ICollection<Thread>? Threads { get; set; }
    }
}
