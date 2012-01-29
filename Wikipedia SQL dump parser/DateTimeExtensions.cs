using System;
using System.Globalization;

namespace WpSqlDumpParser
{
    public static class DateTimeExtensions
    {
        public static DateTime? ParseDate(string s)
        {
            DateTime result;
            if (DateTime.TryParseExact(s, "yyyyMMdd", CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
                return result;
            return null;
        }
    }
}