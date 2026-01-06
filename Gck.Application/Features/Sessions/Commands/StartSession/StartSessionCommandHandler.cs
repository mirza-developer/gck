using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Sessions.Commands.StartSession;

public class StartSessionCommandHandler : IRequestHandler<StartSessionCommand, int>
{
    private readonly GckDbContext _context;

    public StartSessionCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<int> Handle(StartSessionCommand request, CancellationToken cancellationToken)
    {
        var table = await _context.Tables
            .FirstOrDefaultAsync(t => t.Id == request.TableId, cancellationToken);

        if (table == null)
        {
            throw new InvalidOperationException($"Table with ID '{request.TableId}' not found.");
        }

        if (table.IsOccupied)
        {
            throw new InvalidOperationException($"Table '{table.Name}' is already occupied.");
        }

        // Find the hourly fee based on the number of customers (seats)
        var seatsCount = request.CustomerIds?.Count ?? 1;
        var hourlyFee = await _context.Fees
            .FirstOrDefaultAsync(hf => hf.SeatsCount == seatsCount, cancellationToken);

        if (hourlyFee == null)
        {
            throw new InvalidOperationException($"No hourly fee configuration found for {seatsCount} seats.");
        }

        var session = new Session
        {
            TableId = request.TableId,
            FeeId = hourlyFee.Id,
            StartDateTime = DateTime.UtcNow,
            IsCompleted = false,
            CreateDate = DateTime.UtcNow,
            LastModifiedDate = DateTime.UtcNow
        };

        _context.Sessions.Add(session);
        
        table.IsOccupied = true;
        table.LastModifiedDate = DateTime.UtcNow;

        if (request.CustomerIds != null && request.CustomerIds.Any())
        {
            foreach (var customerId in request.CustomerIds)
            {
                var customer = await _context.Customers
                    .FirstOrDefaultAsync(c => c.Id == customerId, cancellationToken);

                if (customer != null)
                {
                    var sessionCustomer = new SessionCustomer
                    {
                        Session = session,
                        CustomerId = customerId,
                        CreateDate = DateTime.UtcNow
                    };
                    _context.SessionCustomers.Add(sessionCustomer);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
