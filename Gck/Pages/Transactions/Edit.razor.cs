using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Transactions;

public partial class Edit
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private UpdateTransactionDto? transaction;
    private List<FinancialAccountDto> accounts = new();
    private bool isLoading = true;
    private bool isSaving = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadData();
    }

    private async Task LoadData()
    {
        isLoading = true;

        accounts = await Http.GetFromJsonWithExceptionAsync<List<FinancialAccountDto>>(ApiConfig.GetApiUrl("/api/financialaccounts")) ?? new();

        var transactionDto = await Http.GetFromJsonWithExceptionAsync<TransactionDto>(ApiConfig.GetApiUrl($"/api/transactions/{Id}"));

        if (transactionDto != null)
        {
            transaction = new UpdateTransactionDto
            {
                Id = transactionDto.Id,
                FinancialAccountId = transactionDto.FinancialAccountId,
                Type = transactionDto.Type,
                Amount = transactionDto.Amount,
                Description = transactionDto.Description,
                TransactionDate = transactionDto.TransactionDate
            };
        }

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (transaction == null) return;

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

        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/transactions/{Id}"), transaction);

        NotificationService.ShowSuccess(PersianResources.SaveSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/transactions");

        isSaving = false;
    }

    private void GoBack()
    {
        Navigation.NavigateTo("/transactions");
    }
}
