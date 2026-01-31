namespace Gck.Application.Services;

public interface ISmsService
{
    /// <summary>
    /// Sends an OTP code to the specified phone number
    /// </summary>
    /// <param name="phoneNumber">The recipient phone number</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The OTP code that was sent</returns>
    Task<string?> SendOtpAsync(string phoneNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Sends a simple text message to the specified phone number
    /// </summary>
    /// <param name="phoneNumber">The recipient phone number</param>
    /// <param name="message">The message text to send</param>
    /// <param name="cancellationToken">Cancellation token</param>
    Task SendMessageAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);
}
