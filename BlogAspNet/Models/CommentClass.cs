using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlogAspNet.Models
{
    public class CommentClass
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Texto { get; set; }
        public int OwnerId { get; set; }
    }
}
