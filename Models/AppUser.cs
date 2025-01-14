﻿using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations.Schema;

namespace ThreadIt.Models
{
    public class AppUser : IdentityUser
    {
        public String? Descriere { get; set; }

        public ICollection<Subscribe> Subs{ get; set; }

        public virtual ICollection<Thread>? Threads { get; set; }

        public virtual ICollection<Comment>? Comments { get; set; }

        [NotMapped]
        public IEnumerable<SelectListItem>? AllRoles { get; set; }

    }
}
