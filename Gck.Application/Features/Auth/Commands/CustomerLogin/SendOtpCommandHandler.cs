using Gck.Persistence;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gck.Application.Features.Auth.Commands.CustomerLogin;

public class SendOtpCommandHandler : IRequestHandler<SendOtpCommand, SendOtpResponse>
{
    private readonly GckDbContext _context;

    public SendOtpCommandHandler(GckDbContext context)
    {
        _context = context;
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

        // Generate 6-digit OTP
        var random = new Random();
        var otpCode = random.Next(100000, 999999).ToString();

        // Store OTP in database (expires in 5 minutes)
        customer.LastOtpCode = otpCode;
        customer.OtpExpiry = DateTime.Now.AddMinutes(5);
        await _context.SaveChangesAsync(cancellationToken);

        // Send OTP via SMS API
        try
        {
            Uri apiBaseAddress = new Uri("https://console.melipayamak.com");
            using (HttpClient client = new HttpClient() { BaseAddress = apiBaseAddress })
            {
                var result = await client.PostAsJsonAsync("api/send/otp/f46fefd347444ded90bda092cde7f6f2",
                    new { to = request.PhoneNumber }, cancellationToken);
                var response = await result.Content.ReadAsStringAsync(cancellationToken);
                
                // Parse response to get the actual OTP code sent
                if (!string.IsNullOrEmpty(response))
                {
                    try
                    {
                        var jsonDoc = JsonDocument.Parse(response);
                        if (jsonDoc.RootElement.TryGetProperty("code", out var codeElement))
                        {
                            var sentCode = codeElement.GetString();
                            // Update with the actual code sent by the API
                            customer.LastOtpCode = sentCode;
                            await _context.SaveChangesAsync(cancellationToken);
                        }
                    }
                    catch
                    {
                        // If parsing fails, use our generated code
                    }
                }
            }

            return new SendOtpResponse
            {
                Success = true,
                Message = "کد تایید برای شما ارسال شد",
                OtpCode = otpCode // Include for testing, remove in production
            };
        }
        catch (Exception ex)
        {
            // Even if SMS fails, return success for testing purposes
            // In production, you should handle this differently
            return new SendOtpResponse
            {
                Success = true,
                Message = "کد تایید ایجاد شد (خطا در ارسال پیامک)",
                OtpCode = otpCode // For testing when SMS API is not available
            };
        }
    }
}
