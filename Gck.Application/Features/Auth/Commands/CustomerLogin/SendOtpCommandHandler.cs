using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Json;
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

        // Get SMS provider configuration
        var smsBaseUrl = _configuration["SmsProvider:BaseUrl"] ?? "https://console.melipayamak.com";
        var smsApiKey = _configuration["SmsProvider:ApiKey"];
        
        if (string.IsNullOrEmpty(smsApiKey))
        {
            // No SMS API configured, return test mode
            var testOtp = new Random().Next(100000, 999999).ToString();
            return new SendOtpResponse
            {
                Success = true,
                Message = "کد تایید ایجاد شد (حالت تست - SMS API پیکربندی نشده)",
#if DEBUG
                OtpCode = testOtp // Only include in debug builds
#endif
            };
        }

        // Send OTP via SMS API
        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(smsBaseUrl);
            
            var result = await client.PostAsJsonAsync($"api/send/otp/{smsApiKey}",
                new { to = request.PhoneNumber }, cancellationToken);
            var response = await result.Content.ReadAsStringAsync(cancellationToken);
            
            // Parse response to check if OTP was sent successfully
            if (!string.IsNullOrEmpty(response))
            {
                try
                {
                    var jsonDoc = JsonDocument.Parse(response);
                    if (jsonDoc.RootElement.TryGetProperty("code", out var codeElement))
                    {
                        var sentCode = codeElement.GetString();
                        return new SendOtpResponse
                        {
                            Success = true,
                            Message = "کد تایید برای شما ارسال شد",
#if DEBUG
                            OtpCode = sentCode // Only include in debug builds for testing
#endif
                        };
                    }
                }
                catch (JsonException)
                {
                    // If parsing fails, check if request was successful based on HTTP status
                    if (result.IsSuccessStatusCode)
                    {
                        return new SendOtpResponse
                        {
                            Success = true,
                            Message = "کد تایید برای شما ارسال شد"
                        };
                    }
                }
            }

            return new SendOtpResponse
            {
                Success = true,
                Message = "کد تایید برای شما ارسال شد"
            };
        }
        catch (HttpRequestException)
        {
            // Network or API error - fallback to test mode for development
            var testOtp = new Random().Next(100000, 999999).ToString();
            
            return new SendOtpResponse
            {
                Success = true,
                Message = "کد تایید ایجاد شد (حالت تست - خطا در اتصال به SMS API)",
#if DEBUG
                OtpCode = testOtp // Only include in debug builds
#endif
            };
        }
    }
}
