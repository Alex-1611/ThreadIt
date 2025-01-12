using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace ThreadIt.Models
{
    public class Comment
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Comment content is required!")]
        public string Content { get; set; }
        public int Level { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreateTime { get; set; }

        //REFEREBCES
        public int? ParentId { get; set; }
        public virtual Comment? Parent { get; set; }


        public int ThreadId { get; set; }
        public virtual Thread? Thread { get; set; }


        public string? UserId { get; set; }
        public virtual AppUser? User { get; set; }

        
        public virtual ICollection<Comment>? Replies { get; set; }


    }
}
