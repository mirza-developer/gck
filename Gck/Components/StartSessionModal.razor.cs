using System.Net.Http.Json;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class StartSessionModal : ComponentBase
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    
    [Parameter] public int TableId { get; set; }
    [Parameter] public string TableName { get; set; } = string.Empty;
    [Parameter] public EventCallback OnSessionStarted { get; set; }

    private bool isVisible;
    private bool loading = true;
    private bool submitting;
    private string? errorMessage;
    private string? successMessage;

    private List<HourlyFeeDto> hourlyFees = new();
    private List<CustomerDto> customers = new();
    private decimal? selectedFee;

    private StartSessionModel model = new();

    private class StartSessionModel
    {
        public int SeatsCount { get; set; } = 1;
        public int AnonymousCustomersCount { get; set; } = 0;
        public List<int> CustomerIds { get; set; } = new();
    }

    public async Task Show()
    {
        model = new StartSessionModel { SeatsCount = 1, AnonymousCustomersCount = 0 };
        errorMessage = null;
        successMessage = null;
        isVisible = true;
        loading = true;

        await LoadData();
    }

    public void Close()
    {
        isVisible = false;
    }

    private async Task LoadData()
    {
        try
        {
            loading = true;
            errorMessage = null;

            var feesResponse = await Http.GetAsync(ApiConfig.GetApiUrl("/api/hourlyfees"));
            if (feesResponse.IsSuccessStatusCode)
            {
                hourlyFees = await feesResponse.Content.ReadFromJsonAsync<List<HourlyFeeDto>>() ?? new();
                hourlyFees = hourlyFees.OrderBy(f => f.SeatsCount).ToList();
                
                if (hourlyFees.Any())
                {
                    model.SeatsCount = hourlyFees.First().SeatsCount;
                    selectedFee = hourlyFees.First().Fee;
                }
            }

            var customersResponse = await Http.GetAsync(ApiConfig.GetApiUrl("/api/customers"));
            if (customersResponse.IsSuccessStatusCode)
            {
                customers = await customersResponse.Content.ReadFromJsonAsync<List<CustomerDto>>() ?? new();
            }

            loading = false;
        }
        catch (Exception ex)
        {
            errorMessage = $"{PersianResources.LoadingError}: {ex.Message}";
            loading = false;
        }
    }

    private void IncrementAnonymousCount()
    {
        model.AnonymousCustomersCount++;
        OnCustomersOrCountChanged();
    }

    private void DecrementAnonymousCount()
    {
        if (model.AnonymousCustomersCount > 0)
        {
            model.AnonymousCustomersCount--;
            OnCustomersOrCountChanged();
        }
    }

    private void OnCustomersOrCountChanged()
    {
        var registeredCustomersCount = model.CustomerIds.Count;
        var totalCustomersCount = registeredCustomersCount + model.AnonymousCustomersCount;
        
        if (totalCustomersCount == 0)
        {
            if (hourlyFees.Any())
            {
                model.SeatsCount = hourlyFees.First().SeatsCount;
                selectedFee = hourlyFees.First().Fee;
            }
            return;
        }
        
        model.SeatsCount = totalCustomersCount;
        
        var exactFee = hourlyFees.FirstOrDefault(f => f.SeatsCount == model.SeatsCount);
        if (exactFee != null)
        {
            selectedFee = exactFee.Fee;
        }
        else
        {
            var nextHigherFee = hourlyFees
                .Where(f => f.SeatsCount > model.SeatsCount)
                .OrderBy(f => f.SeatsCount)
                .FirstOrDefault();
            
            if (nextHigherFee != null)
            {
                model.SeatsCount = nextHigherFee.SeatsCount;
                selectedFee = nextHigherFee.Fee;
            }
            else
            {
                var highestFee = hourlyFees.OrderByDescending(f => f.SeatsCount).FirstOrDefault();
                if (highestFee != null)
                {
                    model.SeatsCount = highestFee.SeatsCount;
                    selectedFee = highestFee.Fee;
                }
            }
        }
    }

    private async Task HandleSubmit()
    {
        try
        {
            submitting = true;
            errorMessage = null;
            successMessage = null;

            var command = new
            {
                tableId = TableId,
                seatsCount = model.SeatsCount,
                anonymousCustomersCount = model.AnonymousCustomersCount,
                customerIds = model.CustomerIds.Any() ? model.CustomerIds : null
            };

            var response = await Http.PostAsJsonAsync(ApiConfig.GetApiUrl("/api/sessions/start"), command);

            if (response.IsSuccessStatusCode)
            {
                successMessage = PersianResources.SessionStartedSuccess;
                await Task.Delay(1000);
                await OnSessionStarted.InvokeAsync();
                Close();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                errorMessage = $"{PersianResources.ErrorStartingSession}: {error}";
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{PersianResources.ErrorStartingSession}: {ex.Message}";
        }
        finally
        {
            submitting = false;
        }
    }

    private class HourlyFeeDto
    {
        public int Id { get; set; }
        public int SeatsCount { get; set; }
        public decimal Fee { get; set; }
    }

    private class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
