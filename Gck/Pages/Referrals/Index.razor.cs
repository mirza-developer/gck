using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Referrals;

public partial class Index
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private List<CustomerDto> pendingReferrals = new();
    private List<CustomerDto> allReferredCustomers = new();
    private bool isLoading = true;
    private string searchText = string.Empty;
    private Dictionary<int, decimal> pendingPercentages = new();
    private Dictionary<int, decimal> verifiedPercentages = new();

    private List<CustomerDto> FilteredReferredCustomers =>
        allReferredCustomers
            .Where(c => c.IsVerifiedByAdmin)
            .Where(c => string.IsNullOrEmpty(searchText) ||
                c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                c.PhoneNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase))
            .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;

        pendingReferrals = await Http.GetFromJsonWithExceptionAsync<List<CustomerDto>>(
            ApiConfig.GetApiUrl("/api/customers/referrals/pending")) ?? new();

        allReferredCustomers = (await Http.GetFromJsonWithExceptionAsync<List<CustomerDto>>(
            ApiConfig.GetApiUrl("/api/customers")) ?? new())
            .Where(c => c.ReferredByCustomerId.HasValue)
            .ToList();

        // Initialize pending percentages
        pendingPercentages.Clear();
        foreach (var c in pendingReferrals)
        {
            pendingPercentages[c.Id] = 0;
        }

        isLoading = false;
    }

    private decimal GetPendingPercentage(int customerId)
    {
        return pendingPercentages.TryGetValue(customerId, out var pct) ? pct : 0;
    }

    private void SetPendingPercentage(int customerId, string? value)
    {
        if (decimal.TryParse(value, out var pct))
        {
            pendingPercentages[customerId] = Math.Max(0, Math.Min(100, pct));
        }
    }

    private void SetVerifiedPercentage(int customerId, string? value)
    {
        if (decimal.TryParse(value, out var pct))
        {
            var customer = allReferredCustomers.FirstOrDefault(c => c.Id == customerId);
            if (customer != null)
            {
                customer.ReferralRewardPercentage = Math.Max(0, Math.Min(100, pct));
            }
        }
    }

    private async Task VerifyReferral(int customerId)
    {
        var percentage = GetPendingPercentage(customerId);

        var response = await Http.PostAsJsonAsync(
            ApiConfig.GetApiUrl($"/api/customers/{customerId}/verify-referral"),
            new { CustomerId = customerId, ReferralRewardPercentage = percentage });

        if (response.IsSuccessStatusCode)
        {
            NotificationService.ShowSuccess(Resources.PersianResources.ReferralVerified);
            await LoadData();
        }
        else
        {
            NotificationService.ShowError(Resources.PersianResources.FailureMessage);
        }
    }

    private async Task SaveRewardPercentage(CustomerDto customer)
    {
        var command = new
        {
            Id = customer.Id,
            Name = customer.Name,
            PhoneNumber = customer.PhoneNumber,
            BirthYear = customer.BirthYear,
            Gender = customer.Gender,
            IsLoyal = customer.IsLoyal,
            SessionsRequiredForFree = customer.SessionsRequiredForFree,
            IsVerifiedByAdmin = customer.IsVerifiedByAdmin,
            ReferralRewardPercentage = customer.ReferralRewardPercentage
        };

        var response = await Http.PutAsJsonAsync(
            ApiConfig.GetApiUrl($"/api/customers/{customer.Id}"),
            command);

        if (response.IsSuccessStatusCode)
        {
            NotificationService.ShowSuccess(Resources.PersianResources.SaveSuccess);
        }
        else
        {
            NotificationService.ShowError(Resources.PersianResources.FailureMessage);
        }
    }
}
