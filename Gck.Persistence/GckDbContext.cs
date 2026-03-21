using Gck.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Gck.Persistence;

public class GckDbContext : DbContext
{
    public GckDbContext(DbContextOptions<GckDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; } = null!;
    public DbSet<UserClaim> UserClaims { get; set; } = null!;
    
    // Gaming Center Management entities
    public DbSet<Table> Tables { get; set; } = null!;
    public DbSet<Customer> Customers { get; set; } = null!;
    public DbSet<CustomerOtp> CustomerOtps { get; set; } = null!;
    public DbSet<CustomerFeedback> CustomerFeedbacks { get; set; } = null!;
    public DbSet<FinancialAccount> FinancialAccounts { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<SessionCustomer> SessionCustomers { get; set; } = null!;
    public DbSet<AccountantReceipt> AccountantReceipts { get; set; } = null!;
    public DbSet<HourlyFee> Fees { get; set; } = null!;
    public DbSet<Transaction> Transactions { get; set; } = null!;
    public DbSet<CreditWithdrawalRequest> CreditWithdrawalRequests { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(e => e.Username).IsUnique();
        });

        // Configure UserClaim entity
        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserClaims)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Table entity
        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasIndex(e => e.PhoneNumber);

            entity.HasOne(e => e.ReferredByCustomer)
                  .WithMany(c => c.ReferredCustomers)
                  .HasForeignKey(e => e.ReferredByCustomerId)
                  .OnDelete(DeleteBehavior.SetNull)
                  .IsRequired(false);

            entity.HasIndex(e => e.ReferredByCustomerId);
            entity.HasIndex(e => e.IsVerifiedByAdmin);
        });

        // Configure CustomerOtp entity
        modelBuilder.Entity<CustomerOtp>(entity =>
        {
            entity.HasIndex(e => e.PhoneNumber);
            entity.HasIndex(e => e.ExpiresAt);
        });

        // Configure CustomerFeedback entity
        modelBuilder.Entity<CustomerFeedback>(entity =>
        {
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.SubmittedAt);
            entity.HasIndex(e => e.IsRead);
        });

        // Configure Session entity
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasOne(e => e.Table)
                  .WithMany(t => t.Sessions)
                  .HasForeignKey(e => e.TableId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.TableId);
            entity.HasIndex(e => e.StartDateTime);
            entity.HasIndex(e => e.IsCompleted);
        });

        // Configure SessionCustomer entity
        modelBuilder.Entity<SessionCustomer>(entity =>
        {
            entity.HasOne(e => e.Session)
                  .WithMany(s => s.SessionCustomers)
                  .HasForeignKey(e => e.SessionId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(e => e.Customer)
                  .WithMany(c => c.SessionCustomers)
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(e => new { e.SessionId, e.CustomerId }, "IX_SessionCustomer_SessionId_CustomerId").IsUnique();
        });

        // Configure AccountantReceipt entity
        modelBuilder.Entity<AccountantReceipt>(entity =>
        {
            entity.HasOne(e => e.Session)
                  .WithOne(s => s.AccountantReceipt)
                  .HasForeignKey<AccountantReceipt>(e => e.SessionId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasOne(e => e.FinancialAccount)
                  .WithMany(f => f.AccountantReceipts)
                  .HasForeignKey(e => e.FinancialAccountId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.SessionId).IsUnique();
            entity.HasIndex(e => e.ReceiptDateTime);
        });

        // Configure Transaction entity
        modelBuilder.Entity<Transaction>(entity =>
        {
            entity.HasOne(e => e.FinancialAccount)
                  .WithMany(f => f.Transactions)
                  .HasForeignKey(e => e.FinancialAccountId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.FinancialAccountId);
            entity.HasIndex(e => e.TransactionDate);
            entity.HasIndex(e => e.Type);
        });

        // Configure CreditWithdrawalRequest entity
        modelBuilder.Entity<CreditWithdrawalRequest>(entity =>
        {
            entity.HasOne(e => e.Customer)
                  .WithMany()
                  .HasForeignKey(e => e.CustomerId)
                  .OnDelete(DeleteBehavior.Restrict);

            entity.HasIndex(e => e.CustomerId);
            entity.HasIndex(e => e.Status);
            entity.HasIndex(e => e.RequestDate);
        });
    }
}
