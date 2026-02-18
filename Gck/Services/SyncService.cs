using System.Net.Http.Json;
using System.Text.Json;

namespace Gck.Services;

/// <summary>
/// Service for synchronizing offline changes with the server
/// </summary>
public class SyncService
{
    private readonly HttpClient _httpClient;
    private readonly OfflineStorageService _offlineStorage;
    private readonly NetworkStatusService _networkStatus;
    private bool _isSyncing;

    public event EventHandler<SyncEventArgs>? SyncStarted;
    public event EventHandler<SyncEventArgs>? SyncCompleted;
    public event EventHandler<SyncEventArgs>? SyncFailed;

    public SyncService(
        HttpClient httpClient,
        OfflineStorageService offlineStorage,
        NetworkStatusService networkStatus)
    {
        _httpClient = httpClient;
        _offlineStorage = offlineStorage;
        _networkStatus = networkStatus;

        // Listen for network status changes
        _networkStatus.OnlineStatusChanged += async (sender, isOnline) =>
        {
            if (isOnline)
            {
                await SyncOfflineChangesAsync();
            }
        };
    }

    /// <summary>
    /// Sync all offline changes with the server
    /// </summary>
    public async Task<SyncResult> SyncOfflineChangesAsync()
    {
        if (_isSyncing)
        {
            return new SyncResult { Success = false, Message = "Sync already in progress" };
        }

        if (!_networkStatus.IsOnline)
        {
            return new SyncResult { Success = false, Message = "Device is offline" };
        }

        _isSyncing = true;
        SyncStarted?.Invoke(this, new SyncEventArgs { StartTime = DateTime.UtcNow });

        try
        {
            var changes = await _offlineStorage.GetQueuedChangesAsync();
            if (changes.Count == 0)
            {
                _isSyncing = false;
                return new SyncResult { Success = true, Message = "No changes to sync", SyncedCount = 0 };
            }

            var result = new SyncResult { TotalCount = changes.Count };

            foreach (var change in changes)
            {
                try
                {
                    var success = await SyncSingleChangeAsync(change);
                    if (success)
                    {
                        await _offlineStorage.RemoveChangeAsync(change.Id);
                        result.SyncedCount++;
                    }
                    else
                    {
                        result.FailedCount++;
                    }
                }
                catch (Exception ex)
                {
                    result.FailedCount++;
                    result.Errors.Add($"Change {change.Id}: {ex.Message}");
                }
            }

            await _offlineStorage.UpdateLastSyncTimeAsync();
            result.Success = result.FailedCount == 0;
            result.Message = $"Synced {result.SyncedCount} of {result.TotalCount} changes";

            SyncCompleted?.Invoke(this, new SyncEventArgs
            {
                StartTime = DateTime.UtcNow,
                SyncedCount = result.SyncedCount,
                FailedCount = result.FailedCount
            });

            return result;
        }
        catch (Exception ex)
        {
            SyncFailed?.Invoke(this, new SyncEventArgs { ErrorMessage = ex.Message });
            return new SyncResult { Success = false, Message = ex.Message };
        }
        finally
        {
            _isSyncing = false;
        }
    }

    /// <summary>
    /// Sync a single change with the server
    /// </summary>
    private async Task<bool> SyncSingleChangeAsync(OfflineChange change)
    {
        try
        {
            // Build the API endpoint based on entity type
            var endpoint = GetApiEndpoint(change.EntityType, change.EntityId, change.OperationType);

            HttpResponseMessage response;
            
            // Prepare the content - data is already JSON serialized
            var content = new StringContent(change.Data, System.Text.Encoding.UTF8, "application/json");

            switch (change.OperationType.ToLower())
            {
                case "create":
                    response = await _httpClient.PostAsync(endpoint, content);
                    break;
                case "update":
                    response = await _httpClient.PutAsync(endpoint, content);
                    break;
                case "delete":
                    response = await _httpClient.DeleteAsync(endpoint);
                    break;
                default:
                    return false;
            }

            return response.IsSuccessStatusCode;
        }
        catch
        {
            return false;
        }
    }

    /// <summary>
    /// Get the API endpoint for a given entity type and operation
    /// </summary>
    private string GetApiEndpoint(string entityType, string entityId, string operationType)
    {
        var baseUrl = $"/api/{entityType.ToLower()}";

        return operationType.ToLower() switch
        {
            "create" => baseUrl,
            "update" => $"{baseUrl}/{entityId}",
            "delete" => $"{baseUrl}/{entityId}",
            _ => baseUrl
        };
    }

    /// <summary>
    /// Get the count of pending changes
    /// </summary>
    public async Task<int> GetPendingChangesCountAsync()
    {
        return await _offlineStorage.GetPendingChangesCountAsync();
    }

    /// <summary>
    /// Check if sync is in progress
    /// </summary>
    public bool IsSyncing => _isSyncing;
}

/// <summary>
/// Result of a sync operation
/// </summary>
public class SyncResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public int TotalCount { get; set; }
    public int SyncedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = new();
}

/// <summary>
/// Event args for sync events
/// </summary>
public class SyncEventArgs : EventArgs
{
    public DateTime StartTime { get; set; }
    public int SyncedCount { get; set; }
    public int FailedCount { get; set; }
    public string? ErrorMessage { get; set; }
}
