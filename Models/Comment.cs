using System.ComponentModel.DataAnnotations;

namespace ThreadIt.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment content is required!")]
        public string Content { get; set; }
        public int ThreadId { get; set; }
        public virtual Thread? Thread { get; set; }
        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }
        public DateTime CreateTime { get; set; }
        public virtual ICollection<Comment> Replies { get; set; }
    }
}
