namespace DuyProject.API.Helpers;

public static class DateExtensions
{
    public static DateTime StartOfDay(this DateTime theDate) => theDate.Date;

    public static DateTime EndOfDay(this DateTime theDate)
    {
        DateTime dateTime = theDate.Date;
        dateTime = dateTime.AddDays(1.0);
        return dateTime.AddTicks(-1L);
    }

    public static DateTime ToMalaysiaTimezone(this DateTime theDate)
    {
        const string displayName = "(GMT+08:00) Malaysia Standard Time";
        const string standardName = "Malaysia Standard Time";
        var offset = new TimeSpan(08, 00, 00);
        var mls = TimeZoneInfo.CreateCustomTimeZone(standardName, offset, displayName, standardName);
        return TimeZoneInfo.ConvertTime(theDate, mls);
    }
}