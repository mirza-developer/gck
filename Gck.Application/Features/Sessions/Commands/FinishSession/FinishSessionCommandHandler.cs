using Gck.Application.Services;
using Gck.Common.Helpers;
using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gck.Application.Features.Sessions.Commands.FinishSession;

public class FinishSessionCommandHandler : IRequestHandler<FinishSessionCommand, Unit>
{
    private readonly GckDbContext _context;
    private readonly ILoyaltyService _loyaltyService;
    private readonly ISmsService _smsService;
    private readonly ILogger<FinishSessionCommandHandler> _logger;

    public FinishSessionCommandHandler(GckDbContext context, ILoyaltyService loyaltyService, ISmsService smsService, ILogger<FinishSessionCommandHandler> logger)
    {
        _context = context;
        _loyaltyService = loyaltyService;
        _smsService = smsService;
        _logger = logger;
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

        // Send SMS to customers after session completion
        await SendSessionCompletionMessagesAsync(session, cancellationToken);

        return Unit.Value;
    }

    private async Task SendSessionCompletionMessagesAsync(Session session, CancellationToken cancellationToken)
    {
        // Get Persian date and time
        var persianDateTimeStr = DateTime.Now.ToPersianDateTime();

        foreach (var sessionCustomer in session.SessionCustomers)
        {
            var customer = sessionCustomer.Customer;

            // Ignore customers without phone numbers or with empty phone numbers
            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                continue;
            }

            // Build loyalty program status message
            string loyaltyStatus = string.Empty;
            if (customer.IsLoyal && customer.SessionsRequiredForFree > 0)
            {
                int remainingSessions = customer.SessionsRequiredForFree - customer.PaidSessionsCount;
                if (remainingSessions > 0)
                {
                    loyaltyStatus = $"تا جلسه‌ی رایگان {remainingSessions} جلسه مانده است";
                }
                else if (remainingSessions == 0)
                {
                    loyaltyStatus = "شما واجد شرایط دریافت جلسه رایگان هستید";
                }
                else
                {
                    // Data integrity issue - log it but continue
                    _logger.LogWarning("Customer {CustomerId} has PaidSessionsCount ({PaidCount}) exceeding SessionsRequiredForFree ({RequiredCount})", 
                        customer.Id, customer.PaidSessionsCount, customer.SessionsRequiredForFree);
                }
            }

            // Build the message
            string message = $"{customer.Name} عزیز\n" +
                           "از اینکه ما را برای گذران وقت تفریح خود انتخاب کردید، متشکریم\n";
            
            if (!string.IsNullOrEmpty(loyaltyStatus))
            {
                message += $"{loyaltyStatus}\n";
            }
            
            message += persianDateTimeStr;

            try
            {
                await _smsService.SendMessageAsync(customer.PhoneNumber, message, cancellationToken);
            }
            catch (Exception ex)
            {
                // Log error but don't fail the entire operation
                // The session has already been completed successfully
                _logger.LogError(ex, "Failed to send session completion message to customer {CustomerId} at phone {PhoneNumber}", 
                    customer.Id, customer.PhoneNumber);
            }
        }
    }
}
