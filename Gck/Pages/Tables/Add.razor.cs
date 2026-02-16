using Gck.Application.Features.Tables.Commands.CreateTable;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Tables;

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

    private CreateTableCommand command = new();
    private bool isSubmitting = false;

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/tables"), command);

        NotificationService.ShowSuccess(PersianResources.TableAddSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/tables");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/tables");
    }
}
