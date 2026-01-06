namespace Gck.Services;

public class TournamentService
{
    private readonly HttpClient _httpClient;
    
    public TournamentService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Tournament>> GetActiveTournamentsAsync()
    {
        // Simulate API call
        await Task.Delay(400);
        
        return new List<Tournament>
        {
            new()
            {
                Id = "winter-championship-2024",
                Title = "??? ??????? ?????? ????",
                Description = "???????? ??????? ??? ?? ????? ??????????",
                Prize = 50000000,
                Currency = "?????",
                StartDate = DateTime.Now.AddDays(15),
                EndDate = DateTime.Now.AddDays(45),
                MaxParticipants = 256,
                CurrentParticipants = 128,
                EntryFee = 0,
                Games = new[] { "???? ?????", "????? ?????????", "?????? ????" },
                Status = TournamentStatus.Upcoming,
                Icon = "fas fa-trophy",
                Banner = "var(--gradient-1)"
            },
            new()
            {
                Id = "precision-shooter",
                Title = "??????? ????? ????",
                Description = "???? ????????? ???????",
                Prize = 15000000,
                Currency = "?????",
                StartDate = DateTime.Now.AddDays(3),
                EndDate = DateTime.Now.AddDays(5),
                MaxParticipants = 64,
                CurrentParticipants = 23,
                EntryFee = 0,
                Games = new[] { "????????? ????" },
                Status = TournamentStatus.Upcoming,
                Icon = "fas fa-bullseye",
                Banner = "var(--gradient-2)"
            },
            new()
            {
                Id = "speed-championship",
                Title = "??????? ????",
                Description = "??????? ???????????",
                Prize = 30000000,
                Currency = "?????",
                StartDate = DateTime.Now.AddDays(7),
                EndDate = DateTime.Now.AddDays(14),
                MaxParticipants = 128,
                CurrentParticipants = 45,
                EntryFee = 50000,
                Games = new[] { "?????? ????" },
                Status = TournamentStatus.Upcoming,
                Icon = "fas fa-flag-checkered",
                Banner = "var(--gradient-1)"
            }
        };
    }

    public async Task<Tournament?> GetTournamentAsync(string id)
    {
        var tournaments = await GetActiveTournamentsAsync();
        return tournaments.FirstOrDefault(t => t.Id == id);
    }

    public async Task<bool> RegisterForTournamentAsync(string tournamentId, string userId)
    {
        // Simulate API call for tournament registration
        await Task.Delay(500);
        
        // In a real app, this would validate user eligibility, process entry fee, etc.
        return true;
    }

    public async Task<List<TournamentResult>> GetTournamentResultsAsync(string tournamentId)
    {
        await Task.Delay(300);
        
        return new List<TournamentResult>
        {
            new() { Rank = 1, PlayerName = "???? ?????", Score = 15420, Prize = 25000000 },
            new() { Rank = 2, PlayerName = "??? ?????", Score = 14890, Prize = 15000000 },
            new() { Rank = 3, PlayerName = "????? ?????", Score = 14350, Prize = 10000000 },
            new() { Rank = 4, PlayerName = "??? ?????", Score = 13720, Prize = 0 },
            new() { Rank = 5, PlayerName = "???? ?????", Score = 13240, Prize = 0 }
        };
    }

    public async Task<List<TournamentCategory>> GetTournamentCategoriesAsync()
    {
        await Task.Delay(200);
        
        return new List<TournamentCategory>
        {
            new()
            {
                Id = "quick",
                Title = "??????????? ????",
                Description = "??????? ????? ??? ?? ????? ????",
                Icon = "?",
                Duration = "?-? ????",
                PrizeRange = "???K - ?M ?????",
                MaxPlayers = "?? ???",
                BackgroundGradient = "var(--gradient-2)"
            },
            new()
            {
                Id = "championship",
                Title = "??? ????????",
                Description = "??????? ??????? ?? ????? ????",
                Icon = "??",
                Duration = "?-? ????",
                PrizeRange = "??M - ???M ?????",
                MaxPlayers = "??? ???",
                BackgroundGradient = "var(--gradient-1)"
            },
            new()
            {
                Id = "team",
                Title = "??????? ????",
                Description = "??????? ????? ?? ??????",
                Icon = "??",
                Duration = "??????? ?-? ????",
                PrizeRange = "????? ??? ???",
                MaxPlayers = "?????? ? ????????",
                BackgroundGradient = "var(--gradient-3)"
            }
        };
    }
}

public class Tournament
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public long Prize { get; set; }
    public string Currency { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int MaxParticipants { get; set; }
    public int CurrentParticipants { get; set; }
    public long EntryFee { get; set; }
    public string[] Games { get; set; } = Array.Empty<string>();
    public TournamentStatus Status { get; set; }
    public string Icon { get; set; } = string.Empty;
    public string Banner { get; set; } = string.Empty;
    
    public TimeSpan TimeUntilStart => StartDate - DateTime.Now;
    public int DaysUntilStart => (int)Math.Ceiling(TimeUntilStart.TotalDays);
    public bool CanRegister => Status == TournamentStatus.Upcoming && CurrentParticipants < MaxParticipants;
    public int AvailableSlots => MaxParticipants - CurrentParticipants;
    public double FillPercentage => (double)CurrentParticipants / MaxParticipants * 100;
}

public class TournamentResult
{
    public int Rank { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public long Prize { get; set; }
}

public class TournamentCategory
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Icon { get; set; } = string.Empty;
    public string Duration { get; set; } = string.Empty;
    public string PrizeRange { get; set; } = string.Empty;
    public string MaxPlayers { get; set; } = string.Empty;
    public string BackgroundGradient { get; set; } = string.Empty;
}

public enum TournamentStatus
{
    Upcoming,
    Active,
    Finished,
    Cancelled
}