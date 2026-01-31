namespace Gck.Services;

public class ApiConfigurationService
{
    public string BaseApiUrl { get; set; } = "http://localhost:5200";
    
    public string GetApiUrl(string endpoint)
    {
        return $"{BaseApiUrl}{endpoint}";
    }
}
