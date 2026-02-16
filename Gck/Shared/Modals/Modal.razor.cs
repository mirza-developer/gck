using Microsoft.AspNetCore.Components;

namespace Gck.Shared.Modals;

public partial class Modal
{
    [Parameter] public string Title { get; set; } = string.Empty;
    
    [Parameter] public RenderFragment? ChildContent { get; set; }
    
    [Parameter] public RenderFragment? FooterContent { get; set; }
    
    [Parameter] public bool IsVisible { get; set; }
    
    [Parameter] public EventCallback<bool> IsVisibleChanged { get; set; }
    
    [Parameter] public EventCallback OnConfirm { get; set; }
    
    [Parameter] public EventCallback OnCancel { get; set; }
    
    [Parameter] public ModalSize Size { get; set; } = ModalSize.Medium;
    
    [Parameter] public bool ShowCloseButton { get; set; } = true;
    
    [Parameter] public bool ShowFooter { get; set; } = true;
    
    [Parameter] public bool ShowConfirmButton { get; set; } = true;
    
    [Parameter] public bool ShowCancelButton { get; set; } = true;
    
    [Parameter] public string ConfirmText { get; set; } = "?????";
    
    [Parameter] public string CancelText { get; set; } = "??????";
    
    [Parameter] public bool CloseOnOverlayClick { get; set; } = true;

    private async Task Close()
    {
        IsVisible = false;
        await IsVisibleChanged.InvokeAsync(IsVisible);
    }

    private async Task Confirm()
    {
        await OnConfirm.InvokeAsync();
        await Close();
    }

    private async Task Cancel()
    {
        await OnCancel.InvokeAsync();
        await Close();
    }

    private async Task OnOverlayClick()
    {
        if (CloseOnOverlayClick)
        {
            await Close();
        }
    }
}

public enum ModalSize
{
    Small,
    Medium,
    Large,
    ExtraLarge
}
