using Microsoft.JSInterop;

namespace Gck.Services;

/// <summary>
/// Service for monitoring network connectivity status based on API accessibility
/// </summary>
public class NetworkStatusService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
    private readonly ApiConfigurationService _apiConfig;
    private IJSObjectReference? _module;
    private DotNetObjectReference<NetworkStatusService>? _dotNetReference;

    public event EventHandler<bool>? OnlineStatusChanged;
    
    private bool _isOnline = true;
    public bool IsOnline
    {
        get => _isOnline;
        private set
        {
            if (_isOnline != value)
            {
                _isOnline = value;
                OnlineStatusChanged?.Invoke(this, value);
            }
        }
    }

    public NetworkStatusService(IJSRuntime jsRuntime, ApiConfigurationService apiConfig)
    {
        _jsRuntime = jsRuntime;
        _apiConfig = apiConfig;
    }

    /// <summary>
    /// Initialize network status monitoring with API health checks
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/network-status.js");

            _dotNetReference = DotNetObjectReference.Create(this);

            // Pass the API base URL for health checks
            await _module.InvokeVoidAsync("initialize", _dotNetReference, _apiConfig.BaseApiUrl);

            // Get initial online status
            _isOnline = await _module.InvokeAsync<bool>("isOnline");
        }
        catch (Exception ex)
        {
            // Log the failure more prominently
            Console.Error.WriteLine($"[NetworkStatusService] Failed to initialize network status monitoring: {ex.Message}");
            Console.Error.WriteLine("[NetworkStatusService] Assuming online status - network detection will be degraded");
            // Assume online if initialization fails
            _isOnline = true;
        }
    }

    /// <summary>
    /// Called from JavaScript when network status changes
    /// </summary>
    [JSInvokable]
    public void UpdateNetworkStatus(bool isOnline)
    {
        IsOnline = isOnline;
    }

    /// <summary>
    /// Manually trigger an immediate network status check
    /// </summary>
    public async Task<bool> CheckNetworkStatusAsync()
    {
        try
        {
            if (_module != null)
            {
                await _module.InvokeVoidAsync("checkNow");
                // Wait a moment for the check to complete
                await Task.Delay(500);
            }
            return _isOnline;
        }
        catch
        {
            return _isOnline;
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_module != null)
        {
            try
            {
                await _module.InvokeVoidAsync("dispose");
            }
            catch
            {
                // Ignore disposal errors
            }
            await _module.DisposeAsync();
        }

        _dotNetReference?.Dispose();
    }
}
