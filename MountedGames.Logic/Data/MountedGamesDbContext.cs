using Microsoft.EntityFrameworkCore;
using MountedGames.Logic.Models;

namespace MountedGames.Logic.Data
{
    public class MountedGamesDbContext : DbContext
    {
        public MountedGamesDbContext(DbContextOptions<MountedGamesDbContext> options)
            : base(options) { }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();
        }
    }
}
