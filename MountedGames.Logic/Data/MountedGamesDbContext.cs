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

        // OnModelCreating nur FÜR:
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
