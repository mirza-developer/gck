using Gck.Application.DTOs;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class FinishSessionModal
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    [Inject] private INotificationService NotificationService { get; set; } = default!;

    [Parameter] public int SessionId { get; set; }
    [Parameter] public EventCallback OnSessionFinished { get; set; }

    private bool isVisible;
    private bool loading = true;
    private bool showConfirm;

    private SessionDetailsDto? sessionDetails;
    private List<FinancialAccountDto> financialAccounts = new();
    private List<CustomerLoyaltyInfo> customersLoyalty = new();
    private decimal recommendedPrice;
    private decimal originalRecommendedPrice;
    private decimal finalPrice;
    private int selectedFinancialAccountId;
    private bool hasFreeSessionDiscount = false;
    private int freeSessionCustomersCount = 0;
    private int totalPeopleInSession = 0;
    private decimal totalDiscountAmount = 0;

    public async Task Show(int sessionId)
    {
        SessionId = sessionId;
        showConfirm = false;
        selectedFinancialAccountId = 0;
        isVisible = true;
        loading = true;

        await LoadSessionDetails();
    }

    public void Close()
    {
        isVisible = false;
    }

    private async Task LoadSessionDetails()
    {
        loading = true;

        var response = await Http.GetAsync(ApiConfig.GetApiUrl($"/api/sessions/{SessionId}"));

        if (response.IsSuccessStatusCode)
        {
            sessionDetails = await response.Content.ReadFromJsonAsync<SessionDetailsDto>();

            var duration = (DateTime.Now - sessionDetails.StartDateTime).TotalHours;
            
            originalRecommendedPrice = Math.Ceiling((decimal)duration * sessionDetails.FeePerHour / 1000) * 1000;
            
            recommendedPrice = originalRecommendedPrice;

            customersLoyalty.Clear();
            
            hasFreeSessionDiscount = false;
            
            freeSessionCustomersCount = 0;
         
            totalDiscountAmount = 0;

            totalPeopleInSession = sessionDetails.Customers.Count + sessionDetails.AnonymousCustomersCount;
            
            decimal pricePerPerson = totalPeopleInSession > 0 ? originalRecommendedPrice / totalPeopleInSession : originalRecommendedPrice;

            foreach (var customer in sessionDetails.Customers)
            {
                var loyaltyInfo = new CustomerLoyaltyInfo
                {
                    CustomerId = customer.Id,
                    IsLoyal = customer.IsLoyal,
                    SessionsRequiredForFree = customer.SessionsRequiredForFree,
                    PaidSessionsCount = customer.PaidSessionsCount,
                    CanGetFreeSession = customer.IsLoyal && customer.SessionsRequiredForFree > 0 &&
                                      customer.PaidSessionsCount >= customer.SessionsRequiredForFree,
                    RemainingSessionsForFree = customer.IsLoyal && customer.SessionsRequiredForFree > 0
                        ? Math.Max(0, customer.SessionsRequiredForFree - customer.PaidSessionsCount)
                        : 0
                };

                if (loyaltyInfo.IsLoyal && loyaltyInfo.SessionsRequiredForFree > 0)
                {
                    loyaltyInfo.ProgressPercentage = Math.Min(100,
                        (double)customer.PaidSessionsCount / customer.SessionsRequiredForFree * 100);
                }

                customersLoyalty.Add(loyaltyInfo);

                if (loyaltyInfo.CanGetFreeSession)
                {
                    hasFreeSessionDiscount = true;
                    
                    freeSessionCustomersCount++;
                
                    totalDiscountAmount += pricePerPerson;
                }
            }

            if (hasFreeSessionDiscount)
            {
                recommendedPrice = Math.Max(0, originalRecommendedPrice - totalDiscountAmount);
                
                recommendedPrice = Math.Ceiling(recommendedPrice / 1000) * 1000;
               
                totalDiscountAmount = originalRecommendedPrice - recommendedPrice;
            }

            finalPrice = recommendedPrice;
        }
    }

    private async Task ProceedToConfirm()
    {
        loading = true;

        var response = await Http.GetAsync(ApiConfig.GetApiUrl("/api/financialaccounts"));
       
        if (response.IsSuccessStatusCode)
        {
            financialAccounts = await response.Content.ReadFromJsonAsync<List<FinancialAccountDto>>() ?? new();
           
            showConfirm = true;
        }

        loading = false;
    }

    private void BackToPrice()
    {
        showConfirm = false;
    }

    private async Task ResumeSession()
    {
        var response = await Http.PostAsync(ApiConfig.GetApiUrl($"/api/sessions/{SessionId}/resume"), null);

        if (response.IsSuccessStatusCode)
        {
            await OnSessionFinished.InvokeAsync();

            Close();
        }
    }

    private async Task FinishSession()
    {
        var command = new
        {
            sessionId = SessionId,
            finalPrice = finalPrice,
            financialAccountId = selectedFinancialAccountId
        };

        var response = await Http.PostAsJsonAsync(ApiConfig.GetApiUrl($"/api/sessions/{SessionId}/finish"), command);

        if (response.IsSuccessStatusCode)
        {
            NotificationService.ShowSuccess(PersianResources.SessionFinishedSuccess);
            
            await Task.Delay(1500);
            
            await OnSessionFinished.InvokeAsync();
            
            Close();
        }
    }

    private string GetDuration()
    {
        if (sessionDetails == null) return "--";

        var duration = DateTime.Now - sessionDetails.StartDateTime;
       
        var hours = (int)duration.TotalHours;
        
        var minutes = duration.Minutes;

        if (hours > 0)
        {
            return $"{hours} {PersianResources.Hours} {PersianResources.And} {minutes} {PersianResources.Minutes}"; 
        }
        else
        {
            return $"{minutes} {PersianResources.Minutes}";
        }
    }
}
