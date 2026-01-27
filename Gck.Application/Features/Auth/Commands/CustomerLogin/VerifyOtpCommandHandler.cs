using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Gck.Application.Features.Auth.Commands.CustomerLogin;

public class VerifyOtpCommandHandler : IRequestHandler<VerifyOtpCommand, VerifyOtpResponse>
{
    private readonly GckDbContext _context;

    public VerifyOtpCommandHandler(GckDbContext context)
    {
        _context = context;
    }

    public async Task<VerifyOtpResponse> Handle(VerifyOtpCommand request, CancellationToken cancellationToken)
    {
        // Find customer by phone number
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (customer == null)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "مشتری با این شماره تلفن یافت نشد"
            };
        }

        // Check if OTP exists
        if (string.IsNullOrEmpty(customer.LastOtpCode))
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید یافت نشد. لطفا ابتدا کد را درخواست کنید"
            };
        }

        // Check if OTP has expired
        if (customer.OtpExpiry == null || customer.OtpExpiry < DateTime.Now)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید منقضی شده است"
            };
        }

        // Verify OTP code
        if (customer.LastOtpCode != request.OtpCode)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید نامعتبر است"
            };
        }

        // OTP is valid, clear it to prevent reuse
        customer.LastOtpCode = null;
        customer.OtpExpiry = null;
        await _context.SaveChangesAsync(cancellationToken);

        return new VerifyOtpResponse
        {
            Success = true,
            Message = "ورود موفقیت‌آمیز بود",
            CustomerId = customer.Id,
            CustomerName = customer.Name,
            PhoneNumber = customer.PhoneNumber
        };
    }
}
