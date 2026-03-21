using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.CreditWithdrawals;

public partial class Index
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private List<CreditWithdrawalRequestDto> requests = new();
    private bool isLoading = true;
    private string? statusFilter = "Pending";

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;

        var url = string.IsNullOrEmpty(statusFilter)
            ? ApiConfig.GetApiUrl("/api/creditwithdrawals")
            : ApiConfig.GetApiUrl($"/api/creditwithdrawals?status={statusFilter}");

        requests = await Http.GetFromJsonWithExceptionAsync<List<CreditWithdrawalRequestDto>>(url) ?? new();

        isLoading = false;
    }

    private async Task FilterAll() => await FilterByStatus(null);
    private async Task FilterPending() => await FilterByStatus("Pending");
    private async Task FilterApproved() => await FilterByStatus("Approved");
    private async Task FilterRejected() => await FilterByStatus("Rejected");
    private async Task ApproveRequest(int requestId) => await ProcessRequest(requestId, "Approve");
    private async Task RejectRequest(int requestId) => await ProcessRequest(requestId, "Reject");

    private async Task FilterByStatus(string? status)
    {
        statusFilter = status;
        await LoadData();
    }

    private async Task ProcessRequest(int requestId, string action)
    {
        var command = new { RequestId = requestId, Action = action, Notes = "" };

        var response = await Http.PostAsJsonAsync(
            ApiConfig.GetApiUrl($"/api/creditwithdrawals/{requestId}/process"),
            command);

        if (response.IsSuccessStatusCode)
        {
            NotificationService.ShowSuccess(Resources.PersianResources.WithdrawalProcessed);
            await LoadData();
        }
        else
        {
            NotificationService.ShowError(Resources.PersianResources.FailureMessage);
        }
    }

    private Microsoft.AspNetCore.Components.MarkupString GetStatusBadge(string status)
    {
        var (color, icon, text) = status switch
        {
            "Pending" => ("rgba(255,165,0,0.2); color: orange; border: 1px solid rgba(255,165,0,0.3)", "fa-clock", Resources.PersianResources.WithdrawalPending),
            "Approved" => ("rgba(0,200,0,0.2); color: #00cc00; border: 1px solid rgba(0,200,0,0.3)", "fa-check", Resources.PersianResources.WithdrawalApproved),
            "Rejected" => ("rgba(255,0,0,0.2); color: #ff4444; border: 1px solid rgba(255,0,0,0.3)", "fa-times", Resources.PersianResources.WithdrawalRejected),
            _ => ("rgba(128,128,128,0.2); color: gray", "fa-question", status)
        };

        return new Microsoft.AspNetCore.Components.MarkupString(
            $"<span class=\"grid-cell-badge\" style=\"background: {color};\">" +
            $"<i class=\"fas {icon}\"></i> {text}</span>");
    }
}
