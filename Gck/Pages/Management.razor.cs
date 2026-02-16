using Microsoft.AspNetCore.Components;
using Gck.Components;
using Gck.Services;
using Gck.Common.Extensions;
using System.Net.Http.Json;

namespace Gck.Pages
{
    public partial class Management : ComponentBase, IDisposable
    {
        [Inject]
        private HttpClient Http { get; set; } = default!;

        [Inject]
        private ApiConfigurationService ApiConfig { get; set; } = default!;

        private List<DashboardTableDto> dashboardTables = new();
        private bool loading = true;
        private DateTime currentTime = DateTime.Now;
        private System.Threading.Timer? timer;

        private StartSessionModal? startSessionModal;
        private FinishSessionModal? finishSessionModal;
        private int selectedTableId;
        private string selectedTableName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await LoadData();
            StartTimer();
        }

        private void StartTimer()
        {
            timer = new System.Threading.Timer(async _ =>
            {
                currentTime = DateTime.Now;
                await InvokeAsync(StateHasChanged);
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(1));
        }

        private async Task LoadData()
        {
            loading = true;

            dashboardTables = await Http.GetFromJsonWithExceptionAsync<List<DashboardTableDto>>(ApiConfig.GetApiUrl("/api/dashboard/tables")) ?? new();

            loading = false;
            currentTime = DateTime.Now;
        }

        private async Task ShowStartModal(int tableId, string tableName)
        {
            selectedTableId = tableId;
            selectedTableName = tableName;

            if (startSessionModal != null)
            {
                await startSessionModal.Show();
                StateHasChanged();
            }
        }

        private async Task ShowFinishModal(int sessionId)
        {
            if (finishSessionModal != null)
            {
                await finishSessionModal.Show(sessionId);
            }
        }

        private async Task HandleSessionChanged()
        {
            await LoadData();
        }

        private string GetSessionDuration(DateTime startTime)
        {
            var duration = currentTime - startTime;
            var hours = (int)duration.TotalHours;
            var minutes = duration.Minutes;
            var seconds = duration.Seconds;
            
            return $"{hours:D2}:{minutes:D2}:{seconds:D2}";
        }

        public void Dispose()
        {
            timer?.Dispose();
        }

        // DTOs
        public class DashboardTableDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public bool IsOccupied { get; set; }
            public SessionDto? CurrentSession { get; set; }
        }

        public class SessionDto
        {
            public int Id { get; set; }
            public int TableId { get; set; }
            public string TableName { get; set; } = string.Empty;
            public decimal FeePerHour { get; set; }
            public DateTime StartDateTime { get; set; }
            public DateTime? EndDateTime { get; set; }
            public bool IsCompleted { get; set; }
            public decimal? RecommendedPrice { get; set; }
            public decimal? FinalPrice { get; set; }
            public List<CustomerDto> Customers { get; set; } = new();
            public string Duration { get; set; } = string.Empty;
        }

        public class CustomerDto
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string PhoneNumber { get; set; } = string.Empty;
            public int BirthYear { get; set; }
            public string Gender { get; set; } = string.Empty;
        }
    }
}
