using Microsoft.AspNetCore.Components;
using Gck.Application.Features.Users.Commands.AddUser;
using Gck.Common.Extensions;
using Gck.Services;

namespace Gck.Pages.Users;

public partial class Add : ComponentBase
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private AddUserCommand command = new();
    private bool isSubmitting = false;

    private async Task HandleSubmit()
    {
        isSubmitting = true;

        await Http.PostAsJsonWithExceptionAsync(ApiConfig.GetApiUrl("/api/user"), command);

        NotificationService.ShowSuccess("کاربر با موفقیت ایجاد شد");
        await Task.Delay(1500);
        Navigation.NavigateTo("/users");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/users");
    }
}
