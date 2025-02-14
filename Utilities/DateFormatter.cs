namespace LibraryManagementSystem.Utilities;

public static class DateFormatter
{
    public static string ExtractDateOnly(DateTime dateTime)
    {
        return dateTime.Date.ToString("yyyy/MM/dd");
    }

    public static string FormatDateAndTime(DateTime dateTime)
    {
        return dateTime.ToString("yyyy/MM/dd hh:mm:ss tt");
    }
}