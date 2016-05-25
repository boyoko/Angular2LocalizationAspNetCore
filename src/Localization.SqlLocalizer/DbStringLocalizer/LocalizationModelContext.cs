﻿using System;
using System.Linq;

using Microsoft.EntityFrameworkCore;

namespace Localization.SqlLocalizer.DbStringLocalizer
{
    // >dotnet ef migrations add LocalizationMigration
    public class LocalizationModelContext : DbContext
    {
        public LocalizationModelContext(DbContextOptions<LocalizationModelContext> options) :base(options)
        { }
        
        public DbSet<LocalizationRecord> LocalizationRecords { get; set; }
        
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<LocalizationRecord>().HasKey(m => m.Id);
            //builder.Entity<LocalizationRecord>().HasKey(m => m.LocalizationCulture + m.Key);

            // shadow properties
            builder.Entity<LocalizationRecord>().Property<DateTime>("UpdatedTimestamp");

            base.OnModelCreating(builder);
        }

        public override int SaveChanges()
        {
            ChangeTracker.DetectChanges();
            updateUpdatedProperty<LocalizationRecord>();
            return base.SaveChanges();
        }

        private void updateUpdatedProperty<T>() where T : class
        {
            var modifiedSourceInfo =
                ChangeTracker.Entries<T>()
                    .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified);

            foreach (var entry in modifiedSourceInfo)
            {
                entry.Property("UpdatedTimestamp").CurrentValue = DateTime.UtcNow;
            }
        }
    }
}