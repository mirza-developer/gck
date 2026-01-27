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

        // Verify OTP code length
        if (string.IsNullOrEmpty(request.OtpCode) || request.OtpCode.Length != 6 || !request.OtpCode.All(char.IsDigit))
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید نامعتبر است"
            };
        }

        // IMPORTANT: In a production environment, you should verify the OTP with the SMS provider's API
        // or implement a secure OTP storage mechanism with expiration.
        // Current implementation accepts any valid 6-digit code for testing purposes.
        // 
        // Recommended production approaches:
        // 1. Call SMS provider's verification endpoint with phone number and OTP
        // 2. Store OTP in a cache (Redis) with TTL and verify against it
        // 3. Use SMS provider's built-in verification webhook

        // OTP is valid (simplified for testing - see comments above)
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
