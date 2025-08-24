using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SmartBank.Domain.Entities;

namespace SmartBank.Infrastructure.Persistence
{
    public class CustomerCoreDbContext : DbContext
    {
        public CustomerCoreDbContext(DbContextOptions<CustomerCoreDbContext> options) : base(options) { }

        public DbSet<Customer> Customers { get; set; }
        public DbSet<Card> Cards { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Reversal> Reversals { get; set; }
        public DbSet<ClearingBatch> ClearingBatches { get; set; } = null;
        public DbSet<ClearingRecord> ClearingRecords { get; set; } = null;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // ClearingBatch
            modelBuilder.Entity<ClearingBatch>(b =>
            {
                b.HasIndex(x => x.FileHash).IsUnique();   // aynı dosyayı iki kez işleme
                b.Property(x => x.Direction).HasMaxLength(3).IsRequired();
                b.Property(x => x.Status).HasMaxLength(1).IsRequired();
                b.Property(x => x.FileName).HasMaxLength(255);
                b.Property(x => x.FileHash).HasMaxLength(64);
                b.Property(x => x.Notes).HasMaxLength(250);
            });

            // ClearingRecord
            modelBuilder.Entity<ClearingRecord>(r =>
            {
                r.HasOne(x => x.Batch)
                 .WithMany(x => x.Records)
                 .HasForeignKey(x => x.BatchId)
                 .OnDelete(DeleteBehavior.Cascade);

                r.HasOne(x => x.Transaction)
                 .WithMany()
                 .HasForeignKey(x => x.TransactionId)
                 .OnDelete(DeleteBehavior.NoAction);

                r.HasOne(x => x.Card)
                 .WithMany()
                 .HasForeignKey(x => x.CardId)
                 .OnDelete(DeleteBehavior.NoAction);

                r.HasIndex(x => new { x.BatchId, x.MatchStatus });
                r.Property(x => x.Currency).HasMaxLength(3).IsRequired();
                r.Property(x => x.MatchStatus).HasMaxLength(1).IsRequired();
                r.Property(x => x.MerchantName).HasMaxLength(100);
                r.Property(x => x.CardLast4).HasMaxLength(4);
                r.Property(x => x.Amount).HasColumnType("decimal(18,2)");
            });
        }
    }
}
