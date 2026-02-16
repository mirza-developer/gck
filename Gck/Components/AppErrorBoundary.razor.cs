using System.Diagnostics;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Gck.Components;

public partial class AppErrorBoundary : ErrorBoundaryBase
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    
    private bool IsDebugMode => 
#if DEBUG
        true;
#else
        false;
#endif

    protected override Task OnErrorAsync(Exception exception)
    {
        if (IsDebugMode)
        {
            Debugger.Break();
        }

        Console.Error.WriteLine($"Error caught by ErrorBoundary: {exception.Message}");
        Console.Error.WriteLine($"Stack Trace: {exception.StackTrace}");
        
        return Task.CompletedTask;
    }

    private new void Recover()
    {
        base.Recover();
    }

    private async Task NavigateToHome()
    {
        await JS.InvokeVoidAsync("eval", "window.location.href = '/'");
    }
}
