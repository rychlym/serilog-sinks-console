using System;
using System.Globalization;

namespace ConsoleDemo
{
    public static class DateTimeUt
    {
        // like ICultureInfo.InvariantCulture.DateTimeFormat.UniversalSortableDateTimePattern
        public const string ISO8601LocalDateTime = "yyyy'-'MM'-'dd'T'HH':'mm':'ss";

        /// <summary>The ISO8601 UTC date time format.</summary>
        public const string ISO8601UtcDateTime = ISO8601LocalDateTime + "Z";

        /// <summary>The ISO8601 date time format with 3decimals for milliseconds.</summary>
        public const string ISO86013MsLocalDateTime = ISO8601LocalDateTime + ".fff";

        /// <summary>The ISO8601 UTC date time format with 3decimals for milliseconds.</summary>
        public const string ISO86013MsUtcDateTime = ISO8601LocalDateTime + ".fffZ";

        /// <summary>The short local date time (till the minutes) format</summary>
        public const string NoSepMnLocalDateTimeFormat = "yyyyMMdd'T'HHmm";

        /// <summary>The short UTC date time (till the minutes) format</summary>
        public const string NoSepMnUtcDateTimeFormat = NoSepMnLocalDateTimeFormat + "Z";

        public static string ToInvariant(DateTime dt, string dtFormat = ISO8601LocalDateTime) => dt.ToString(dtFormat, CultureInfo.InvariantCulture);
        public static string ToUtcInvariant(DateTime dt, string dtFormat = ISO8601UtcDateTime) => dt.ToUniversalTime().ToString(dtFormat, CultureInfo.InvariantCulture);

        public static bool TryParseExact(string dateTimeStr, string dateTimeFormat, out DateTime dateTime)
            => DateTime.TryParseExact(dateTimeStr, dateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);

        public static bool TryParseExactUtc(string utcDateTimeStr, string utcDateTimeFormat, out DateTime dateTime)
            => DateTime.TryParseExact(utcDateTimeStr, utcDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal | DateTimeStyles.AdjustToUniversal, out dateTime);

        public static bool TryParseExactLocal(string utcDateTimeStr, string utcDateTimeFormat, out DateTime dateTime)
            => DateTime.TryParseExact(utcDateTimeStr, utcDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dateTime);

        public static DateTime ParseExactToLocalDateTime(string ltDateTimeStr, string localDateTimeFormat = ISO8601LocalDateTime)
            => DateTime.ParseExact(ltDateTimeStr, localDateTimeFormat, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal).ToLocalTime();


        public static string ConvertExactDateFormat(string inputDateStr, string inputFormat, string outputFormat, string parseFailOutput = "-")
        {
            if (TryParseExact(inputDateStr, inputFormat, out DateTime dateTime))
            {
                return dateTime.ToString(outputFormat);
            }
            return parseFailOutput;
        }


        /// <summary>
        /// Tries to parse the given datetime string that is not annotated with a timezone 
        /// information but known to be in the CET/CEST zone and returns a DateTime struct
        /// in UTC (so it can be converted to the devices local time). If it could not be 
        /// parsed, result contains the current date/time in UTC.
        /// </summary>
        public static bool TryParseCetCestToUtc(string s, string isoFormat, out DateTime result)
        {
            // Parse datetime, knowing it is in CET/CEST timezone. Parse as universal as we fix it afterwards
            if (s.EndsWith(" CET")) s = s.Substring(0, s.Length - 4);
            else if (s.EndsWith(" CEST")) s = s.Substring(0, s.Length - 5);

            if (!DateTime.TryParseExact(s, isoFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out result))
            {
                result = DateTime.UtcNow;
                return false;
            }
            result = DateTime.SpecifyKind(result, DateTimeKind.Utc);

            // The boundaries of the daylight saving time period in CET and CEST (_not_ in UTC!)
            // Both DateTime structs are of kind 'Utc', to be able to compare them with the parsing result
            DateTime DstStart = LastSundayOf(result.Year, 3).AddHours(2);
            DateTime DstEnd = LastSundayOf(result.Year, 10).AddHours(3);

            // Are we inside the daylight saving time period?
            if (DstStart.CompareTo(result) <= 0 && result.CompareTo(DstEnd) < 0)
                result = result.AddHours(-2); // CEST = UTC+2h
            else
                result = result.AddHours(-1); // CET = UTC+1h

            return true;
        }

        /// <summary>
        /// Returns the last Sunday of the given month and year in UTC
        /// </summary>
        private static DateTime LastSundayOf(int year, int month)
        {
            DateTime firstOfNextMonth = new DateTime(year, month + 1, 1, 0, 0, 0, DateTimeKind.Utc);
            return firstOfNextMonth.AddDays(firstOfNextMonth.DayOfWeek == DayOfWeek.Sunday ? -7 :
                                                        (-1 * (int)firstOfNextMonth.DayOfWeek));
        }

        public static bool TryParseDateTimeFromSharePointProperty(string propertyValue, out DateTime dateTime)
        {
            try
            {
                var dateAndTime = propertyValue.Split(' ');
                var dateValues = dateAndTime[0].Split('/');
                var timeValues = dateAndTime[1].Split(':');

                var year = int.Parse(dateValues[2]);
                var month = int.Parse(dateValues[0]);
                var day = int.Parse(dateValues[1]);
                var hours = int.Parse(timeValues[0]);
                var minutes = int.Parse(timeValues[1]);
                var seconds = int.Parse(timeValues[2]);

                dateTime = new DateTime(year, month, day, hours, minutes, seconds);
                return true;
            }
            catch (Exception)
            {

            }

            dateTime = DateTime.MinValue;
            return false;
        }
    }

}

