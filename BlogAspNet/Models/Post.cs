using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAspNet.Models
{
    public class Post
    {
        public Post() { }

        public int Id { get; set; }
        public string Titulo { get; set; }
        public string Texto { get; set; }

        [ForeignKey("AppUser")]
        public int AppUserFK { get; set; }
    }
}
