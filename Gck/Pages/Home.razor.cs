using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages;

public partial class Home
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    private List<HourlyFeeDto> hourlyFees = new();
    private bool isLoadingFees = true;

    protected override async Task OnInitializedAsync()
    {
        await LoadHourlyFees();
    }

    private async Task LoadHourlyFees()
    {
        try
        {
            isLoadingFees = true;
            hourlyFees = await Http.GetFromJsonWithExceptionAsync<List<HourlyFeeDto>>(ApiConfig.GetApiUrl("/api/hourlyfees")) ?? new();
        }
        catch
        {
            // Silently fail and show empty table if API is not available
            hourlyFees = new();
        }
        finally
        {
            isLoadingFees = false;
        }
    }
}
