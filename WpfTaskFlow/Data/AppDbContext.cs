using System.IO;
using Microsoft.EntityFrameworkCore;
using WpfTaskFlow.Models;

namespace WpfTaskFlow.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }

        public AppDbContext() { }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            if (!options.IsConfigured)
            {
                string dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "taskflow.db");
                options.UseSqlite($"Data Source={dbPath}");
            }
        }
    }
}