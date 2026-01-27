using Gck.Application.Services;
using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Sessions.Commands.FinishSession;

public class FinishSessionCommandHandler : IRequestHandler<FinishSessionCommand, Unit>
{
    private readonly GckDbContext _context;
    private readonly ILoyaltyService _loyaltyService;

    public FinishSessionCommandHandler(GckDbContext context, ILoyaltyService loyaltyService)
    {
        _context = context;
        _loyaltyService = loyaltyService;
    }

    public async Task<Unit> Handle(FinishSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .Include(s => s.Table)
            .Include(s => s.Fee)
            .Include(s => s.SessionCustomers)
            .ThenInclude(sc => sc.Customer)
            .FirstOrDefaultAsync(s => s.Id == request.SessionId, cancellationToken);

        if (session == null)
        {
            throw new InvalidOperationException($"Session with ID '{request.SessionId}' not found.");
        }

        if (session.IsCompleted)
        {
            throw new InvalidOperationException($"Session with ID '{request.SessionId}' is already completed.");
        }

        var financialAccount = await _context.FinancialAccounts
            .FirstOrDefaultAsync(f => f.Id == request.FinancialAccountId, cancellationToken);

        if (financialAccount == null)
        {
            throw new InvalidOperationException($"Financial account with ID '{request.FinancialAccountId}' not found.");
        }

        session.EndDateTime = DateTime.Now;
        session.IsCompleted = true;
        
        var duration = (session.EndDateTime.Value - session.StartDateTime).TotalHours;
        var recommendedPrice = Convert.ToDecimal(duration) * session.Fee.Fee;
        session.RecommendedPrice = recommendedPrice;
        session.FinalPrice = request.FinalPrice;
        session.LastModifiedDate = DateTime.Now;

        session.Table.IsOccupied = false;
        session.Table.LastModifiedDate = DateTime.Now;

        // Handle loyalty program - per-person basis
        int totalPeople = session.SessionCustomers.Count + session.AnonymousCustomersCount;
        decimal pricePerPerson = totalPeople > 0 ? recommendedPrice / totalPeople : recommendedPrice;
        bool anyFreeSession = false;
        
        foreach (var sessionCustomer in session.SessionCustomers)
        {
            var customer = sessionCustomer.Customer;
            
            if (customer.IsLoyal && customer.SessionsRequiredForFree > 0)
            {
                bool customerGotFreeSession = await _loyaltyService.CanCustomerGetFreeSession(customer.Id);
                
                if (customerGotFreeSession)
                {
                    // Customer used their free session, reset counter
                    await _loyaltyService.ResetPaidSessionsCount(customer.Id);
                    anyFreeSession = true;
                }
                else
                {
                    // Increment paid sessions count for loyal customers who paid
                    await _loyaltyService.IncrementPaidSessions(customer.Id);
                }
            }
        }
        
        // Mark session as free if any customer got their share for free
        if (anyFreeSession)
        {
            session.IsFreeSession = true;
        }

        var receipt = new AccountantReceipt
        {
            SessionId = session.Id,
            FinancialAccountId = request.FinancialAccountId,
            RecommendedPrice = recommendedPrice,
            FinalPrice = request.FinalPrice,
            ReceiptDateTime = DateTime.Now,
            CreateDate = DateTime.Now
        };

        _context.AccountantReceipts.Add(receipt);
        await _context.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
