using Gck.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;

namespace Gck.Persistence;

public static class DbInitializer
{
    private const int SaltSize = 16;
    private const int KeySize = 32;
    private const int Iterations = 50000;
    private static readonly HashAlgorithmName Algorithm = HashAlgorithmName.SHA256;

    private static string HashPassword(string password)
    {
        var salt = RandomNumberGenerator.GetBytes(SaltSize);
        var hash = Rfc2898DeriveBytes.Pbkdf2(password, salt, Iterations, Algorithm, KeySize);
        
        var hashBytes = new byte[SaltSize + KeySize];
        Array.Copy(salt, 0, hashBytes, 0, SaltSize);
        Array.Copy(hash, 0, hashBytes, SaltSize, KeySize);
        
        return Convert.ToBase64String(hashBytes);
    }

    public static async Task InitializeAsync(GckDbContext context, ILogger logger)
    {
        // Apply pending migrations
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
        {
            await context.Database.MigrateAsync();
            logger.LogInformation("Database migrations applied successfully");
        }

        if (!await context.Users.AnyAsync())
        {
            var adminUser = new User
            {
                Id = Guid.NewGuid().ToString(),
                Username = "admin",
                Name = "مدیر سیستم",
                Email = "admin@gckgames.ir",
                PasswordHash = HashPassword("Admin@123"),
                IsActive = true,
                CreateDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                CreatorIdentityID = "system",
                PhoneNumber = null,
                Details = "حساب کاربری پیش‌فرض مدیر سیستم"
            };

            context.Users.Add(adminUser);
            await context.SaveChangesAsync();
        }

        // Seed default hourly fees if none exist
        if (!await context.Fees.AnyAsync())
        {
            var hourlyFees = new List<HourlyFee>
            {
                new HourlyFee { SeatsCount = 2, Fee = 150000, CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new HourlyFee { SeatsCount = 3, Fee = 200000, CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new HourlyFee { SeatsCount = 4, Fee = 250000, CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now },
                new HourlyFee { SeatsCount = 5, Fee = 300000, CreateDate = DateTime.Now, LastModifiedDate = DateTime.Now }
            };

            context.Fees.AddRange(hourlyFees);
            await context.SaveChangesAsync();
            logger.LogInformation("Hourly fees seeded successfully");
        }
    }
}
