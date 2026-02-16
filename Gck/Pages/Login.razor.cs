using Microsoft.AspNetCore.Components;
using Gck.Application.Features.Auth.Commands.Login;
using Gck.Application.Features.Auth.Commands.CustomerLogin;
using Gck.Resources;
using Gck.Services;
using Gck.Common.Extensions;

namespace Gck.Pages
{
    public partial class Login : IDisposable
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

        private bool isAdminTab = true;
        private bool otpSent = false;

        // Admin login
        private LoginCommand loginCommand = new();
        private bool isLoggingIn = false;

        // Customer OTP login
        private SendOtpCommand sendOtpCommand = new();
        private VerifyOtpCommand verifyOtpCommand = new();
        private bool isSendingOtp = false;
        private bool isVerifyingOtp = false;

        private string errorMessage = string.Empty;
        private string successMessage = string.Empty;

        // OTP resend timer
        private int resendCountdown = 0;
        private bool canResendOtp = false;
        private System.Threading.Timer? resendTimer;

        protected override async Task OnInitializedAsync()
        {
            // Check if user is already logged in
            var currentUser = await LocalStorage.GetItemAsync<string>("currentUser");
            if (!string.IsNullOrEmpty(currentUser))
            {
                Navigation.NavigateTo("/");
            }
        }

        private void SwitchTab(bool isAdmin)
        {
            isAdminTab = isAdmin;
            errorMessage = string.Empty;
            successMessage = string.Empty;
            otpSent = false;

            // Reset forms
            loginCommand = new();
            sendOtpCommand = new();
            verifyOtpCommand = new();

            // Stop timer if running
            StopResendTimer();
        }

        private void ResetOtpForm()
        {
            errorMessage = string.Empty;
            successMessage = string.Empty;
            sendOtpCommand = new() { PhoneNumber = verifyOtpCommand.PhoneNumber };
            verifyOtpCommand = new();

            // Reset to send OTP form
            otpSent = false;

            // Stop timer (will restart when new OTP is sent)
            StopResendTimer();
        }

        private void StartResendTimer()
        {
            resendCountdown = 60;
            canResendOtp = false;

            resendTimer?.Dispose();
            resendTimer = new System.Threading.Timer(async _ =>
            {
                if (resendCountdown > 0)
                {
                    resendCountdown--;
                    await InvokeAsync(StateHasChanged);
                }
                else
                {
                    canResendOtp = true;
                    resendTimer?.Dispose();
                    resendTimer = null;
                    await InvokeAsync(StateHasChanged);
                }
            }, null, TimeSpan.FromSeconds(1), TimeSpan.FromSeconds(1));
        }

        private void StopResendTimer()
        {
            resendTimer?.Dispose();
            resendTimer = null;
            resendCountdown = 0;
            canResendOtp = false;
        }

        private async Task HandleAdminLogin()
        {
            isLoggingIn = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;

            var result = await Http.PostAsJsonWithExceptionAsync<LoginCommand, LoginResponse>(ApiConfig.GetApiUrl("/api/auth/login"), loginCommand);

            if (result != null && result.Success)
            {
                // Store user info in local storage
                await LocalStorage.SetItemAsync("currentUser", result.Username);
                await LocalStorage.SetItemAsync("currentUserId", result.UserId);
                await LocalStorage.SetItemAsync("currentUserName", result.Name);
                await LocalStorage.SetItemAsync("userRole", "admin");

                NotificationService.ShowSuccess(result.Message);
                await Task.Delay(1000);

                // Redirect to home page
                Navigation.NavigateTo("/", true);
            }
            else
            {
                NotificationService.ShowError(result?.Message ?? PersianResources.LoginError);
            }

            isLoggingIn = false;
        }

        private async Task HandleSendOtp()
        {
            isSendingOtp = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;

            var result = await Http.PostAsJsonWithExceptionAsync<SendOtpCommand, SendOtpResponse>(ApiConfig.GetApiUrl("/api/auth/customer/send-otp"), sendOtpCommand);

            if (result != null && result.Success)
            {
                NotificationService.ShowSuccess(result.Message);
                otpSent = true;
                verifyOtpCommand.PhoneNumber = sendOtpCommand.PhoneNumber;

                // Start the resend countdown timer
                StartResendTimer();
            }
            else
            {
                NotificationService.ShowError(result?.Message ?? PersianResources.OtpSendError);
            }

            isSendingOtp = false;
        }

        private async Task HandleVerifyOtp()
        {
            isVerifyingOtp = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;

            var result = await Http.PostAsJsonWithExceptionAsync<VerifyOtpCommand, VerifyOtpResponse>(ApiConfig.GetApiUrl("/api/auth/customer/verify-otp"), verifyOtpCommand);

            if (result != null && result.Success)
            {
                // Store customer info in local storage
                await LocalStorage.SetItemAsync("currentUser", result.PhoneNumber);
                await LocalStorage.SetItemAsync("currentUserId", result.CustomerId.ToString());
                await LocalStorage.SetItemAsync("currentUserName", result.CustomerName);
                await LocalStorage.SetItemAsync("userRole", "customer");

                NotificationService.ShowSuccess(result.Message);

                // Stop timer on successful login
                StopResendTimer();

                await Task.Delay(1000);

                // Redirect to home page
                Navigation.NavigateTo("/", true);
            }
            else
            {
                NotificationService.ShowError(result?.Message ?? PersianResources.InvalidOtp);
            }

            isVerifyingOtp = false;
        }

        private async Task HandleResendOtp()
        {
            isSendingOtp = true;
            errorMessage = string.Empty;
            successMessage = string.Empty;

            // Use the phone number from the verify command
            var resendCommand = new SendOtpCommand { PhoneNumber = verifyOtpCommand.PhoneNumber };
            var result = await Http.PostAsJsonWithExceptionAsync<SendOtpCommand, SendOtpResponse>(ApiConfig.GetApiUrl("/api/auth/customer/send-otp"), resendCommand);

            if (result != null && result.Success)
            {
                NotificationService.ShowSuccess(result.Message);

                // Clear the OTP field for the new code
                verifyOtpCommand.OtpCode = string.Empty;

                // Restart the countdown timer
                StartResendTimer();
            }
            else
            {
                NotificationService.ShowError(result?.Message ?? PersianResources.OtpSendError);
            }

            isSendingOtp = false;
        }

        public void Dispose()
        {
            StopResendTimer();
        }
    }
}
