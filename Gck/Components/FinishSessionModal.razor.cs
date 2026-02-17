using System.Net.Http.Json;
using Gck.Resources;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Components;

public partial class FinishSessionModal
{
    [Inject] private HttpClient Http { get; set; } = default!;
    [Inject] private ApiConfigurationService ApiConfig { get; set; } = default!;
    
    [Parameter] public int SessionId { get; set; }
    [Parameter] public EventCallback OnSessionFinished { get; set; }

    private bool isVisible;
    private bool loading = true;
    private bool submitting;
    private bool showConfirm;
    private string? errorMessage;
    private string? confirmErrorMessage;
    private string? successMessage;

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
        errorMessage = null;
        confirmErrorMessage = null;
        successMessage = null;
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
        try
        {
            loading = true;
            errorMessage = null;

            var response = await Http.GetAsync(ApiConfig.GetApiUrl($"/api/sessions/{SessionId}"));
            if (response.IsSuccessStatusCode)
            {
                sessionDetails = await response.Content.ReadFromJsonAsync<SessionDetailsDto>();
                
                if (sessionDetails != null)
                {
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
            else
            {
                errorMessage = PersianResources.ErrorLoadingSessionDetails;
            }

            loading = false;
        }
        catch (Exception ex)
        {
            errorMessage = $"{PersianResources.LoadingError}: {ex.Message}";
            loading = false;
        }
    }

    private async Task ProceedToConfirm()
    {
        try
        {
            loading = true;
            confirmErrorMessage = null;

            var response = await Http.GetAsync(ApiConfig.GetApiUrl("/api/financialaccounts"));
            if (response.IsSuccessStatusCode)
            {
                financialAccounts = await response.Content.ReadFromJsonAsync<List<FinancialAccountDto>>() ?? new();
                showConfirm = true;
            }
            else
            {
                confirmErrorMessage = PersianResources.ErrorLoadingFinancialAccounts;
            }

            loading = false;
        }
        catch (Exception ex)
        {
            confirmErrorMessage = $"{PersianResources.Error}: {ex.Message}";
            loading = false;
        }
    }

    private void BackToPrice()
    {
        showConfirm = false;
        confirmErrorMessage = null;
    }

    private async Task ResumeSession()
    {
        try
        {
            submitting = true;
            errorMessage = null;

            var response = await Http.PostAsync(ApiConfig.GetApiUrl($"/api/sessions/{SessionId}/resume"), null);
            
            if (response.IsSuccessStatusCode)
            {
                await OnSessionFinished.InvokeAsync();
                Close();
            }
            else
            {
                errorMessage = PersianResources.ErrorResumingSession;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"{PersianResources.Error}: {ex.Message}";
        }
        finally
        {
            submitting = false;
        }
    }

    private async Task FinishSession()
    {
        try
        {
            submitting = true;
            confirmErrorMessage = null;

            var command = new
            {
                sessionId = SessionId,
                finalPrice = finalPrice,
                financialAccountId = selectedFinancialAccountId
            };

            var response = await Http.PostAsJsonAsync(ApiConfig.GetApiUrl($"/api/sessions/{SessionId}/finish"), command);

            if (response.IsSuccessStatusCode)
            {
                successMessage = PersianResources.SessionFinishedSuccess;
                await Task.Delay(1500);
                await OnSessionFinished.InvokeAsync();
                Close();
            }
            else
            {
                var error = await response.Content.ReadAsStringAsync();
                confirmErrorMessage = $"{PersianResources.ErrorFinishingSession}: {error}";
            }
        }
        catch (Exception ex)
        {
            confirmErrorMessage = $"{PersianResources.Error}: {ex.Message}";
        }
        finally
        {
            submitting = false;
        }
    }

    private string GetDuration()
    {
        if (sessionDetails == null) return "--";
        
        var duration = DateTime.Now - sessionDetails.StartDateTime;
        var hours = (int)duration.TotalHours;
        var minutes = duration.Minutes;
        
        if (hours > 0)
            return $"{hours} {PersianResources.Hours} {PersianResources.And} {minutes} {PersianResources.Minutes}";
        else
            return $"{minutes} {PersianResources.Minutes}";
    }

    private class SessionDetailsDto
    {
        public int Id { get; set; }
        public string TableName { get; set; } = string.Empty;
        public DateTime StartDateTime { get; set; }
        public decimal FeePerHour { get; set; }
        public int AnonymousCustomersCount { get; set; } = 0;
        public List<CustomerDto> Customers { get; set; } = new();
    }

    private class CustomerDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsLoyal { get; set; }
        public int SessionsRequiredForFree { get; set; }
        public int PaidSessionsCount { get; set; }
    }

    private class FinancialAccountDto
    {
        public int Id { get; set; }
        public string AccountName { get; set; } = string.Empty;
        public string BankName { get; set; } = string.Empty;
        public string CardNumber { get; set; } = string.Empty;
    }

    private class CustomerLoyaltyInfo
    {
        public int CustomerId { get; set; }
        public bool IsLoyal { get; set; }
        public int SessionsRequiredForFree { get; set; }
        public int PaidSessionsCount { get; set; }
        public bool CanGetFreeSession { get; set; }
        public int RemainingSessionsForFree { get; set; }
        public double ProgressPercentage { get; set; }
    }
}
