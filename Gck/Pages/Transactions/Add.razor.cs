using System.Globalization;
using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Transactions;

public partial class Add : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private CreateTransactionDto transaction = new();
    private List<FinancialAccountDto> accounts = new();
    private bool isLoading = true;
    private bool isSaving = false;
    private readonly PersianCalendar _persianCalendar = new();

    private string GetCurrentPersianDate()
    {
        var now = DateTime.Now;
        var year = _persianCalendar.GetYear(now);
        var month = _persianCalendar.GetMonth(now);
        var day = _persianCalendar.GetDayOfMonth(now);
        return $"{year:0000}/{month:00}/{day:00}";
    }

    protected override async Task OnInitializedAsync()
    {
        transaction.TransactionDate = GetCurrentPersianDate();
        await LoadFinancialAccounts();
    }

    private async Task LoadFinancialAccounts()
    {
        isLoading = true;

        accounts = await Http.GetFromJsonWithExceptionAsync<List<FinancialAccountDto>>(ApiConfig.GetApiUrl("/api/financialaccounts")) ?? new();

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (transaction.FinancialAccountId == 0)
        {
            NotificationService.ShowWarning(PersianResources.PleaseSelectFinancialAccount);
            return;
        }

        if (string.IsNullOrEmpty(transaction.Type))
        {
            NotificationService.ShowWarning(PersianResources.PleaseSelectTransactionType);
            return;
        }

        isSaving = true;

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/transactions"), transaction);

        NotificationService.ShowSuccess(PersianResources.TransactionAddSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/transactions");

        isSaving = false;
    }

    private void GoBack()
    {
        Navigation.NavigateTo("/transactions");
    }
}
