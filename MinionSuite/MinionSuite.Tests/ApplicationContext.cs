using System;
using Microsoft.EntityFrameworkCore;
using MinionSuite.Tests.Models;

namespace MinionSuite.Tests
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
        {
        }

        public DbSet<Post> Posts { get; set; }
    }
}
