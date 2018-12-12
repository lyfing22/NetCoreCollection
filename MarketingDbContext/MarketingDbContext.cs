using System;
using System.Collections.Generic;
using System.Text;
using MarketingAsync.Dapper;
using MarketingAsync.MySql;
using Microsoft.EntityFrameworkCore;

namespace MarketingAsync.EFCore
{
    public class MarketingDbContext : DbContext
    {
        public DbSet<BookEntity> Book { get; set; }

        public DbSet<PublisherEntity> Publisher { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseMySQL(PersistentConfigurage.MasterConnectionString);
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PublisherEntity>(entity =>
            {
                entity.HasKey(e => e.ID);
                entity.Property(e => e.Name).IsRequired();
            });

            modelBuilder.Entity<BookEntity>(entity =>
            {
                entity.HasKey(e => e.ISBN);
                entity.Property(e => e.Title).IsRequired();
                entity.HasOne(d => d.Publisher)
                    .WithMany(p => p.Books);
            });
        }
    }
}
