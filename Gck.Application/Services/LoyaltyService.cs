using Gck.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Services;

public class LoyaltyService : ILoyaltyService
{
    private readonly GckDbContext _context;

    public LoyaltyService(GckDbContext context)
    {
        _context = context;
    }

    public async Task<bool> CanCustomerGetFreeSession(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        
        if (customer == null || !customer.IsLoyal || customer.SessionsRequiredForFree == 0)
        {
            return false;
        }

        return customer.PaidSessionsCount >= customer.SessionsRequiredForFree;
    }

    public async Task IncrementPaidSessions(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        
        if (customer != null && customer.IsLoyal)
        {
            customer.PaidSessionsCount++;
            customer.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }

    public async Task ResetPaidSessionsCount(int customerId)
    {
        var customer = await _context.Customers.FindAsync(customerId);
        
        if (customer != null && customer.IsLoyal)
        {
            customer.PaidSessionsCount = 0;
            customer.LastModifiedDate = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }
    }
}
