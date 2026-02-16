using Gck.Application.DTOs;
using Gck.Application.Features.Tables.Commands.UpdateTable;
using Gck.Common.Extensions;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Tables;

public partial class Edit : ComponentBase
{
    [Parameter]
    public int Id { get; set; }

    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private UpdateTableCommand? command;
    private bool isLoading = true;
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadTable();
    }

    private async Task LoadTable()
    {
        isLoading = true;

        var table = await Http.GetFromJsonWithExceptionAsync<TableDto>(ApiConfig.GetApiUrl($"/api/tables/{Id}"));
        if (table != null)
        {
            command = new UpdateTableCommand
            {
                Id = table.Id,
                Name = table.Name,
                NumberOfControllers = table.NumberOfControllers,
                HourlyFeePerController = table.HourlyFeePerController
            };
        }

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (command == null) return;

        isSubmitting = true;

        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/tables/{command.Id}"), command);

        NotificationService.ShowSuccess(PersianResources.SaveSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/tables");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/tables");
    }
}
