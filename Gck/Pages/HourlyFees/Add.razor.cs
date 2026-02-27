using Gck.Application.Features.HourlyFees.Commands.CreateHourlyFee;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.HourlyFees;

public partial class Add
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private CreateHourlyFeeCommand command = new();
    private bool isLoading = false;

    private async Task HandleSubmit()
    {
        isLoading = true;

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/hourlyfees"), command);
        NotificationService.ShowSuccess(Resources.PersianResources.CreateSuccess);
        Navigation.NavigateTo("/hourlyfees");

        isLoading = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/hourlyfees");
    }
}
