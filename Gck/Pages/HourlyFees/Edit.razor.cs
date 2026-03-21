using Gck.Application.DTOs;
using Gck.Application.Features.HourlyFees.Commands.UpdateHourlyFee;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.HourlyFees;

public partial class Edit
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

    private UpdateHourlyFeeCommand? command;
    private bool isLoading = true;
    private bool isSaving = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadHourlyFee();
    }

    private async Task LoadHourlyFee()
    {
        isLoading = true;

        var fee = await Http.GetFromJsonWithExceptionAsync<HourlyFeeDto>(ApiConfig.GetApiUrl($"/api/hourlyfees/{Id}"));
        if (fee != null)
        {
            command = new UpdateHourlyFeeCommand
            {
                Id = fee.Id,
                SeatsCount = fee.SeatsCount,
                Fee = fee.Fee
            };
        }

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (command == null) return;

        isSaving = true;

        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/hourlyfees/{Id}"), command);
        NotificationService.ShowSuccess(Resources.PersianResources.UpdateSuccess);
        Navigation.NavigateTo("/hourlyfees");

        isSaving = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/hourlyfees");
    }
}
