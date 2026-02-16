using Microsoft.AspNetCore.Components;
using Gck.Application.DTOs;
using Gck.Application.Features.Users.Commands.UpdateUser;
using Gck.Application.Features.Users.Queries.GetUserByUsername;
using Gck.Common.Extensions;
using Gck.Services;

namespace Gck.Pages.Users;

public partial class Edit : ComponentBase
{
    [Parameter]
    public string Username { get; set; } = string.Empty;

    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private UpdateUserCommand? command;
    private bool isLoading = true;
    private bool isSubmitting = false;

    protected override async Task OnInitializedAsync()
    {
        await LoadUser();
    }

    private async Task LoadUser()
    {
        isLoading = true;

        var user = await Http.GetFromJsonWithExceptionAsync<GetUserByIdVm>(ApiConfig.GetApiUrl($"/api/user/username/{Username}"));
        if (user != null)
        {
            command = new UpdateUserCommand
            {
                Id = user.Id,
                Username = user.Username,
                Name = user.Name,
                Email = user.Email,
                PhoneNumber = user.PhoneNumber,
                IsActive = user.IsActive,
                Details = user.Details
            };
        }

        isLoading = false;
    }

    private async Task HandleSubmit()
    {
        if (command == null) return;

        isSubmitting = true;

        await Http.PutAsJsonWithExceptionAsync(ApiConfig.GetApiUrl($"/api/user/{command.Id}"), command);

        NotificationService.ShowSuccess("تغییرات با موفقیت ذخیره شد");
        await Task.Delay(1500);
        Navigation.NavigateTo("/users");

        isSubmitting = false;
    }

    private void NavigateBack()
    {
        Navigation.NavigateTo("/users");
    }
}
