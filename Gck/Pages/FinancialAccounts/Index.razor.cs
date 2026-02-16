using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.FinancialAccounts;

public partial class Index : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private List<FinancialAccountDto> accounts = new();
    private string searchText = string.Empty;
    private bool isLoading = true;

    private List<FinancialAccountDto> FilteredAccounts =>
        accounts.Where(a =>
            string.IsNullOrEmpty(searchText) ||
            a.AccountName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            a.BankName.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            a.CardNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadAccounts();
    }

    private async Task LoadAccounts()
    {
        isLoading = true;

        accounts = await Http.GetFromJsonWithExceptionAsync<List<FinancialAccountDto>>(ApiConfig.GetApiUrl("/api/financialaccounts")) ?? new();

        isLoading = false;
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/financialaccounts/add");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/financialaccounts/edit/{id}");
    }

    private async Task DeleteAccount(int id)
    {
        var account = accounts.FirstOrDefault(a => a.Id == id);
        if (account != null && account.Balance > 0)
        {
            NotificationService.ShowError(PersianResources.CannotDeleteAccountWithBalance);
            return;
        }

        await Http.DeleteWithExceptionAsync(ApiConfig.GetApiUrl($"/api/financialaccounts/{id}"));
        NotificationService.ShowSuccess(PersianResources.DeleteSuccess);
        await LoadAccounts();
    }
}
