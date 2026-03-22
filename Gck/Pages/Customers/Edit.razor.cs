using Gck.Application.DTOs;
using Gck.Application.Features.Customers.Commands.UpdateCustomer;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Customers;

public partial class Edit
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    [Parameter]
    public int Id { get; set; }

    private UpdateCustomerCommand? command;
    private bool isLoading = true;
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadCustomer();
    }

    private async Task LoadCustomer()
    {
        isLoading = true;

        var customer = await Http.GetFromJsonWithExceptionAsync<CustomerDto>(ApiConfig.GetApiUrl($"/api/customers/{Id}"));
        if (customer != null)
        {
            command = new UpdateCustomerCommand
            {
                Id = customer.Id,
                Name = customer.Name,
                PhoneNumber = customer.PhoneNumber,
                BirthYear = customer.BirthYear,
                Gender = customer.Gender,
                IsLoyal = customer.IsLoyal,
                SessionsRequiredForFree = customer.SessionsRequiredForFree,
                IsVerifiedByAdmin = customer.IsVerifiedByAdmin,
                ReferralRewardPercentage = customer.ReferralRewardPercentage
            };
        }

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (command == null) return;

        isSubmitting = true;

        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/customers/{command.Id}"), command);

        NotificationService.ShowSuccess(Resources.PersianResources.SaveSuccess);
        await Task.Delay(1500);
        Navigation.NavigateTo("/customers");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/customers");
    }

    private void OnLoyaltyToggle(ChangeEventArgs e)
    {
        if (command != null && !command.IsLoyal)
        {
            command.SessionsRequiredForFree = 0;
        }
    }
}
