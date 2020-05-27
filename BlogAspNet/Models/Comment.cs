using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAspNet.Models
{
    public class Comment
    {
        public Comment () { }

        public int Id { get; set; }
        public string Text { get; set; }

        [ForeignKey("AppUser")]
        public int AppUserFK { get; set; }

        [ForeignKey("Post")]
        public int PostFK { get; set; }

        [ForeignKey("PostId")]
        public int PostId { get; set; }

    }
}
