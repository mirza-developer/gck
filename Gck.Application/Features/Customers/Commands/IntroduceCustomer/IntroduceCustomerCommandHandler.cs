using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Customers.Commands.IntroduceCustomer;

public class IntroduceCustomerCommandHandler : IRequestHandler<IntroduceCustomerCommand, IntroduceCustomerResult>
{
    private readonly GckDbContext _context;

    public IntroduceCustomerCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<IntroduceCustomerResult> Handle(IntroduceCustomerCommand request, CancellationToken cancellationToken)
    {
        // Verify referrer exists
        var referrer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.ReferrerCustomerId, cancellationToken);

        if (referrer == null)
        {
            return new IntroduceCustomerResult
            {
                Success = false,
                Message = "مشتری معرف یافت نشد"
            };
        }

        // Check if phone number already exists
        var existing = await _context.Customers
            .FirstOrDefaultAsync(c => c.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (existing != null)
        {
            return new IntroduceCustomerResult
            {
                Success = false,
                Message = "این شماره تلفن قبلاً در سیستم ثبت شده است"
            };
        }

        // Create the new customer - unverified, pending admin approval
        var newCustomer = new Customer
        {
            Name = request.Name,
            PhoneNumber = request.PhoneNumber,
            BirthYear = request.BirthYear,
            IsMale = request.Gender.Equals("Male", StringComparison.OrdinalIgnoreCase),
            IsLoyal = false,
            SessionsRequiredForFree = 0,
            PaidSessionsCount = 0,
            ReferredByCustomerId = request.ReferrerCustomerId,
            IsVerifiedByAdmin = false,
            ReferralCredit = 0,
            ReferralRewardPercentage = 0,
            CreateDate = DateTime.Now,
            LastModifiedDate = DateTime.Now
        };

        _context.Customers.Add(newCustomer);
        await _context.SaveChangesAsync(cancellationToken);

        return new IntroduceCustomerResult
        {
            Success = true,
            Message = "درخواست معرفی با موفقیت ثبت شد و منتظر تایید مدیر است",
            NewCustomerId = newCustomer.Id
        };
    }
}
