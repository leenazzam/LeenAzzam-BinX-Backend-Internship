using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using WebApplication1.models;

namespace WebApplication1.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {

        }


        public DbSet<Project> Projects { get; set; }

        public DbSet<AppTask> Tasks { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<TaskAssignment> TaskAssignments { get; set; }
    }
}