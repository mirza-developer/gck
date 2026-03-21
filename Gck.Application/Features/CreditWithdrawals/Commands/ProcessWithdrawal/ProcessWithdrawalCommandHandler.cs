using Gck.Application.Services;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Gck.Application.Features.CreditWithdrawals.Commands.ProcessWithdrawal;

public class ProcessWithdrawalCommandHandler : IRequestHandler<ProcessWithdrawalCommand, Unit>
{
    private readonly GckDbContext _context;
    private readonly ISmsService _smsService;
    private readonly ILogger<ProcessWithdrawalCommandHandler> _logger;

    public ProcessWithdrawalCommandHandler(GckDbContext context, ISmsService smsService, ILogger<ProcessWithdrawalCommandHandler> logger)
    {
        _context = context;
        _smsService = smsService;
        _logger = logger;
    }

    public async Task<Unit> Handle(ProcessWithdrawalCommand request, CancellationToken cancellationToken)
    {
        var withdrawalRequest = await _context.CreditWithdrawalRequests
            .Include(r => r.Customer)
            .FirstOrDefaultAsync(r => r.Id == request.RequestId, cancellationToken);

        if (withdrawalRequest == null)
        {
            throw new InvalidOperationException($"Withdrawal request with ID '{request.RequestId}' not found");
        }

        if (withdrawalRequest.Status != "Pending")
        {
            throw new InvalidOperationException($"Withdrawal request with ID '{request.RequestId}' is not pending");
        }

        if (request.Action == "Approve")
        {
            var customer = withdrawalRequest.Customer;

            if (customer.ReferralCredit < withdrawalRequest.Amount)
            {
                throw new InvalidOperationException("موجودی اعتبار مشتری کافی نیست");
            }

            customer.ReferralCredit -= withdrawalRequest.Amount;
            customer.LastModifiedDate = DateTime.Now;

            withdrawalRequest.Status = "Approved";
            withdrawalRequest.ProcessedDate = DateTime.Now;
            withdrawalRequest.Notes = request.Notes;

            await _context.SaveChangesAsync(cancellationToken);

            // Send SMS notification
            if (!string.IsNullOrWhiteSpace(customer.PhoneNumber))
            {
                try
                {
                    string message = $"{customer.Name} عزیز\n" +
                                   $"درخواست برداشت نقدی شما به مبلغ {withdrawalRequest.Amount:N0} تومان تایید و پرداخت شد.\n" +
                                   "گیم سنتر کوثر";
                    await _smsService.SendMessageAsync(customer.PhoneNumber, message, cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send withdrawal approval SMS to customer {CustomerId}", customer.Id);
                }
            }
        }
        else
        {
            withdrawalRequest.Status = "Rejected";
            withdrawalRequest.ProcessedDate = DateTime.Now;
            withdrawalRequest.Notes = request.Notes;

            await _context.SaveChangesAsync(cancellationToken);
        }

        return Unit.Value;
    }
}
