namespace Gck.Services;

public class ApiConfigurationService
{
#if RELEASE
    public string BaseApiUrl { get; set; } = "https://api.gckgames.ir";
#else
    public string BaseApiUrl { get; set; } = "http://localhost:5200";
#endif
    
    public string GetApiUrl(string endpoint)
    {
        return $"{BaseApiUrl}{endpoint}";
    }
}
