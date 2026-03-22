using Gck.Application.DTOs;
using Microsoft.AspNetCore.Components;
using Gck.Services;

namespace Gck.Pages.Customer;

public partial class Credits
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private bool isLoading = true;
    private bool isSubmitting = false;
    private int currentCustomerId;
    private decimal creditBalance = 0;
    private decimal withdrawalAmount = 0;
    private string withdrawalNotes = string.Empty;
    private List<CreditWithdrawalRequestDto> withdrawals = new();
    private bool hasPendingRequest = false;
    private string successMsg = string.Empty;
    private string errorMsg = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var customerIdStr = await LocalStorage.GetItemAsync<string>("currentUserId");
        var userRole = await LocalStorage.GetItemAsync<string>("userRole");

        if (string.IsNullOrEmpty(customerIdStr) || userRole != "customer")
        {
            Navigation.NavigateTo("/login");
            return;
        }

        if (int.TryParse(customerIdStr, out var id))
        {
            currentCustomerId = id;
        }

        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;

        try
        {
            var customerResponse = await Http.GetAsync(ApiConfig.GetApiUrl($"/api/customers/{currentCustomerId}"));
            if (customerResponse.IsSuccessStatusCode)
            {
                var customer = await customerResponse.Content.ReadFromJsonAsync<CustomerInfoDto>();
                creditBalance = customer?.ReferralCredit ?? 0;
            }

            var withdrawalsResponse = await Http.GetAsync(
                ApiConfig.GetApiUrl($"/api/creditwithdrawals/customer/{currentCustomerId}"));
            if (withdrawalsResponse.IsSuccessStatusCode)
            {
                withdrawals = await withdrawalsResponse.Content.ReadFromJsonAsync<List<CreditWithdrawalRequestDto>>() ?? new();
                hasPendingRequest = withdrawals.Any(w => w.Status == "Pending");
            }
        }
        catch { }

        isLoading = false;
    }

    private async Task SubmitWithdrawal()
    {
        successMsg = string.Empty;
        errorMsg = string.Empty;

        if (withdrawalAmount <= 0 || withdrawalAmount > creditBalance)
        {
            errorMsg = Resources.PersianResources.InsufficientCredit;
            return;
        }

        isSubmitting = true;

        try
        {
            var command = new
            {
                CustomerId = currentCustomerId,
                Amount = withdrawalAmount,
                Notes = withdrawalNotes
            };

            var response = await Http.PostAsJsonAsync(
                ApiConfig.GetApiUrl("/api/creditwithdrawals"),
                command);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<WithdrawalResult>();
                if (result?.Success == true)
                {
                    successMsg = Resources.PersianResources.WithdrawalSubmitted;
                    withdrawalAmount = 0;
                    withdrawalNotes = string.Empty;
                    await LoadData();
                }
                else
                {
                    errorMsg = result?.Message ?? Resources.PersianResources.FailureMessage;
                }
            }
            else
            {
                errorMsg = Resources.PersianResources.FailureMessage;
            }
        }
        catch (Exception ex)
        {
            errorMsg = ex.Message;
        }
        finally
        {
            isSubmitting = false;
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

    private class CustomerInfoDto
    {
        public int Id { get; set; }
        public decimal ReferralCredit { get; set; }
    }

    private class WithdrawalResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
