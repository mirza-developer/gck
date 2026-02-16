using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Customers;

public partial class Index : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private List<CustomerDto> customers = new();
    private string searchText = string.Empty;
    private bool isLoading = true;
    private int? expandedRowId = null;

    private List<CustomerDto> FilteredCustomers =>
        customers.Where(c =>
            string.IsNullOrEmpty(searchText) ||
            c.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            c.PhoneNumber.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadCustomers();
    }

    private async Task LoadCustomers()
    {
        isLoading = true;

        customers = await Http.GetFromJsonWithExceptionAsync<List<CustomerDto>>(ApiConfig.GetApiUrl("/api/customers")) ?? new();

        isLoading = false;
    }

    private void ToggleRow(int customerId)
    {
        expandedRowId = expandedRowId == customerId ? null : customerId;
    }

    private int CalculateAge(int birthYear)
    {
        var currentPersianYear = 1403;
        return currentPersianYear - birthYear;
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/customers/add");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/customers/edit/{id}");
    }

    private async Task DeleteCustomer(int id)
    {
        await Http.DeleteWithExceptionAsync(ApiConfig.GetApiUrl($"/api/customers/{id}"));
        NotificationService.ShowSuccess(Resources.PersianResources.DeleteSuccess);
        await LoadCustomers();
    }
}
