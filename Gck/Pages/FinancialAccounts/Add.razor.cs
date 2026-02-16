using Gck.Application.Features.FinancialAccounts.Commands.CreateFinancialAccount;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.FinancialAccounts;

public partial class Add
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private CreateFinancialAccountCommand command = new();
    private bool isSubmitting = false;

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/financialaccounts"), command);

        NotificationService.ShowSuccess(PersianResources.FinancialAccountAddSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/financialaccounts");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/financialaccounts");
    }
}
