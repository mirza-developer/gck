using System.Text.Json;
using Blazored.LocalStorage;

namespace Gck.Services;

/// <summary>
/// Service for managing offline data changes and synchronization
/// </summary>
public class OfflineStorageService
{
    private readonly ILocalStorageService _localStorage;
    private const string OfflineChangesKey = "offline-changes";
    private const string LastSyncKey = "last-sync-time";

    public OfflineStorageService(ILocalStorageService localStorage)
    {
        _localStorage = localStorage;
    }

    /// <summary>
    /// Queue a change to be synced when online
    /// </summary>
    public async Task QueueChangeAsync(OfflineChange change)
    {
        var changes = await GetQueuedChangesAsync();
        change.Timestamp = DateTime.UtcNow;
        change.Id = Guid.NewGuid().ToString();
        changes.Add(change);
        await _localStorage.SetItemAsync(OfflineChangesKey, changes);
    }

    /// <summary>
    /// Get all queued changes
    /// </summary>
    public async Task<List<OfflineChange>> GetQueuedChangesAsync()
    {
        try
        {
            var changes = await _localStorage.GetItemAsync<List<OfflineChange>>(OfflineChangesKey);
            return changes ?? new List<OfflineChange>();
        }
        catch
        {
            return new List<OfflineChange>();
        }
    }

    /// <summary>
    /// Remove a specific change from the queue
    /// </summary>
    public async Task RemoveChangeAsync(string changeId)
    {
        var changes = await GetQueuedChangesAsync();
        changes.RemoveAll(c => c.Id == changeId);
        await _localStorage.SetItemAsync(OfflineChangesKey, changes);
    }

    /// <summary>
    /// Clear all queued changes
    /// </summary>
    public async Task ClearQueueAsync()
    {
        await _localStorage.RemoveItemAsync(OfflineChangesKey);
    }

    /// <summary>
    /// Get the count of pending changes
    /// </summary>
    public async Task<int> GetPendingChangesCountAsync()
    {
        var changes = await GetQueuedChangesAsync();
        return changes.Count;
    }

    /// <summary>
    /// Update last sync time
    /// </summary>
    public async Task UpdateLastSyncTimeAsync()
    {
        await _localStorage.SetItemAsync(LastSyncKey, DateTime.UtcNow);
    }

    /// <summary>
    /// Get last sync time
    /// </summary>
    public async Task<DateTime?> GetLastSyncTimeAsync()
    {
        try
        {
            return await _localStorage.GetItemAsync<DateTime?>(LastSyncKey);
        }
        catch
        {
            return null;
        }
    }
}

/// <summary>
/// Represents a change made while offline
/// </summary>
public class OfflineChange
{
    public string Id { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string OperationType { get; set; } = string.Empty; // Create, Update, Delete
    public string Data { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
}
