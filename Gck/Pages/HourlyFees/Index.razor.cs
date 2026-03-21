using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.HourlyFees;

public partial class Index
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private List<HourlyFeeDto> hourlyFees = new();
    private string searchText = string.Empty;
    private bool isLoading = true;

    private List<HourlyFeeDto> FilteredHourlyFees =>
        hourlyFees.Where(f =>
            string.IsNullOrEmpty(searchText) ||
            f.SeatsCount.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            f.Fee.ToString().Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadHourlyFees();
    }

    private async Task LoadHourlyFees()
    {
        isLoading = true;

        hourlyFees = await Http.GetFromJsonWithExceptionAsync<List<HourlyFeeDto>>(ApiConfig.GetApiUrl("/api/hourlyfees")) ?? new();

        isLoading = false;
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/hourlyfees/add");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/hourlyfees/edit/{id}");
    }

    private async Task DeleteHourlyFee(int id)
    {
        await Http.DeleteWithExceptionAsync(ApiConfig.GetApiUrl($"/api/hourlyfees/{id}"));
        NotificationService.ShowSuccess(Resources.PersianResources.DeleteSuccess);
        await LoadHourlyFees();
    }
}
