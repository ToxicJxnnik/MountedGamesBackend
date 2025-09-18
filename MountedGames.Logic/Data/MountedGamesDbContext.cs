using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Models;
using MountedGames.Logic.Models.Event;

namespace MountedGames.Logic.Data
{
    public class MountedGamesDbContext : DbContext
    {
        public MountedGamesDbContext(DbContextOptions<MountedGamesDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }
        public DbSet<EventRegistration> EventRegistrations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // User email should be unique
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Tournament Registration relationships
            modelBuilder.Entity<EventRegistration>()
                .HasOne(r => r.Event)
                .WithMany()
                .HasForeignKey(r => r.EventId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<EventRegistration>()
                .HasOne(r => r.User)
                .WithMany()
                .HasForeignKey(r => r.UserId)
                .OnDelete(DeleteBehavior.NoAction);

            // Ensure one registration per user per event
            modelBuilder.Entity<EventRegistration>()
                .HasIndex(r => new { r.EventId, r.UserId })
                .IsUnique();
        }
    }
}