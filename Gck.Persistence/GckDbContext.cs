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
    public DbSet<FinancialAccount> FinancialAccounts { get; set; } = null!;
    public DbSet<Session> Sessions { get; set; } = null!;
    public DbSet<SessionCustomer> SessionCustomers { get; set; } = null!;
    public DbSet<AccountantReceipt> AccountantReceipts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.Username).IsUnique();
            entity.Property(e => e.Username).IsRequired().HasMaxLength(256);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(512);
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.PasswordHash).IsRequired();
            entity.Property(e => e.CreatorIdentityID).HasMaxLength(128);
            entity.Property(e => e.LastModifierIdentityID).HasMaxLength(128);
        });

        // Configure UserClaim entity
        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.User)
                  .WithMany(u => u.UserClaims)
                  .HasForeignKey(e => e.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure Table entity
        modelBuilder.Entity<Table>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.NumberOfControllers).IsRequired();
            entity.Property(e => e.HourlyFeePerController).IsRequired().HasColumnType("decimal(18,2)");
            entity.HasIndex(e => e.Name).IsUnique();
        });

        // Configure Customer entity
        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(256);
            entity.Property(e => e.PhoneNumber).IsRequired().HasMaxLength(20);
            entity.Property(e => e.BirthYear).IsRequired();
            entity.Property(e => e.Gender).IsRequired().HasMaxLength(20);
            entity.HasIndex(e => e.PhoneNumber);
        });

        // Configure FinancialAccount entity
        modelBuilder.Entity<FinancialAccount>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.AccountName).IsRequired().HasMaxLength(256);
            entity.Property(e => e.CardNumber).IsRequired().HasMaxLength(50);
            entity.Property(e => e.BankName).IsRequired().HasMaxLength(256);
        });

        // Configure Session entity
        modelBuilder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.FeePerHour).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.RecommendedPrice).HasColumnType("decimal(18,2)");
            entity.Property(e => e.FinalPrice).HasColumnType("decimal(18,2)");
            
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
            entity.HasKey(e => e.Id);
            
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
            entity.HasKey(e => e.Id);
            entity.Property(e => e.RecommendedPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.FinalPrice).IsRequired().HasColumnType("decimal(18,2)");
            entity.Property(e => e.ReceiptDateTime).IsRequired();
            
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
    }
}
