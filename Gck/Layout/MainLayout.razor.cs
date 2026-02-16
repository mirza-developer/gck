using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gck.Layout
{
    public partial class MainLayout : IDisposable
    {
        [Inject]
        private IJSRuntime JS { get; set; } = default!;

        [Inject]
        private Blazored.LocalStorage.ILocalStorageService LocalStorage { get; set; } = default!;

        [Inject]
        private NavigationManager Navigation { get; set; } = default!;

        private bool isLoggedIn = false;
        private string currentUserName = string.Empty;
        private string userRole = string.Empty;
        private bool isMobileMenuOpen = false;

        protected override async Task OnInitializedAsync()
        {
            await CheckLoginStatus();
            Navigation.LocationChanged += OnLocationChanged;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await JS.InvokeVoidAsync("initFooterMap");
            }
        }

        private async void OnLocationChanged(object? sender, Microsoft.AspNetCore.Components.Routing.LocationChangedEventArgs e)
        {
            await CheckLoginStatus();
            StateHasChanged();
        }

        private async Task CheckLoginStatus()
        {
            var user = await LocalStorage.GetItemAsync<string>("currentUser");
            var name = await LocalStorage.GetItemAsync<string>("currentUserName");
            var role = await LocalStorage.GetItemAsync<string>("userRole");
            isLoggedIn = !string.IsNullOrEmpty(user);
            currentUserName = name ?? user ?? string.Empty;
            userRole = role ?? "admin"; // Default to admin for backward compatibility
        }

        private async Task HandleLogout()
        {
            await LocalStorage.RemoveItemAsync("currentUser");
            await LocalStorage.RemoveItemAsync("currentUserId");
            await LocalStorage.RemoveItemAsync("currentUserName");
            await LocalStorage.RemoveItemAsync("userRole");
            Navigation.NavigateTo("/login", true);
        }

        private void ToggleMobileMenu()
        {
            isMobileMenuOpen = !isMobileMenuOpen;
        }

        public void Dispose()
        {
            Navigation.LocationChanged -= OnLocationChanged;
        }
    }
}
