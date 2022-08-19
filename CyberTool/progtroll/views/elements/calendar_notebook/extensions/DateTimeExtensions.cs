using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace progtroll.views.elements.calendar_notebook.extensions
{
    internal static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = (7 + (dt.DayOfWeek - startOfWeek)) % 7;
            return dt.AddDays(-1 * diff).Date;
        }

        public static DateTime EndOfWeek(this DateTime dt, DayOfWeek endOfWeek)
        {
            int diff = (7 + (endOfWeek - dt.DayOfWeek)) % 7;
            return dt.AddDays(diff).Date;
        }

        public static DateTime StartOfMonth(this DateTime dt)
        {
            var firstDayOfMonth = new DateTime(dt.Year, dt.Month, 1);
            return firstDayOfMonth;
        }

        public static DateTime EndOfMonth(this DateTime dt)
        {
            var lastDayOfMonth = new DateTime(dt.Year, dt.Month, 1).AddMonths(1).AddDays(-1);
            return lastDayOfMonth;
        }

        public static DateTime StartOfYear(this DateTime dt)
        {
            var firstDayOfYear = new DateTime(dt.Year, 1, 1);
            return firstDayOfYear;
        }

        public static DateTime EndOfYear(this DateTime dt)
        {
            var lastDayOfYear = new DateTime(dt.Year, 1, 1).AddYears(1).AddDays(-1);
            return lastDayOfYear;
        }

        public static int TotalMonths(this DateTime startDate, DateTime endDate)
        {
            var total = ((endDate.Year - startDate.Year) * 12) + endDate.Month - startDate.Month;
            return total;
        }
    }
}
