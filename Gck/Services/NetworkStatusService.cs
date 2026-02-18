using Microsoft.JSInterop;

namespace Gck.Services;

/// <summary>
/// Service for monitoring network connectivity status
/// </summary>
public class NetworkStatusService : IAsyncDisposable
{
    private readonly IJSRuntime _jsRuntime;
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

    public NetworkStatusService(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    /// <summary>
    /// Initialize network status monitoring
    /// </summary>
    public async Task InitializeAsync()
    {
        try
        {
            _module = await _jsRuntime.InvokeAsync<IJSObjectReference>(
                "import", "./js/network-status.js");

            _dotNetReference = DotNetObjectReference.Create(this);

            await _module.InvokeVoidAsync("initialize", _dotNetReference);

            // Get initial online status
            _isOnline = await _module.InvokeAsync<bool>("isOnline");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to initialize network status: {ex.Message}");
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
    /// Check current network status
    /// </summary>
    public async Task<bool> CheckNetworkStatusAsync()
    {
        try
        {
            if (_module != null)
            {
                _isOnline = await _module.InvokeAsync<bool>("isOnline");
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
            await _module.DisposeAsync();
        }

        _dotNetReference?.Dispose();
    }
}
