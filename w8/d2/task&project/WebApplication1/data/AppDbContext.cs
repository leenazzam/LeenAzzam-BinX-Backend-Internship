using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication1.models;

namespace WebApplication1.Data
{
    public class AppDbContext : IdentityDbContext<IdentityUser>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Project> Projects { get; set; }
        public DbSet<AppTask> Tasks { get; set; }
        public DbSet<TaskAssignment> TaskAssignments { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Project -> Owner (AspNetUsers)
            builder.Entity<Project>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(p => p.OwnerId)
                .OnDelete(DeleteBehavior.Restrict);

            // Project -> Tasks
            builder.Entity<AppTask>()
                .HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId)
                .OnDelete(DeleteBehavior.Cascade);

            // Task -> TaskAssignments
            builder.Entity<TaskAssignment>()
                .HasOne(ta => ta.Task)
                .WithMany()
                .HasForeignKey(ta => ta.TaskId)
                .OnDelete(DeleteBehavior.Cascade);

            // TaskAssignment -> User (AspNetUsers)
            builder.Entity<TaskAssignment>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ta => ta.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Seed initial project data
            builder.Entity<Project>().HasData(
                new Project
                {
                    Id = 2,
                    Name = "Sample Project",
                    Description = "Initial seeded project for Sprint 1",
                    CreatedDate = new DateTime(2026, 8, 24),
                    OwnerId = "f0e5b283-ea85-4462-b567-d0f95f649ff8"
                }
            );

            // Seed initial task data
            builder.Entity<AppTask>().HasData(
                new AppTask
                {
                    Id = 1,
                    Title = "Initial Setup Task",
                    Status = "Pending",
                    DueDate = new DateTime(2026, 9, 1),
                    ProjectId = 2
                }
            );
        }

    }
}