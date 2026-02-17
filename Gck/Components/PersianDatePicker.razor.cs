using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace Gck.Components;

public partial class PersianDatePicker
{
    [Inject] private IJSRuntime JS { get; set; } = default!;
    
    [Parameter]
    public string Value { get; set; } = string.Empty;
    
    [Parameter]
    public EventCallback<string> ValueChanged { get; set; }
    
    private bool isOpen = false;
    private int currentYear;
    private int currentMonth;
    private DateTime tempSelectedDate;
    private readonly System.Globalization.PersianCalendar calendar = new();
    
    private string[] weekDays = { "ش", "ی", "د", "س", "چ", "پ", "ج" };
    private string[] monthNames = { 
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    };
    
    protected override void OnInitialized()
    {
        if (!string.IsNullOrEmpty(Value))
        {
            tempSelectedDate = ParsePersianDate(Value);
        }
        else
        {
            tempSelectedDate = DateTime.Now;
        }
        
        currentYear = calendar.GetYear(tempSelectedDate);
        currentMonth = calendar.GetMonth(tempSelectedDate);
    }
    
    private void TogglePicker()
    {
        isOpen = !isOpen;
    }
    
    private void PreviousMonth()
    {
        currentMonth--;
        if (currentMonth < 1)
        {
            currentMonth = 12;
            currentYear--;
        }
    }
    
    private void NextMonth()
    {
        currentMonth++;
        if (currentMonth > 12)
        {
            currentMonth = 1;
            currentYear++;
        }
    }
    
    private async Task SelectDate(DayInfo day)
    {
        if (!day.IsCurrentMonth) return;
        
        var persianDate = $"{currentYear:0000}/{currentMonth:00}/{day.Day:00}";
        
        await ValueChanged.InvokeAsync(persianDate);
        isOpen = false;
    }
    
    private async Task ConfirmSelection()
    {
        var persianDate = $"{currentYear:0000}/{currentMonth:00}/{calendar.GetDayOfMonth(tempSelectedDate):00}";
        
        await ValueChanged.InvokeAsync(persianDate);
        isOpen = false;
    }
    
    private string GetDisplayText()
    {
        if (string.IsNullOrEmpty(Value))
            return "تاریخ را انتخاب کنید";
            
        return Value;
    }
    
    private string CurrentMonthName => monthNames[currentMonth - 1];
    private string CurrentYear => currentYear.ToString();
    
    private List<DayInfo> GetCalendarDays()
    {
        var days = new List<DayInfo>();
        var firstDayOfMonth = calendar.ToDateTime(currentYear, currentMonth, 1, 0, 0, 0, 0);
        
        var dayOfWeek = ((int)firstDayOfMonth.DayOfWeek + 1) % 7;
        
        var prevYear = currentMonth == 1 ? currentYear - 1 : currentYear;
        var prevMonth = currentMonth == 1 ? 12 : currentMonth - 1;
        var daysInPrevMonth = calendar.GetDaysInMonth(prevYear, prevMonth);
        for (int i = dayOfWeek - 1; i >= 0; i--)
        {
            days.Add(new DayInfo { Day = daysInPrevMonth - i, IsCurrentMonth = false });
        }
        
        var daysInMonth = calendar.GetDaysInMonth(currentYear, currentMonth);
        var today = DateTime.Now;
        var todayYear = calendar.GetYear(today);
        var todayMonth = calendar.GetMonth(today);
        var todayDay = calendar.GetDayOfMonth(today);
        
        var selected = string.IsNullOrEmpty(Value) ? DateTime.Now : ParsePersianDate(Value);
        var selectedYear = calendar.GetYear(selected);
        var selectedMonth = calendar.GetMonth(selected);
        var selectedDay = calendar.GetDayOfMonth(selected);
        
        for (int i = 1; i <= daysInMonth; i++)
        {
            days.Add(new DayInfo 
            { 
                Day = i, 
                IsCurrentMonth = true,
                IsToday = todayYear == currentYear && todayMonth == currentMonth && todayDay == i,
                IsSelected = selectedYear == currentYear && selectedMonth == currentMonth && selectedDay == i
            });
        }
        
        var remainingCells = 42 - days.Count;
        for (int i = 1; i <= remainingCells; i++)
        {
            days.Add(new DayInfo { Day = i, IsCurrentMonth = false });
        }
        
        return days;
    }
    
    private DateTime ParsePersianDate(string persianDate)
    {
        var parts = persianDate.Split('/');
        if (parts.Length >= 3)
        {
            if (int.TryParse(parts[0], out int year) && 
                int.TryParse(parts[1], out int month) && 
                int.TryParse(parts[2], out int day))
            {
                return calendar.ToDateTime(year, month, day, 0, 0, 0, 0);
            }
        }
        return DateTime.Now;
    }
    
    private class DayInfo
    {
        public int Day { get; set; }
        public bool IsCurrentMonth { get; set; }
        public bool IsToday { get; set; }
        public bool IsSelected { get; set; }
    }
}
