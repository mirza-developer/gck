# SMS Configuration Setup

## Initial Setup

1. Copy the example configuration file:
   ```bash
   cp appsettings.Example.json appsettings.json
   ```

2. Edit `appsettings.json` and update the SMS provider settings:
   - `ApiKey`: Your Melipayamak API key (used for both OTP and simple messages)
   - `FromNumber`: Your sender phone number

## Configuration Structure

```json
{
  "SmsProvider": {
    "BaseUrl": "https://console.melipayamak.com",
    "ApiKey": "your-api-key-here",
    "FromNumber": "your-sender-number-here"
  }
}
```

## Security Notes

- `appsettings.json` is excluded from git to prevent committing sensitive API keys
- Always use `appsettings.Example.json` as a template
- Never commit actual API keys to version control
- The same `ApiKey` is used for both OTP and simple message sending

## Usage

The SMS service is automatically registered in the dependency injection container and can be used via the `ISmsService` interface:

- `SendOtpAsync(phoneNumber)` - Sends an OTP code
- `SendMessageAsync(phoneNumber, message)` - Sends a simple text message
