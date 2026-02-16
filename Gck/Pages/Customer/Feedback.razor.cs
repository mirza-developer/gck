using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Components;
using Gck.Services;
using Gck.Common.Extensions;

namespace Gck.Pages.Customer;

public partial class Feedback : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private FeedbackModel feedbackModel = new();
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        // Check if user is a customer
        var userRole = await LocalStorage.GetItemAsync<string>("userRole");
        if (userRole != "customer")
        {
            Navigation.NavigateTo("/login");
        }
    }

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        var customerIdStr = await LocalStorage.GetItemAsync<string>("currentUserId");
        if (string.IsNullOrEmpty(customerIdStr))
        {
            Navigation.NavigateTo("/login");
            return;
        }

        var feedbackDto = new
        {
            CustomerId = int.Parse(customerIdStr),
            Subject = feedbackModel.Subject,
            Message = feedbackModel.Message
        };

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/feedback"), feedbackDto);

        NotificationService.ShowSuccess(Resources.PersianResources.FeedbackSubmitted);
        feedbackModel = new(); // Reset form

        isSubmitting = false;
    }

    private void Cancel()
    {
        Navigation.NavigateTo("/");
    }

    public class FeedbackModel
    {
        [Required(ErrorMessage = "موضوع الزامی است")]
        [StringLength(100, ErrorMessage = "موضوع نباید بیشتر از 100 کاراکتر باشد")]
        public string Subject { get; set; } = string.Empty;

        [Required(ErrorMessage = "پیام الزامی است")]
        [StringLength(1000, ErrorMessage = "پیام نباید بیشتر از 1000 کاراکتر باشد")]
        public string Message { get; set; } = string.Empty;
    }
}
