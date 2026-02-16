using System.Globalization;

namespace Gck.Common.Helpers;

public static class PersianDateHelper
{
    private static readonly PersianCalendar PersianCalendar = new();

    /// <summary>
    /// Convert DateTime to Persian date string (yyyy/MM/dd)
    /// </summary>
    public static string ToPersianDate(this DateTime dateTime)
    {
        return $"{PersianCalendar.GetYear(dateTime):0000}/{PersianCalendar.GetMonth(dateTime):00}/{PersianCalendar.GetDayOfMonth(dateTime):00}";
    }

    /// <summary>
    /// Convert DateTime to Persian date and time string (yyyy/MM/dd HH:mm)
    /// </summary>
    public static string ToPersianDateTime(this DateTime dateTime)
    {
        return $"{dateTime.ToPersianDate()} {dateTime:HH:mm}";
    }

    /// <summary>
    /// Convert DateTime to Persian date and time with seconds (yyyy/MM/dd HH:mm:ss)
    /// </summary>
    public static string ToPersianDateTimeFull(this DateTime dateTime)
    {
        return $"{dateTime.ToPersianDate()} {dateTime:HH:mm:ss}";
    }

    /// <summary>
    /// Get Persian day of week name
    /// </summary>
    public static string GetPersianDayOfWeek(this DateTime dateTime)
    {
        return dateTime.DayOfWeek switch
        {
            DayOfWeek.Saturday => "شنبه",
            DayOfWeek.Sunday => "یکشنبه",
            DayOfWeek.Monday => "دوشنبه",
            DayOfWeek.Tuesday => "سه‌شنبه",
            DayOfWeek.Wednesday => "چهارشنبه",
            DayOfWeek.Thursday => "پنجشنبه",
            DayOfWeek.Friday => "جمعه",
            _ => ""
        };
    }

    /// <summary>
    /// Get Persian month name
    /// </summary>
    public static string GetPersianMonthName(this DateTime dateTime)
    {
        var month = PersianCalendar.GetMonth(dateTime);
        return month switch
        {
            1 => "فروردین",
            2 => "اردیبهشت",
            3 => "خرداد",
            4 => "تیر",
            5 => "مرداد",
            6 => "شهریور",
            7 => "مهر",
            8 => "آبان",
            9 => "آذر",
            10 => "دی",
            11 => "بهمن",
            12 => "اسفند",
            _ => ""
        };
    }

    /// <summary>
    /// Convert DateTime to Persian date with month name (dd MonthName yyyy)
    /// </summary>
    public static string ToPersianDateWithMonthName(this DateTime dateTime)
    {
        var day = PersianCalendar.GetDayOfMonth(dateTime);
        var monthName = dateTime.GetPersianMonthName();
        var year = PersianCalendar.GetYear(dateTime);
        
        return $"{day} {monthName} {year}";
    }

    /// <summary>
    /// Convert DateTime to full Persian format with day of week (DayOfWeek, dd MonthName yyyy)
    /// </summary>
    public static string ToPersianDateFull(this DateTime dateTime)
    {
        var dayOfWeek = dateTime.GetPersianDayOfWeek();
        var dateWithMonth = dateTime.ToPersianDateWithMonthName();
        
        return $"{dayOfWeek}، {dateWithMonth}";
    }

    /// <summary>
    /// Get current Persian year
    /// </summary>
    public static int GetCurrentPersianYear()
    {
        return PersianCalendar.GetYear(DateTime.Now);
    }

    /// <summary>
    /// Get current Persian month
    /// </summary>
    public static int GetCurrentPersianMonth()
    {
        return PersianCalendar.GetMonth(DateTime.Now);
    }

    /// <summary>
    /// Get current Persian day
    /// </summary>
    public static int GetCurrentPersianDay()
    {
        return PersianCalendar.GetDayOfMonth(DateTime.Now);
    }

    /// <summary>
    /// Convert Persian date string to DateTime (yyyy/MM/dd format)
    /// Returns null if the date is invalid
    /// </summary>
    public static DateTime? FromPersianDate(string? persianDate)
    {
        if (string.IsNullOrWhiteSpace(persianDate))
            return null;

        try
        {
            var parts = persianDate.Split('/');
            if (parts.Length != 3)
                return null;

            if (!int.TryParse(parts[0], out var year) ||
                !int.TryParse(parts[1], out var month) ||
                !int.TryParse(parts[2], out var day))
                return null;

            // Validate Persian date ranges
            if (year < 1 || month < 1 || month > 12 || day < 1 || day > 31)
                return null;

            return PersianCalendar.ToDateTime(year, month, day, 0, 0, 0, 0);
        }
        catch
        {
            return null;
        }
    }

    /// <summary>
    /// Convert Persian date string to DateTime with default fallback
    /// Returns the provided default value if the date is invalid
    /// </summary>
    public static DateTime FromPersianDateOrDefault(string? persianDate, DateTime defaultValue = default)
    {
        return FromPersianDate(persianDate) ?? defaultValue;
    }

    /// <summary>
    /// Convert Persian date string to DateTime or current date
    /// Returns DateTime.Now if the date is invalid
    /// </summary>
    public static DateTime FromPersianDateOrNow(string? persianDate)
    {
        return FromPersianDate(persianDate) ?? DateTime.Now;
    }

    /// <summary>
    /// Get age in Persian years from birth year
    /// </summary>
    public static int GetPersianAge(int persianBirthYear)
    {
        var currentYear = GetCurrentPersianYear();
        return currentYear - persianBirthYear;
    }
}
