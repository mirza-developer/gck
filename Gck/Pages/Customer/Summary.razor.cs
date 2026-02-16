using System.Net.Http.Json;
using Microsoft.AspNetCore.Components;
using Gck.Services;

namespace Gck.Pages.Customer;

public partial class Summary
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;

    private CustomerSummaryDto? customer;
    private List<CustomerSessionDto> sessions = new();
    private bool isLoading = true;
    private string errorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadCustomerSummary();
    }

    private async Task LoadCustomerSummary()
    {
        isLoading = true;
        errorMessage = string.Empty;

        try
        {
            // Get customer ID from local storage
            var customerIdStr = await LocalStorage.GetItemAsync<string>("currentUserId");
            var userRole = await LocalStorage.GetItemAsync<string>("userRole");

            if (string.IsNullOrEmpty(customerIdStr) || userRole != "customer")
            {
                Navigation.NavigateTo("/login");
                return;
            }

            // Load customer details
            var customerResponse = await Http.GetAsync(ApiConfig.GetApiUrl($"/api/customers/{customerIdStr}"));
            if (customerResponse.IsSuccessStatusCode)
            {
                customer = await customerResponse.Content.ReadFromJsonAsync<CustomerSummaryDto>();
            }

            // Load customer sessions
            var sessionsResponse = await Http.GetAsync(ApiConfig.GetApiUrl($"/api/customers/{customerIdStr}/sessions"));
            if (sessionsResponse.IsSuccessStatusCode)
            {
                sessions = await sessionsResponse.Content.ReadFromJsonAsync<List<CustomerSessionDto>>() ?? new();
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{Resources.PersianResources.LoadingError}: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private string GetDuration(CustomerSessionDto session)
    {
        if (!session.EndDateTime.HasValue)
            return "-";

        var duration = session.EndDateTime.Value - session.StartDateTime;
        var hours = (int)duration.TotalHours;
        var minutes = duration.Minutes;

        if (hours > 0 && minutes > 0)
            return $"{hours} {Resources.PersianResources.Hours} {Resources.PersianResources.And} {minutes} {Resources.PersianResources.Minutes}";
        else if (hours > 0)
            return $"{hours} {Resources.PersianResources.Hours}";
        else
            return $"{minutes} {Resources.PersianResources.Minutes}";
    }

    public class CustomerSummaryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public int BirthYear { get; set; }
        public bool IsMale { get; set; }
        public bool IsLoyal { get; set; }
        public int SessionsRequiredForFree { get; set; }
        public int PaidSessionsCount { get; set; }
    }

    public class CustomerSessionDto
    {
        public int Id { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string TableName { get; set; } = string.Empty;
        public bool IsFreeSession { get; set; }
        public decimal? FinalPrice { get; set; }
    }
}
