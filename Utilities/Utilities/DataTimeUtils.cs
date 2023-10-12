namespace Utilities.Utilities
{
    public static class DataTimeUtils
    {
        public static long NowMilis()
        {
            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        }

        public static string NewGuidStr()
        {
            return Guid.NewGuid().ToString("N");
        }

        public static long NowDateMilis()
        {
            var now = NowMilis();

            return now - now % (long)TimeSpan.FromDays(1).TotalMilliseconds;
        }

        public static long YesterdayDateMilis()
        {
            return NowDateMilis() - (long)TimeSpan.FromHours(24).TotalMilliseconds;
        }

        public static DateTime DateTimeFromMilis(long unixTimeStamp)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddMilliseconds(unixTimeStamp).ToUniversalTime();
        }

        public static DateTime DateTimeFromSeconds(long seconds)
        {
            var dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return dtDateTime.AddSeconds(seconds).ToUniversalTime();
        }

        public static DateTime ConvertFromUnixTimestamp(long timestamp)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddSeconds(timestamp);
        }

        public static long ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalSeconds);
        }

        public static DateTime NowDate()
        {
            return DateTime.UtcNow;
        }

        public static long DateTimeToUnixInMilliSeconds(DateTime dateTime)
        {
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        public static long DateTimeToUnixSeconds(DateTime dateTime)
        {
            return (long)dateTime.Subtract(new DateTime(1970, 1, 1)).TotalSeconds;
        }

        public static DateTime DateTimeToRemoveMinutes(DateTime dateTime)
        {
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, dateTime.Hour, dateTime.Minute, 0);
        }

        public static bool TimeBetween(long requestTime, long? min, long? max)
        {
            if (min is null || max is null)
                return false;
            return requestTime > min && requestTime < max;
        }

        public static bool TimeBetween(long? min, long? max)
        {
            long current = NowMilis();
            return TimeBetween(current, min, max);
        }

        public static bool TimeBetween(DateTime requestTime, DateTime? min, DateTime? max)
        {
            if (min is null || max is null)
                return false;
            return requestTime > min && requestTime < max;
        }

        public static bool TimeBetween(DateTime? min, DateTime? max)
        {
            DateTime current = DateTime.UtcNow;
            return TimeBetween(current, min, max);
        }
    }
}
