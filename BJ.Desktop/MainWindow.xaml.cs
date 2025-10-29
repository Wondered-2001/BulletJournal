using BJ.Data;
using BJ.Core.ViewModels;
//using Microsoft.EntityFrameworkCore;
using System.Windows;

namespace BJ.Desktop
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //// Configure DbContext options
            //var options = new DbContextOptionsBuilder<BulletJournalDbContext>()
            //              .UseSqlite("Data Source=bulletjournal.db")
            //              .Options;

            //// Create DbContext & repository
            //var dbContext = new BulletJournalDbContext(options);
            //dbContext.Database.EnsureCreated();
            var repo = new JournalRepository(App.DbContext);

            // Assign ViewModel as DataContext
            DataContext = new MainViewModel(repo); 
            if (DataContext == null)
                MessageBox.Show("DataContext is null!");

        }
    }
}
