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
            .Include(t => t.Sessions.Where(s => !s.IsCompleted))
            .FirstOrDefaultAsync(t => t.Id == request.TableId, cancellationToken);

        if (table == null)
        {
            throw new InvalidOperationException($"Table with ID '{request.TableId}' not found.");
        }

        if (table.IsOccupied)
        {
            throw new InvalidOperationException($"Table '{table.Name}' is already occupied.");
        }

        // Clean up any incomplete sessions for this table before starting a new one
        var incompleteSessions = table.Sessions.Where(s => !s.IsCompleted).ToList();
        foreach (var oldSession in incompleteSessions)
        {
            oldSession.IsCompleted = true;
            oldSession.EndDateTime = DateTime.Now;
            oldSession.LastModifiedDate = DateTime.Now;
        }

        // Find the hourly fee based on the selected seats count
        var hourlyFee = await _context.Fees
            .FirstOrDefaultAsync(hf => hf.SeatsCount == request.SeatsCount, cancellationToken);

        if (hourlyFee == null)
        {
            throw new InvalidOperationException($"No hourly fee configuration found for {request.SeatsCount} seats.");
        }

        var session = new Session
        {
            TableId = request.TableId,
            FeeId = hourlyFee.Id,
            StartDateTime = DateTime.Now,
            IsCompleted = false,
            AnonymousCustomersCount = request.AnonymousCustomersCount,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Sessions.Add(session);
        
        table.IsOccupied = true;
        table.LastModifiedDate = DateTime.Now;

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
                        CreateDate = DateTime.Now
                    };
                    _context.SessionCustomers.Add(sessionCustomer);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);

        return session.Id;
    }
}
