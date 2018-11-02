using System;

namespace KaSon.FrameWork.Common.ChinaRailway
{
    public static class DateTimeHelper
    {
        public static readonly DateTime UTCOrigin;

        static DateTimeHelper()
        {
            UTCOrigin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
        }

        public static DateTime GetLocalTime(DateTime universal)
        {
            return universal.ToLocalTime();
        }

        public static DateTime GetUniversalTime(DateTime local)
        {
            return local.ToUniversalTime();
        }

        public static string GetDateString(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd");
        }

        public static string GetTimeString(DateTime datetime)
        {
            return datetime.ToString("HH:mm:ss");
        }

        public static string GetDateTimeString(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss");
        }

        public static string GetDateTimeWithMillisecondsString(DateTime datetime)
        {
            return datetime.ToString("yyyy-MM-dd HH:mm:ss.fff");
        }

        public static string GetCurrentDateString(DateTime datetime)
        {
            return GetCurrentDateString(DateTime.Now);
        }

        public static string GetCurrentTimeString(DateTime datetime)
        {
            return GetCurrentTimeString(DateTime.Now);
        }

        public static string GetCurrentDateTimeString(DateTime datetime)
        {
            return GetCurrentDateTimeString(DateTime.Now);
        }

        public static string GetCurrentDateTimeWithMillisecondsString(DateTime datetime)
        {
            return GetCurrentDateTimeWithMillisecondsString(DateTime.Now);
        }

        public static ulong LocalDateTimeToUnixTimeStamp(DateTime date)
        {
            return (ulong)Math.Floor((date.ToUniversalTime() - UTCOrigin).TotalSeconds);
        }

        public static DateTime UnixTimeStampToLocalDateTime(ulong timestamp)
        {
            return UTCOrigin.ToLocalTime().AddSeconds(timestamp);
        }

        public static ulong UniversalDateTimeToUnixTimeStamp(DateTime date)
        {
            return (ulong)Math.Floor((date - UTCOrigin).TotalSeconds);
        }

        public static DateTime UnixTimeStampToUniversalDateTime(ulong timestamp)
        {
            return UTCOrigin.AddSeconds(timestamp);
        }

        public static ulong GetCurrentUnixTimeStamp()
        {
            return LocalDateTimeToUnixTimeStamp(DateTime.Now);
        }

        public static ulong LocalDateTimeToJavaTimeStamp(DateTime date)
        {
            return (ulong)Math.Floor((date.ToUniversalTime() - UTCOrigin).TotalMilliseconds);
        }

        public static DateTime JavaTimeStampToLocalDateTime(ulong timestamp)
        {
            return UTCOrigin.ToLocalTime().AddMilliseconds(timestamp);
        }

        public static ulong UniversalDateTimeToJavaTimeStamp(DateTime date)
        {
            return (ulong)Math.Floor((date - UTCOrigin).TotalMilliseconds);
        }

        public static DateTime JavaTimeStampToUniversalDateTime(ulong timestamp)
        {
            return UTCOrigin.AddMilliseconds(timestamp);
        }

        public static ulong GetCurrentJavaTimestamp()
        {
            return LocalDateTimeToJavaTimeStamp(DateTime.Now);
        }
    }
}
