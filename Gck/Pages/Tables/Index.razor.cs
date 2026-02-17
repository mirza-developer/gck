using Gck.Application.DTOs;
using Gck.Common.Extensions;
using Gck.Services;
using Microsoft.AspNetCore.Components;

namespace Gck.Pages.Tables;

public partial class Index
{
    [Inject]
    private HttpClient Http { get; set; } = default!;

    [Inject]
    private NavigationManager Navigation { get; set; } = default!;

    [Inject]
    private ApiConfigurationService ApiConfig { get; set; } = default!;

    [Inject]
    private INotificationService NotificationService { get; set; } = default!;

    private List<TableDto> tables = new();
    private string searchText = string.Empty;
    private bool isLoading = true;
    private bool showDeleteModal = false;
    private int tableToDeleteId = 0;

    private List<TableDto> FilteredTables =>
        tables.Where(t =>
            string.IsNullOrEmpty(searchText) ||
            t.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase))
        .ToList();

    protected override async Task OnInitializedAsync()
    {
        await LoadTables();
    }

    private async Task LoadTables()
    {
        isLoading = true;

        tables = await Http.GetFromJsonWithExceptionAsync<List<TableDto>>(ApiConfig.GetApiUrl("/api/tables")) ?? new();

        isLoading = false;
    }

    private void NavigateToAdd()
    {
        Navigation.NavigateTo("/tables/add");
    }

    private void NavigateToEdit(int id)
    {
        Navigation.NavigateTo($"/tables/edit/{id}");
    }

    private void ShowDeleteConfirmation(int id)
    {
        tableToDeleteId = id;
        showDeleteModal = true;
    }

    private async Task ConfirmDelete()
    {
        await Http.DeleteWithExceptionAsync(ApiConfig.GetApiUrl($"/api/tables/{tableToDeleteId}"));
        NotificationService.ShowSuccess("میز با موفقیت حذف شد");
        await LoadTables();
    }
}
