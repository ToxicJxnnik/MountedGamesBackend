using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Models;

namespace MountedGames.Logic.Data
{
    public class MountedGamesDbContext : DbContext
    {
        public MountedGamesDbContext(DbContextOptions<MountedGamesDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        public DbSet<Event> Events { get; set; }

        // OnModelCreating nur FÜR:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Email eindeutig machen (wichtig für Login!)
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // Das war's! Alles andere macht EF automatisch.
        }
    }
}
