using Gck.Application.Features.Customers.Commands.CreateCustomer;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Customers;

public partial class Add : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private CreateCustomerCommand command = new() { Gender = "Male" };
    private bool isSubmitting = false;

    private void OnLoyaltyToggle(ChangeEventArgs e)
    {
        if (!command.IsLoyal)
        {
            command.SessionsRequiredForFree = 0;
        }
    }

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/customers"), command);

        NotificationService.ShowSuccess(Resources.PersianResources.CustomerAddSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/customers");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/customers");
    }
}
