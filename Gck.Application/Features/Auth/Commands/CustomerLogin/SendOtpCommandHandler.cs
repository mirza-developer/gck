using Gck.Domain.Entities;
using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
using System.Security.Cryptography;
using System.Text.Json;

namespace Gck.Application.Features.Auth.Commands.CustomerLogin;

public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, SendOtpResponse>
{
    private readonly GckDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly IHttpClientFactory _httpClientFactory;

    public SendOtpCommandHandler(GckDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _context = context;
        _configuration = configuration;
        _httpClientFactory = httpClientFactory;
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

        // Generate cryptographically secure 6-digit OTP
        var otpCode = GenerateSecureOtp();

        // Get SMS provider configuration
        var smsBaseUrl = _configuration["SmsProvider:BaseUrl"] ?? "https://console.melipayamak.com";
        var smsApiKey = _configuration["SmsProvider:ApiKey"];
        
        string? actualOtpSent = null;

        // Try to send OTP via SMS API if configured
        if (!string.IsNullOrEmpty(smsApiKey))
        {
            try
            {
                var client = _httpClientFactory.CreateClient();
                client.BaseAddress = new Uri(smsBaseUrl);
                
                var result = await client.PostAsJsonAsync($"api/send/otp/{smsApiKey}",
                    new { to = request.PhoneNumber }, cancellationToken);
                var response = await result.Content.ReadAsStringAsync(cancellationToken);
                
                // Parse response to get the actual OTP code sent by the provider
                if (!string.IsNullOrEmpty(response))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(response);
                        if (jsonDoc.RootElement.TryGetProperty("code", out var codeElement))
                        {
                            actualOtpSent = codeElement.GetString();
                        }
                    }
                    catch (JsonException)
                    {
                        // If parsing fails, use our generated code
                    }
                }
            }
            catch (HttpRequestException)
            {
                // Network error - will use our generated code
            }
        }

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
