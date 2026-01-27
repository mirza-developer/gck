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

        // Verify OTP code
        // Note: In production, you would verify the OTP with the SMS provider's API
        // For now, we accept any 6-digit code for testing purposes
        if (string.IsNullOrEmpty(request.OtpCode) || request.OtpCode.Length != 6)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید نامعتبر است"
            };
        }

        // OTP is valid (in production, verify with SMS provider)
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
