using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyTestApp.Models;
using System;
using System.Collections.Generic;

using System.Linq;
using System.Threading.Tasks;

namespace BlogAspNet.Models
{
    public class IdentityAppContext : IdentityDbContext<AppUser, AppRole, int> 
    {
        public IdentityAppContext(DbContextOptions<IdentityAppContext> options) : base(options) { }

        public DbSet<Post> Posts { get; set; }
    }
}
