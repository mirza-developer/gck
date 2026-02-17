using Blazored.LocalStorage;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Feedbacks;

public partial class Index
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private ILocalStorageService LocalStorage { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private List<FeedbackDto> feedbacks = new();
    private bool isLoading = true;

    protected override async Task OnInitializedAsync()
    {
        var userRole = await LocalStorage.GetItemAsync<string>("userRole");
        if (userRole != "admin")
        {
            Navigation.NavigateTo("/");
            return;
        }

        await LoadFeedbacks();
    }

    private async Task LoadFeedbacks()
    {
        isLoading = true;

        feedbacks = await Http.GetFromJsonWithExceptionAsync<List<FeedbackDto>>(ApiConfig.GetApiUrl("/api/feedback")) ?? new();

        isLoading = false;
    }

    private async Task MarkAsRead(int feedbackId)
    {
        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/feedback/{feedbackId}/mark-read"), (object?)null);
        var feedback = feedbacks.FirstOrDefault(f => f.Id == feedbackId);
        if (feedback != null)
        {
            feedback.IsRead = true;
            NotificationService.ShowSuccess("بازخورد به عنوان خوانده شده علامت‌گذاری شد");
        }
    }

    public class FeedbackDto
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string Subject { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime SubmittedAt { get; set; }
        public bool IsRead { get; set; }
    }
}
