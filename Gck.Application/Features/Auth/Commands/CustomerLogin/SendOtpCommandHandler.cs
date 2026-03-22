using Gck.Application.Services;
using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace Gck.Application.Features.Auth.Commands.CustomerLogin;

public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, SendOtpResponse>
{
    private readonly GckDbContext _context;
    private readonly ISmsService _smsService;

    public SendOtpCommandHandler(GckDbContext context, ISmsService smsService)
    {
        _context = context;
        _smsService = smsService;
    }

    public async Task<SendOtpResponse> Handle(SendOtpCommand request, CancellationToken cancellationToken)
    {
        // Find customer by phone number
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.PhoneNumber == request.PhoneNumber, cancellationToken);

        if (customer == null)
        {
            return new SendOtpResponse
            {
                Success = false,
                Message = "مشتری با این شماره تلفن یافت نشد"
            };
        }

        if (!customer.IsVerifiedByAdmin)
        {
            return new SendOtpResponse
            {
                Success = false,
                Message = "حساب کاربری شما هنوز توسط مدیر تایید نشده است"
            };
        }

        // Generate cryptographically secure 6-digit OTP
        var otpCode = GenerateSecureOtp();

        // Send OTP via SMS service
        string? actualOtpSent = await _smsService.SendOtpAsync(request.PhoneNumber, cancellationToken);

        // Use the OTP from SMS provider if available, otherwise use generated one
        var otpToStore = actualOtpSent ?? otpCode;

        // Clean up old expired OTPs for this phone number to prevent database bloat
        var expiredOtps = await _context.CustomerOtps
            .Where(o => o.PhoneNumber == request.PhoneNumber && o.ExpiresAt < DateTime.UtcNow)
            .ToListAsync(cancellationToken);
        
        if (expiredOtps.Any())
        {
            _context.CustomerOtps.RemoveRange(expiredOtps);
        }

        // Store OTP in database with 1-minute expiration
        var otpEntity = new CustomerOtp
        {
            PhoneNumber = request.PhoneNumber,
            OtpCode = otpToStore,
            CreatedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddMinutes(1),
            IsUsed = false
        };

        _context.CustomerOtps.Add(otpEntity);
        await _context.SaveChangesAsync(cancellationToken);

        return new SendOtpResponse
        {
            Success = true,
            Message = "کد تایید برای شما ارسال شد"
        };
    }

    private static string GenerateSecureOtp()
    {
        // Generate cryptographically secure random 6-digit OTP
        var randomNumber = RandomNumberGenerator.GetInt32(100000, 1000000);
        return randomNumber.ToString();
    }
}
