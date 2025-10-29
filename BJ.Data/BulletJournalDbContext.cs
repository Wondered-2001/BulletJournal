using Microsoft.EntityFrameworkCore;
using BJ.Core.Models;

namespace BJ.Data
{
    public class BulletJournalDbContext : DbContext
    {
        public DbSet<JournalItem> JournalItems { get; set; } = null;
        public DbSet<Tag> Tags { get; set; } = null;

        public BulletJournalDbContext(DbContextOptions<BulletJournalDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // setup many-to-many mapping
            modelBuilder.Entity<JournalItem>()
                .HasMany(j => j.Tags)
                .WithMany(t => t.Items);
        }
    }
}
