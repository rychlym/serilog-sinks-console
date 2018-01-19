using System;
using System.Globalization;

namespace ConsoleDemo
{
    public class DateTimeUt
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

        public static string ToInvariant(DateTime dt, string dtFormat = ISO8601UtcDateTime) => dt.ToString(dtFormat, CultureInfo.InvariantCulture);

        public static bool TryParseExact(string dateStr, string dateFormat, out DateTime dateTime)
        {
            return DateTime.TryParseExact(dateStr, dateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out dateTime);
        }

        public static string ConvertExactDateFormat(string inputDateStr, string inputFormat, string outputFormat, string parseFailOutput = "-")
        {
            if (TryParseExact(inputDateStr, inputFormat, out DateTime dateTime))
            {
                return dateTime.ToString(outputFormat);
            }
            return parseFailOutput;
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
