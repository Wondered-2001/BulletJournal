using BJ.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BulletJournal.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<BulletJournalDbContext>
    {
        public BulletJournalDbContext CreateDbContext(string[] args)
        {
            var options = new DbContextOptionsBuilder<BulletJournalDbContext>()
                .UseSqlite("Data Source=bulletjournal.db")
                .Options;
            return new BulletJournalDbContext(options);
        }
    }
}
