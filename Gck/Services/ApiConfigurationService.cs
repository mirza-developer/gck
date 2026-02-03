namespace Gck.Services;

public class ApiConfigurationService
{
#if RELEASE
    public string BaseApiUrl { get; set; } = "http://185.141.213.163:8080/";
#else
    public string BaseApiUrl { get; set; } = "http://localhost:5200";
#endif
    
    public string GetApiUrl(string endpoint)
    {
        return $"{BaseApiUrl}{endpoint}";
    }
}
