using Microsoft.AspNetCore.Components;
using Gck.Services;

namespace Gck.Pages.Customer;

public partial class Referral
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    private bool isLoading = true;
    private bool isSubmitting = false;
    private int currentCustomerId;
    private decimal creditBalance = 0;
    private string friendName = string.Empty;
    private string friendPhone = string.Empty;
    private int friendBirthYear = 1380;
    private string friendGender = "Male";
    private string successMessage = string.Empty;
    private string errorMessage = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        var customerIdStr = await LocalStorage.GetItemAsync<string>("currentUserId");
        var userRole = await LocalStorage.GetItemAsync<string>("userRole");

        if (string.IsNullOrEmpty(customerIdStr) || userRole != "customer")
        {
            Navigation.NavigateTo("/login");
            return;
        }

        if (int.TryParse(customerIdStr, out var id))
        {
            currentCustomerId = id;
        }

        // Load credit balance
        try
        {
            var response = await Http.GetAsync(ApiConfig.GetApiUrl($"/api/customers/{currentCustomerId}"));
            if (response.IsSuccessStatusCode)
            {
                var customer = await response.Content.ReadFromJsonAsync<CustomerInfoDto>();
                creditBalance = customer?.ReferralCredit ?? 0;
            }
        }
        catch { }

        isLoading = false;
    }

    private async Task SubmitIntroduction()
    {
        successMessage = string.Empty;
        errorMessage = string.Empty;

        if (string.IsNullOrWhiteSpace(friendName) || string.IsNullOrWhiteSpace(friendPhone))
        {
            errorMessage = "لطفاً نام و شماره تلفن دوست خود را وارد کنید";
            return;
        }

        isSubmitting = true;

        try
        {
            var command = new
            {
                ReferrerCustomerId = currentCustomerId,
                Name = friendName,
                PhoneNumber = friendPhone,
                BirthYear = friendBirthYear,
                Gender = friendGender
            };

            var response = await Http.PostAsJsonAsync(
                ApiConfig.GetApiUrl("/api/customers/introduce"),
                command);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<IntroduceResult>();
                if (result?.Success == true)
                {
                    successMessage = Resources.PersianResources.ReferralSubmitted;
                    friendName = string.Empty;
                    friendPhone = string.Empty;
                    friendBirthYear = 1380;
                    friendGender = "Male";
                }
                else
                {
                    errorMessage = result?.Message ?? Resources.PersianResources.FailureMessage;
                }
            }
            else
            {
                errorMessage = Resources.PersianResources.FailureMessage;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{Resources.PersianResources.LoadingError}: {ex.Message}";
        }
        finally
        {
            isSubmitting = false;
        }
    }

    private class CustomerInfoDto
    {
        public int Id { get; set; }
        public decimal ReferralCredit { get; set; }
    }

    private class IntroduceResult
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public int? NewCustomerId { get; set; }
    }
}
