using System;
using Microsoft.EntityFrameworkCore;
using MinionSuiteExample.Web.Models;

namespace MinionSuiteExample.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}
