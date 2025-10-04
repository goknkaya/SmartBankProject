using Microsoft.EntityFrameworkCore;
using SmartBank.Domain.Entities;
using SmartBank.Domain.Entities.Chargeback;
using SmartBank.Domain.Entities.Switching;

namespace SmartBank.Infrastructure.Persistence
{
    public class CustomerCoreDbContext : DbContext
    {
        public CustomerCoreDbContext(DbContextOptions<CustomerCoreDbContext> options) : base(options) { }

        // === Core tablolar ===
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Card> Cards => Set<Card>();
        public DbSet<Transaction> Transactions => Set<Transaction>();
        public DbSet<Reversal> Reversals => Set<Reversal>();

        // === Clearing tabloları ===
        public DbSet<ClearingBatch> ClearingBatches => Set<ClearingBatch>();
        public DbSet<ClearingRecord> ClearingRecords => Set<ClearingRecord>();

        // === Switch tabloları ===
        public DbSet<SwitchMessage> SwitchMessages => Set<SwitchMessage>();
        public DbSet<SwitchLog> SwitchLogs => Set<SwitchLog>();
        public DbSet<CardBin> CardBins => Set<CardBin>();

        // === Chargeback tabloları ===
        public DbSet<ChargebackCase> ChargebackCases => Set<ChargebackCase>();
        public DbSet<ChargebackEvent> ChargebackEvents => Set<ChargebackEvent>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // =======================
            // Transaction
            // =======================
            modelBuilder.Entity<Transaction>(t =>
            {
                t.Property(x => x.AcquirerRef).HasMaxLength(64);
                t.HasIndex(x => x.AcquirerRef);

                // Kart silinince transaction'lar silinmesin
                t.HasOne(x => x.Card)
                 .WithMany()
                 .HasForeignKey(x => x.CardId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // =======================
            // ClearingBatch
            // =======================
            modelBuilder.Entity<ClearingBatch>(b =>
            {
                b.HasIndex(x => x.FileHash).IsUnique();          // aynı dosya iki kez işlenmesin
                b.Property(x => x.Direction).HasMaxLength(3).IsRequired(); // IN/OUT
                b.Property(x => x.Status).HasMaxLength(1).IsRequired();    // N/P/M/E
                b.Property(x => x.FileName).HasMaxLength(255);
                b.Property(x => x.FileHash).HasMaxLength(64);
                b.Property(x => x.Notes).HasMaxLength(250);
            });

            // =======================
            // ClearingRecord
            // =======================
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
                r.Property(x => x.MatchStatus).HasMaxLength(1).IsRequired(); // P/M/N/E
                r.Property(x => x.MerchantName).HasMaxLength(100);
                r.Property(x => x.CardLast4).HasMaxLength(4);
                r.Property(x => x.Amount).HasColumnType("decimal(18,2)");

                // aynı batch içinde aynı Transaction sadece 1 kere olsun
                r.HasIndex(x => new { x.BatchId, x.TransactionId })
                 .IsUnique()
                 .HasFilter("[TransactionId] IS NOT NULL");
            });

            // =======================
            // SwitchMessage
            // =======================
            modelBuilder.Entity<SwitchMessage>(e =>
            {
                e.Property(x => x.PANMasked).HasMaxLength(19);
                e.Property(x => x.Bin).HasMaxLength(6);
                e.Property(x => x.Currency).HasMaxLength(3).IsRequired();
                e.Property(x => x.Acquirer).HasMaxLength(50);
                e.Property(x => x.Issuer).HasMaxLength(50);
                e.Property(x => x.Status).HasMaxLength(20).HasDefaultValue("Pending");
                e.Property(x => x.ExternalId).HasMaxLength(64).IsRequired();

                // Amount -> decimal(18,2)
                e.Property(x => x.Amount).HasColumnType("decimal(18,2)");

                // İndeksler
                e.HasIndex(x => x.Status);
                e.HasIndex(x => x.CreatedAt);
                e.HasIndex(x => new { x.Issuer, x.Status });
                e.HasIndex(x => new { x.Acquirer, x.ExternalId })
                 .IsUnique()
                 .HasFilter("[ExternalId] IS NOT NULL");

                // FK'ler (opsiyonel ama iyi)
                e.HasOne(x => x.Card)
                 .WithMany()
                 .HasForeignKey(x => x.CardId)
                 .OnDelete(DeleteBehavior.NoAction);

                // Transaction'a da FK vermek istersen (nav yoksa da olur)
                e.HasOne<Transaction>()
                 .WithMany()
                 .HasForeignKey(x => x.TransactionId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // =======================
            // SwitchLog
            // =======================
            modelBuilder.Entity<SwitchLog>(e =>
            {
                e.HasOne(x => x.Message)
                 .WithMany()
                 .HasForeignKey(x => x.MessageId)
                 .OnDelete(DeleteBehavior.Cascade);

                e.Property(x => x.Stage).HasMaxLength(20); // Received/Routed/Persisted/Responded
                e.Property(x => x.Level).HasMaxLength(10); // INFO/WARN/ERROR
                e.Property(x => x.Note).HasMaxLength(200);

                e.HasIndex(x => x.MessageId);
                e.HasIndex(x => x.CreatedAt);

                e.HasOne(x => x.Message)
                    .WithMany()
                    .HasForeignKey(x => x.MessageId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // CardBin
            // =======================
            modelBuilder.Entity<CardBin>(e =>
            {
                e.ToTable("CardBins");
                e.HasKey(x => x.Id);
                e.HasIndex(x => x.Bin).IsUnique();
                e.Property(x => x.Bin).HasMaxLength(6);
                e.Property(x => x.Issuer).HasMaxLength(50);
            });

            // =======================
            // ChargebackCase
            // =======================
            modelBuilder.Entity<ChargebackCase>(e =>
            {
                e.Property(x => x.ReasonCode).HasMaxLength(20).IsRequired();
                e.Property(x => x.Status).HasMaxLength(15).IsRequired();
                e.Property(x => x.Currency).HasMaxLength(3).IsRequired();
                e.Property(x => x.MerchantName).HasMaxLength(100);

                e.Property(x => x.TransactionAmount).HasColumnType("decimal(18,2)");
                e.Property(x => x.DisputedAmount).HasColumnType("decimal(18,2)");

                e.HasIndex(x => x.Status);
                e.HasIndex(x => x.OpenedAt);

                // FK
                e.HasOne(x => x.Transaction)
                 .WithMany()
                 .HasForeignKey(x => x.TransactionId)
                 .OnDelete(DeleteBehavior.NoAction);
            });

            // =======================
            // ChargebackEvent
            // =======================
            modelBuilder.Entity<ChargebackEvent>(e =>
            {
                e.Property(x => x.Type).HasMaxLength(20).IsRequired();
                e.Property(x => x.Note).HasMaxLength(200);

                e.HasIndex(x => x.CaseId);
                e.HasOne(x => x.Case)
                 .WithMany(c => c.Events)
                 .HasForeignKey(x => x.CaseId)
                 .OnDelete(DeleteBehavior.Cascade);
            });

            // =======================
            // Customer
            // =======================
            modelBuilder.Entity<Customer>(e =>
            {
                e.ToTable("Customers");

                e.Property(p => p.FirstName).HasMaxLength(100).IsRequired();
                e.Property(p => p.LastName).HasMaxLength(100).IsRequired();

                e.Property(p => p.TCKN).HasMaxLength(11).IsRequired();
                e.HasIndex(p => p.TCKN).IsUnique();

                e.Property(p => p.Email).HasMaxLength(150);
                e.Property(p => p.PhoneNumber).HasMaxLength(20);
                e.Property(p => p.Gender).HasMaxLength(10);

                e.Property(p => p.IsActive).HasDefaultValue(true);
                e.Property(p => p.CreatedAt).HasDefaultValueSql("GETUTCDATE()");
            });
        }
    }
}
