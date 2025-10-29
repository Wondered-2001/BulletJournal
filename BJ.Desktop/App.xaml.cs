using BJ.Data;
//using BulletJournal.Data;
using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace BJ.Desktop
{
    public partial class App : Application
    {
        // Make DbContext accessible from MainWindow or anywhere else
        public static BulletJournalDbContext DbContext { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Configure SQLite DbContext
            var options = new DbContextOptionsBuilder<BulletJournalDbContext>()
                .UseSqlite("Data Source=bulletjournal.db")
                .Options;

            DbContext = new BulletJournalDbContext(options);

            // Ensure database and tables are created
            DbContext.Database.EnsureCreated();

            // Launch the main window
            var mainWindow = new MainWindow();
            mainWindow.Show();
        }

        protected override void OnExit(ExitEventArgs e)
        {
            DbContext?.Dispose();
            base.OnExit(e);
        }
    }
}
