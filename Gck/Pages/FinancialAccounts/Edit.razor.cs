using Gck.Application.DTOs;
using Gck.Application.Features.FinancialAccounts.Commands.UpdateFinancialAccount;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.FinancialAccounts;

public partial class Edit : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private UpdateFinancialAccountCommand? command;
    private bool isLoading = true;
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadAccount();
    }

    private async Task LoadAccount()
    {
        isLoading = true;

        var account = await Http.GetFromJsonWithExceptionAsync<FinancialAccountDto>(ApiConfig.GetApiUrl($"/api/financialaccounts/{Id}"));
        if (account != null)
        {
            command = new UpdateFinancialAccountCommand
            {
                Id = account.Id,
                AccountName = account.AccountName,
                CardNumber = account.CardNumber,
                BankName = account.BankName
            };
        }

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (command == null) return;

        isSubmitting = true;

        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/financialaccounts/{command.Id}"), command);

        NotificationService.ShowSuccess(PersianResources.SaveSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/financialaccounts");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/financialaccounts");
    }
}
