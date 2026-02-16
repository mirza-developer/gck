using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Common.Helpers;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Transactions;

public partial class Index : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private List<TransactionDto> transactions = new();
    private List<FinancialAccountDto> accounts = new();
    private List<FinancialAccountDto> allAccounts = new();
    private List<TransactionTypeOption> transactionTypes = new();
    private TransactionReportDto? report;
    private string searchText = string.Empty;
    private string filterType = string.Empty;
    private string filterAccountId = string.Empty;
    private string filterStartDate = string.Empty;
    private string filterEndDate = string.Empty;
    private bool isLoading = true;

    private class TransactionTypeOption
    {
        public string Value { get; set; } = string.Empty;
        public string Display { get; set; } = string.Empty;
    }

    private IEnumerable<TransactionDto> FilteredTransactions => 
        report?.Transactions
            .Where(t => string.IsNullOrEmpty(searchText) || 
                       t.Description.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
                       t.FinancialAccountName.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        ?? Enumerable.Empty<TransactionDto>();

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;

        accounts = await Http.GetFromJsonWithExceptionAsync<List<FinancialAccountDto>>(ApiConfig.GetApiUrl("/api/financialaccounts")) ?? new();
        allAccounts = new List<FinancialAccountDto>(accounts);

        transactionTypes = new List<TransactionTypeOption>
        {
            new TransactionTypeOption { Value = "", Display = PersianResources.AllTypes },
            new TransactionTypeOption { Value = "Income", Display = PersianResources.Income },
            new TransactionTypeOption { Value = "Outcome", Display = PersianResources.Outcome }
        };

        await ApplyFilters();

        isLoading = false;
    }

    private async Task ApplyFilters()
    {
        isLoading = true;

        var queryParams = new List<string>();

        if (!string.IsNullOrEmpty(filterAccountId))
            queryParams.Add($"financialAccountId={filterAccountId}");

        if (!string.IsNullOrEmpty(filterType))
            queryParams.Add($"type={filterType}");

        if (!string.IsNullOrEmpty(filterStartDate))
        {
            var startDate = PersianDateHelper.FromPersianDate(filterStartDate);
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
        }

        if (!string.IsNullOrEmpty(filterEndDate))
        {
            var endDate = PersianDateHelper.FromPersianDate(filterEndDate);
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
        }

        var queryString = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
        report = await Http.GetFromJsonWithExceptionAsync<TransactionReportDto>(ApiConfig.GetApiUrl($"/api/transactions/report{queryString}"));

        isLoading = false;
    }

    private async Task ClearFilters()
    {
        filterAccountId = string.Empty;
        filterType = string.Empty;
        filterStartDate = string.Empty;
        filterEndDate = string.Empty;
        await ApplyFilters();
    }

    private void OnAccountSelected(string? value)
    {
        filterAccountId = value ?? string.Empty;
    }

    private void OnTypeSelected(string? value)
    {
        filterType = value ?? string.Empty;
    }

    private void OnStartDateChanged(string value)
    {
        filterStartDate = value;
    }

    private void OnEndDateChanged(string value)
    {
        filterEndDate = value;
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/transactions/add");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/transactions/edit/{id}");
    }

    private async Task DeleteTransaction(int id)
    {
        await Http.DeleteWithExceptionAsync(ApiConfig.GetApiUrl($"/api/transactions/{id}"));

        NotificationService.ShowSuccess(PersianResources.DeleteSuccess);
        await ApplyFilters();
    }
}
