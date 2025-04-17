using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace ADValidation.Helpers.TimeZone
{
    public static class TimeZoneHelper
    {
        private const string UkrainianTimeZoneIdWindows = "FLE Standard Time";
        private const string UkrainianTimeZoneIdUnix = "Europe/Kiev";
        private const string DefaultDateTimeFormat = "yyyy-MM-ddTHH:mm:sszzz"; // ISO 8601 with offset
        
        /// <summary>
        /// Gets the Ukrainian timezone info (automatically handles DST)
        /// </summary>
        public static TimeZoneInfo GetUkrainianTimeZone()
        {
            var timeZoneId = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) 
                ? UkrainianTimeZoneIdWindows 
                : UkrainianTimeZoneIdUnix;
            
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }

        /// <summary>
        /// Parses a local DateTime string with timezone and converts it to UTC
        /// </summary>
        /// <param name="dateTimeString">The local DateTime string (e.g., "2023-05-15T14:30:00+03:00")</param>
        /// <returns>Tuple containing UTC DateTime and original timezone offset</returns>
        public static (DateTime UtcTime, TimeSpan OriginalOffset) ParseLocalToUtc(string dateTimeString)
        {
            if (DateTimeOffset.TryParse(dateTimeString, out var dto))
            {
                return (dto.UtcDateTime, dto.Offset);
            }

            throw new FormatException($"Invalid DateTime format: {dateTimeString}");
        }
        
        /// <summary>
        /// Converts a local DateTime with specified timezone offset to UTC
        /// </summary>
        /// <param name="localTime">The local DateTime value</param>
        /// <param name="offset">The timezone offset from UTC</param>
        /// <returns>Tuple containing UTC DateTime and original timezone offset</returns>
        public static (DateTime UtcTime, TimeSpan OriginalOffset) ConvertLocalToUtc(DateTime localTime, TimeSpan offset)
        {
            if (localTime.Kind == DateTimeKind.Utc)
            {
                throw new ArgumentException("Input time is already UTC", nameof(localTime));
            }

            // Create DateTimeOffset using the local time and offset
            var dto = new DateTimeOffset(localTime, offset);
            return (dto.UtcDateTime, offset);
        }

        /// <summary>
        /// Overload that accepts DateTimeOffset directly
        /// </summary>
        public static (DateTime UtcTime, TimeSpan OriginalOffset) ConvertLocalToUtc(DateTimeOffset dto)
        {
            return (dto.UtcDateTime, dto.Offset);
        }
        

        /// <summary>
        /// Parses a local DateTime string with Ukrainian timezone and converts it to UTC
        /// </summary>
        /// <param name="dateTimeString">The local DateTime string without offset (e.g., "2023-05-15 14:30:00")</param>
        /// <returns>Tuple containing UTC DateTime and Ukrainian timezone offset</returns>
        public static (DateTime UtcTime, TimeSpan UkrainianOffset) ParseUkrainianToUtc(string dateTimeString)
        {
            if (DateTime.TryParse(dateTimeString, out var localTime))
            {
                var ukrainianTz = GetUkrainianTimeZone();
                var ukrainianTime = DateTime.SpecifyKind(localTime, DateTimeKind.Unspecified);
                var utcTime = TimeZoneInfo.ConvertTimeToUtc(ukrainianTime, ukrainianTz);
                var offset = ukrainianTz.GetUtcOffset(utcTime);
                
                return (utcTime, offset);
            }

            throw new FormatException($"Invalid DateTime format: {dateTimeString}");
        }
        
        /// <summary>
        /// Converts UTC DateTime to Ukrainian timezone
        /// </summary>
        public static DateTime ConvertUtcToUkrainianTime(DateTime utcTime)
        {
            if (utcTime.Kind != DateTimeKind.Utc)
            {
                throw new ArgumentException("Input time must be UTC", nameof(utcTime));
            }

            return TimeZoneInfo.ConvertTimeFromUtc(utcTime, GetUkrainianTimeZone());
        }

        /// <summary>
        /// Formats a UTC DateTime as a Ukrainian timezone string with offset
        /// </summary>
        public static string FormatUkrainianTimeWithOffset(DateTime utcTime)
        {
            var ukrainianTime = ConvertUtcToUkrainianTime(utcTime);
            var offset = GetUkrainianTimeZone().GetUtcOffset(ukrainianTime);
            return $"{ukrainianTime:yyyy-MM-ddTHH:mm:ss}{offset.Hours:+00;-00}:{offset.Minutes:00}";
        }

        /// <summary>
        /// Converts UTC DateTime to formatted Ukrainian time string
        /// </summary>
        public static string FormatUkrainianTime(DateTime utcTime, string format = "G")
        {
            var ukrainianTime = ConvertUtcToUkrainianTime(utcTime);
            return ukrainianTime.ToString(format, new CultureInfo("uk-UA"));
        }

        /// <summary>
        /// Gets current time in Ukrainian timezone
        /// </summary>
        public static DateTime GetCurrentUkrainianTime()
        {
            return ConvertUtcToUkrainianTime(DateTime.UtcNow);
        }

        /// <summary>
        /// Checks if a given UTC time is during Ukrainian Daylight Saving Time
        /// </summary>
        public static bool IsUkrainianDaylightSavingTime(DateTime utcTime)
        {
            var timeZone = GetUkrainianTimeZone();
            var ukrainianTime = ConvertUtcToUkrainianTime(utcTime);
            return timeZone.IsDaylightSavingTime(ukrainianTime);
        }

        /// <summary>
        /// Gets the current UTC offset for Ukrainian timezone (including DST)
        /// </summary>
        public static TimeSpan GetCurrentUkrainianOffset()
        {
            return GetUkrainianTimeZone().GetUtcOffset(DateTime.UtcNow);
        }
    }
}