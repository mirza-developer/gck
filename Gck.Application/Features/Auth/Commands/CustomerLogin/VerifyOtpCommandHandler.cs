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

        // Verify OTP code length and format
        if (string.IsNullOrEmpty(request.OtpCode) || request.OtpCode.Length != 6 || !request.OtpCode.All(char.IsDigit))
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید نامعتبر است"
            };
        }

        // Find the most recent unused OTP for this phone number
        var otpEntity = await _context.CustomerOtps
            .Where(o => o.PhoneNumber == request.PhoneNumber && !o.IsUsed)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otpEntity == null)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید یافت نشد. لطفا ابتدا کد را درخواست کنید"
            };
        }

        // Check if OTP has expired
        if (otpEntity.ExpiresAt < DateTime.UtcNow)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید منقضی شده است"
            };
        }

        // Verify OTP code
        if (otpEntity.OtpCode != request.OtpCode)
        {
            return new VerifyOtpResponse
            {
                Success = false,
                Message = "کد تایید نامعتبر است"
            };
        }

        // Mark OTP as used to prevent reuse
        otpEntity.IsUsed = true;
        await _context.SaveChangesAsync(cancellationToken);

        // OTP is valid and verified
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
