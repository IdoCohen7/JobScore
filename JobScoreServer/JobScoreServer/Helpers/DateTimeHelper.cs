using System.Globalization;

namespace JobScoreServer.Helpers
{
    public static class DateTimeHelper
    {

        public static int GetWeekOfYear(DateTime date)
        {
            var culture = CultureInfo.CurrentCulture;
            var calendar = culture.Calendar;
            var weekRule = culture.DateTimeFormat.CalendarWeekRule;
            var firstDayOfWeek = culture.DateTimeFormat.FirstDayOfWeek;
            
            return calendar.GetWeekOfYear(date, weekRule, firstDayOfWeek);
        }

        public static string GetMonthName(int month)
        {
            return new DateTime(2000, month, 1).ToString("MMM", CultureInfo.InvariantCulture);
        }
    }
}
