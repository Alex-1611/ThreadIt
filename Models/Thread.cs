using System.ComponentModel.DataAnnotations;

namespace ThreadIt.Models
{
    public class Thread
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Titlul este obligatoriu")]
        [StringLength(75, ErrorMessage = "Titlul nu poate avea mai mult de 75 de caractere")]
        [MinLength(5, ErrorMessage = "Titlul trebuie sa aiba mai mult de 5 caractere")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Continutul articolului este obligatoriu")]
        public string Content { get; set; }

        public int? CategoryId { get; set; }
        public virtual Category? Category { get; set; }

        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        public DateTime CreateTime { get; set; }
    }
}
