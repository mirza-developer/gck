using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Sessions.Commands.FinishSession;

public class FinishSessionCommandHandler : IRequestHandler<FinishSessionCommand, Unit>
{
    private readonly GckDbContext _context;

    public FinishSessionCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<Unit> Handle(FinishSessionCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.Sessions
            .Include(s => s.Table)
            .Include(s => s.Fee)
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
