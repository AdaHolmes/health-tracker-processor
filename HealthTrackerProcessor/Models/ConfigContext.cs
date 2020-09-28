using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Sqlite;
using Microsoft.Extensions.Configuration;
using HealthTrackerProcessor.Class;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure.Interception;
using HealthTrackerProcessor.Constant;

namespace HealthTrackerProcessor.Models
{
    public class ConfigContext : DbContext
    {
        private readonly IDBContextConfiguration _configuration;
        public DbSet<Project> Porjects { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<HealthData> HealthDatas { get; set; }


        public ConfigContext(DbContextOptions<ConfigContext> options) : base(options)
        {

        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseNpgsql(HealthTrackerProcessorConstant.PostgresDBConnectionStr);
            optionsBuilder.UseSqlite(HealthTrackerProcessorConstant.SQLiteDBConnectionStr);
            optionsBuilder.EnableDetailedErrors();
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Project>().HasKey(m => m.ID);
            builder.Entity<User>().HasKey(m => m.ID);
            builder.Entity<HealthData>().HasKey(m => m.ProjectID);
            base.OnModelCreating(builder);

        }
    }
}
