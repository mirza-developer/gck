using Microsoft.JSInterop;

namespace Gck.Services.Helpers;

public static class JSInterop
{
    public static async Task<bool> Confirm(string message)
    {
        // This is a placeholder - actual implementation requires IJSRuntime injection
        // For now, return true to proceed with operations
        return await Task.FromResult(true);
    }
}
