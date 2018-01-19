using System;
using System.Globalization;

namespace ConsoleDemo
{
    /// <summary>An custom short date pattern date-time provider. It works as default format provider and in case of sort date allows inject a custom pattern.</summary>
    public class CustomShortDateProvider : IFormatProvider, ICustomFormatter
    {
        public static readonly string[] ShortDateTimeSeparators = { " ", "T", "'T'" };
        readonly IFormatProvider basedOn;

        private readonly string shortDateTimePattern;
        private readonly string shortDateTimePatternZ;
        private readonly string shortDateTimePatternLocal;

        readonly string datePattern;
        readonly string timePattern;

        readonly DateTimeFormatInfo dateFormatInfo;

        public CustomShortDateProvider(string shortUnspecDateTimePattern = DateTimeUt.ISO8601LocalDateTime, IFormatProvider basedOn = null)
        {
            this.basedOn = basedOn ?? CultureInfo.CurrentCulture;
            this.shortDateTimePattern = shortUnspecDateTimePattern;
            this.shortDateTimePatternZ = shortUnspecDateTimePattern;
            this.shortDateTimePatternLocal = shortUnspecDateTimePattern;
            if (shortUnspecDateTimePattern.EndsWith("Z"))
            {
                this.shortDateTimePatternLocal = shortUnspecDateTimePattern.Substring(0, shortUnspecDateTimePattern.Length - 1);
            }
            else
            {
                this.shortDateTimePatternZ = this.shortDateTimePatternZ + 'Z';
            }

            if (this.TryGetDateTimePatterns(this.shortDateTimePatternLocal, out string datePattern, out string timePattern))
            {
                this.datePattern = datePattern;
                this.timePattern = timePattern;
                this.dateFormatInfo = this.GetFormatForDateTimeFormatInfo();
            }            
        }

        private bool TryGetDateTimePatterns(string shortDateTimePattern, out string datePattern, out string timePattern)
        {
            datePattern = null;
            timePattern = null;
            var matchCount = 0;
            var matchIndex = 0;
            var matchSep = default(string);
            for (var i = 0; i < ShortDateTimeSeparators.Length; i++)
            {
                var shortDateTimeSep = ShortDateTimeSeparators[i];
                matchIndex = this.shortDateTimePattern.IndexOf(shortDateTimeSep);
                if (matchIndex != -1)
                {
                    matchCount++;
                    matchSep = shortDateTimeSep;
                }
            }
            //if (matchCount == 0) throw new ArgumentException("The missing date-time separator in the short date time pattern!", nameof(shortDatePattern));
            if (matchCount == 0) return false;
            datePattern = this.shortDateTimePattern.Substring(0, matchIndex);
            timePattern = this.shortDateTimePattern.Substring(matchIndex + matchSep.Length);
            //if (timePattern.EndsWith("Z")) timePattern = timePattern.Substring(0, timePattern.Length - 1);
            return true;
        }

        public string Format(string format, object arg, IFormatProvider formatProvider = null)
        {
            if (format == null) format = shortDateTimePattern;
            if (arg is DateTime argDateTime)
            {
                if (format == null)
                {
                    if (argDateTime.Kind == DateTimeKind.Utc) format = this.shortDateTimePatternZ;
                    else if (argDateTime.Kind == DateTimeKind.Local) format = this.shortDateTimePatternLocal;
                    else if (argDateTime.Kind == DateTimeKind.Unspecified) format = this.shortDateTimePattern;
                }
                return argDateTime.ToString(format, dateFormatInfo);
            }
            if (arg is DateTimeOffset argDateTimeOffset)
            {
                return argDateTimeOffset.ToString(format, dateFormatInfo);
            }

            var formattable = arg as IFormattable;
            return formattable?.ToString(format, basedOn) ?? arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(DateTimeFormatInfo))
            {
                return GetFormatForDateTimeFormatInfo();
            }
            else if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return this.basedOn;
        }

        private DateTimeFormatInfo GetFormatForDateTimeFormatInfo()
        {
            var basedOnDtfi = (DateTimeFormatInfo) this.basedOn.GetFormat(typeof(DateTimeFormatInfo));
            var dtfi = (DateTimeFormatInfo)basedOnDtfi.Clone();
            dtfi.ShortDatePattern = this.datePattern;
            dtfi.ShortTimePattern = this.timePattern;
            return dtfi;
        }
    }
}
