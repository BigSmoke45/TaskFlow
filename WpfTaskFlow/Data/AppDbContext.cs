using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using WpfTaskFlow.Models;

namespace WpfTaskFlow.Data
{

    public class AppDbContext : DbContext
    {
        public DbSet<TaskItem> Tasks { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            string dbPath = System.IO.Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "taskflow.db"
            );
            options.UseSqlite($"Data Source={dbPath}");
        }
    }
}