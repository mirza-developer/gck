namespace Gck.Services;

public class ApiConfigurationService
{
    public string BaseApiUrl { get; set; } = "https://localhost:7023";
    
    public string GetApiUrl(string endpoint)
    {
        return $"{BaseApiUrl}{endpoint}";
    }
}
