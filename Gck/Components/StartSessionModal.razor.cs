using System.Net.Http.Json;
using Gck.Application.DTOs;
using Gck.Models;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class StartSessionModal
{
    private bool isVisible;
    private bool loading = true;
    private List<HourlyFeeDto> hourlyFees = new();
    private List<CustomerDto> customers = new();
    private decimal? selectedFee;

    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    [Parameter] public int TableId { get; set; }
    [Parameter] public string TableName { get; set; } = string.Empty;
    [Parameter] public EventCallback OnSessionStarted { get; set; }

    private StartSessionModel model = new();

    public async Task Show()
    {
        model = new StartSessionModel { SeatsCount = 1, AnonymousCustomersCount = 0 };
        
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
        loading = true;

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
    
        if (exactFee is not null)
        {
            selectedFee = exactFee.Fee;
        }
        else
        {
            var nextHigherFee = hourlyFees
                .Where(f => f.SeatsCount > model.SeatsCount)
                .OrderBy(f => f.SeatsCount)
                .FirstOrDefault();

            if (nextHigherFee is not null)
            {
                model.SeatsCount = nextHigherFee.SeatsCount;
             
                selectedFee = nextHigherFee.Fee;
            }
            else
            {
                var highestFee = hourlyFees.OrderByDescending(f => f.SeatsCount).FirstOrDefault();
              
                if (highestFee is not null)
                {
                    model.SeatsCount = highestFee.SeatsCount;
                   
                    selectedFee = highestFee.Fee;
                }
            }
        }
    }

    private async Task HandleSubmit()
    {
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
            NotificationService.ShowSuccess(PersianResources.SessionStartedSuccess);

            await Task.Delay(1000);

            await OnSessionStarted.InvokeAsync();

            Close();
        }
        else
        { 
            NotificationService.ShowError(PersianResources.FailureMessage); 
        }
    }
}
