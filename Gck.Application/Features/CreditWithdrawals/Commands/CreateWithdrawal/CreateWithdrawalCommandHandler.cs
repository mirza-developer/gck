using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.CreditWithdrawals.Commands.CreateWithdrawal;

public class CreateWithdrawalCommandHandler : IRequestHandler<CreateWithdrawalCommand, CreateWithdrawalResult>
{
    private readonly GckDbContext _context;

    public CreateWithdrawalCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<CreateWithdrawalResult> Handle(CreateWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId, cancellationToken);

        if (customer == null)
        {
            return new CreateWithdrawalResult
            {
                Success = false,
                Message = "مشتری یافت نشد"
            };
        }

        if (customer.ReferralCredit < request.Amount || request.Amount <= 0)
        {
            return new CreateWithdrawalResult
            {
                Success = false,
                Message = "موجودی اعتبار کافی نیست"
            };
        }

        // Check no pending request exists
        var hasPending = await _context.CreditWithdrawalRequests
            .AnyAsync(r => r.CustomerId == request.CustomerId && r.Status == "Pending", cancellationToken);

        if (hasPending)
        {
            return new CreateWithdrawalResult
            {
                Success = false,
                Message = "یک درخواست برداشت در انتظار بررسی دارید"
            };
        }

        var withdrawalRequest = new CreditWithdrawalRequest
        {
            CustomerId = request.CustomerId,
            Amount = request.Amount,
            RequestDate = DateTime.Now,
            Status = "Pending",
            Notes = request.Notes,
            CreateDate = DateTime.Now
        };

        _context.CreditWithdrawalRequests.Add(withdrawalRequest);
        await _context.SaveChangesAsync(cancellationToken);

        return new CreateWithdrawalResult
        {
            Success = true,
            Message = "درخواست برداشت با موفقیت ثبت شد",
            RequestId = withdrawalRequest.Id
        };
    }
}
