using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Users;

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

    private List<GetAllUsersVm> users = new();
    private string searchText = string.Empty;
    private bool isLoading = true;

    private List<GetAllUsersVm> FilteredUsers =>
        users.Where(u =>
            string.IsNullOrEmpty(searchText) ||
            u.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase) ||
            u.Username.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadUsers();
    }

    private async Task LoadUsers()
    {
        isLoading = true;

        users = await Http.GetFromJsonWithExceptionAsync<List<GetAllUsersVm>>(ApiConfig.GetApiUrl("/api/user")) ?? new();

        isLoading = false;
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/users/add");
    }

    private void NavigateToEdit(string username)
    {
        Navigation.NavigateTo($"/users/edit/{username}");
    }

    private async Task DeleteUser(string userId)
    {
        await Http.DeleteWithExceptionAsync(ApiConfig.GetApiUrl($"/api/user/{userId}"));
        NotificationService.ShowSuccess("کاربر با موفقیت حذف شد");
        await LoadUsers();
    }
}
