using Gck.Application.Services;
using System.Net.Http.Json;
using System.Text.Json;

namespace Gck.Api.Services;

public class SmsService : ISmsService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmsService> _logger;

    public SmsService(
        IHttpClientFactory httpClientFactory, 
        IConfiguration configuration,
        ILogger<SmsService> logger)
    {
        _httpClientFactory = httpClientFactory;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<string?> SendOtpAsync(string phoneNumber, CancellationToken cancellationToken = default)
    {
        var smsBaseUrl = _configuration["SmsProvider:BaseUrl"] ?? "https://console.melipayamak.com";
        var smsApiKey = _configuration["SmsProvider:ApiKey"];

        if (string.IsNullOrEmpty(smsApiKey))
        {
            _logger.LogWarning("SMS API Key is not configured. OTP will not be sent.");
            return null;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(smsBaseUrl);

            var result = await client.PostAsJsonAsync($"api/send/otp/{smsApiKey}",
                new { to = phoneNumber }, cancellationToken);

            result.EnsureSuccessStatusCode();

            var response = await result.Content.ReadAsStringAsync(cancellationToken);

            // Parse response to get the actual OTP code sent by the provider
            if (!string.IsNullOrEmpty(response))
            {
                var jsonDoc = JsonDocument.Parse(response);
                if (jsonDoc.RootElement.TryGetProperty("code", out var codeElement))
                {
                    return codeElement.GetString();
                }
            }

            _logger.LogWarning("OTP response did not contain a code property");
            return null;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send OTP to {PhoneNumber}", phoneNumber);
            throw;
        }
    }

    public async Task SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        var smsBaseUrl = _configuration["SmsProvider:BaseUrl"] ?? "https://console.melipayamak.com";
        var smsApiKey = _configuration["SmsProvider:SimpleMessageApiKey"];
        var fromNumber = _configuration["SmsProvider:FromNumber"] ?? "50004001381951";

        if (string.IsNullOrEmpty(smsApiKey))
        {
            _logger.LogWarning("SMS Simple Message API Key is not configured. Message will not be sent.");
            return;
        }

        try
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri(smsBaseUrl);

            var result = await client.PostAsJsonAsync($"api/send/simple/{smsApiKey}",
                new { from = fromNumber, to = phoneNumber, text = message }, cancellationToken);

            result.EnsureSuccessStatusCode();

            _logger.LogInformation("Message sent successfully to {PhoneNumber}", phoneNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message to {PhoneNumber}", phoneNumber);
            throw;
        }
    }
}
