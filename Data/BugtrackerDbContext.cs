using bugtracker_backend_net.Data.Models;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace bugtracker_backend_net.Data
{
    public class BugtrackerDbContext : DbContext
    {
        public BugtrackerDbContext(DbContextOptions<BugtrackerDbContext> options) : base(options) { }

        public DbSet<Project> Projects { get; set; }
        public DbSet<Ticket> Tickets { get; set; }
        public DbSet<User> Users { get; set; }

        public DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Configure Project-User Many-to-Many
            modelBuilder.Entity<Project>()
                .HasMany(p => p.Users)
                .WithMany(u => u.Projects);


            // Configure Ticket-Project
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Project)
                .WithMany()
                .HasForeignKey(t => t.ProjectId);

            // Configure Ticket-User (Assignee) 
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Assignee)
                .WithMany(u => u.TicketsAssigned)
                .HasForeignKey(t => t.AssigneeId);

            // Configure Ticket-User (Submitter)
            modelBuilder.Entity<Ticket>()
                .HasOne(t => t.Submitter)
                .WithMany(u => u.TicketsSubmitted)
                .HasForeignKey(t => t.SubmitterId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
