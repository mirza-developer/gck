using Blazored.LocalStorage;

namespace Gck.Services;

public class UserProfileService
{
    private readonly ILocalStorageService _localStorage;
    private readonly HttpClient _httpClient;
    
    public UserProfileService(ILocalStorageService localStorage, HttpClient httpClient)
    {
        _localStorage = localStorage;
        _httpClient = httpClient;
    }

    public async Task<UserProfile> GetUserProfileAsync()
    {
        // Try to get from local storage first
        var cachedProfile = await _localStorage.GetItemAsync<UserProfile>("userProfile");
        if (cachedProfile != null)
        {
            return cachedProfile;
        }

        // Simulate API call for guest user
        var guestProfile = new UserProfile
        {
            Id = "guest",
            Username = "????? ?????",
            DisplayName = "????? ?????",
            Email = "guest@gckgames.ir",
            JoinDate = DateTime.Now.AddDays(-30),
            Level = 7,
            Experience = 2450,
            ExperienceToNextLevel = 500,
            Coins = 12500,
            Gems = 45,
            Avatar = "gradient-1",
            IsGuest = true,
            Achievements = new List<Achievement>
            {
                new() { Id = "first-win", Title = "????? ??????", Description = "????? ???? ??? ?? ????? ????", Icon = "fas fa-trophy", UnlockedAt = DateTime.Now.AddDays(-25) },
                new() { Id = "speed-demon", Title = "????? ????", Description = "?? ?? ?????? ???? ????? ????", Icon = "fas fa-tachometer-alt", UnlockedAt = DateTime.Now.AddDays(-15) },
                new() { Id = "strategist", Title = "??????????", Description = "? ???? ????? ?????? ?????", Icon = "fas fa-chess-king", UnlockedAt = DateTime.Now.AddDays(-10) }
            },
            RecentGames = new List<RecentGame>
            {
                new() { GameTitle = "???? ?????", PlayedAt = DateTime.Now.AddHours(-2), Score = 1250, Won = true },
                new() { GameTitle = "????? ?????????", PlayedAt = DateTime.Now.AddHours(-5), Score = 0, Won = false },
                new() { GameTitle = "?????? ????", PlayedAt = DateTime.Now.AddDays(-1), Score = 3400, Won = true }
            }
        };

        // Cache the profile
        await _localStorage.SetItemAsync("userProfile", guestProfile);
        return guestProfile;
    }

    public async Task UpdateUserProfileAsync(UserProfile profile)
    {
        await _localStorage.SetItemAsync("userProfile", profile);
        // In a real app, you would also send to API
    }

    public async Task AddExperienceAsync(int experience)
    {
        var profile = await GetUserProfileAsync();
        profile.Experience += experience;
        
        // Check for level up
        while (profile.Experience >= profile.ExperienceToNextLevel)
        {
            profile.Experience -= profile.ExperienceToNextLevel;
            profile.Level++;
            profile.ExperienceToNextLevel = profile.Level * 100; // Simple formula
        }
        
        await UpdateUserProfileAsync(profile);
    }

    public async Task AddCoinsAsync(int coins)
    {
        var profile = await GetUserProfileAsync();
        profile.Coins += coins;
        await UpdateUserProfileAsync(profile);
    }

    public async Task<bool> SpendCoinsAsync(int coins)
    {
        var profile = await GetUserProfileAsync();
        if (profile.Coins >= coins)
        {
            profile.Coins -= coins;
            await UpdateUserProfileAsync(profile);
            return true;
        }
        return false;
    }
}

public class UserProfile
{
    public string Id { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public DateTime JoinDate { get; set; }
    public int Level { get; set; }
    public int Experience { get; set; }
    public int ExperienceToNextLevel { get; set; }
    public int Coins { get; set; }
    public int Gems { get; set; }
    public string Avatar { get; set; } = string.Empty;
    public bool IsGuest { get; set; }
    public List<Achievement> Achievements { get; set; } = new();
    public List<RecentGame> RecentGames { get; set; } = new();
}

public class Achievement
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public DateTime UnlockedAt { get; set; }
}

public class RecentGame
{
    public string GameTitle { get; set; } = string.Empty;
    public DateTime PlayedAt { get; set; }
    public int Score { get; set; }
    public bool Won { get; set; }
}