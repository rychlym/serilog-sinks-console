using System;
using System.Globalization;

namespace ConsoleDemo
{
    public class IsoDateTimeProvider : IFormatProvider, ICustomFormatter
    {
        private readonly IFormatProvider basedOn;
        private readonly string shortDateTimePattern;
        private readonly string shortDateTimePatternZ;
        private readonly string shortDateTimePatternLocal;

        public IsoDateTimeProvider(string shortUnspecDateTimePattern = DateTimeUt.ISO8601LocalDateTime, IFormatProvider basedOn = null)
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
        }


        public string Format(string format, object arg, IFormatProvider formatProvider = null)
        {
            if (arg is DateTime argDateTime)
            {
                if (format == null)
                {
                    if (argDateTime.Kind == DateTimeKind.Utc) format = this.shortDateTimePatternZ;
                    else if (argDateTime.Kind == DateTimeKind.Local) format = this.shortDateTimePatternLocal;
                    else if (argDateTime.Kind == DateTimeKind.Unspecified) format = this.shortDateTimePattern;
                }
                return argDateTime.ToString(format, basedOn);
            }
            if (arg is DateTimeOffset argDateTimeOffset)
            {
                return argDateTimeOffset.ToString(format, basedOn);
            }

            var formattable = arg as IFormattable;
            return formattable?.ToString(format, basedOn) ?? arg.ToString();
        }

        public object GetFormat(Type formatType)
        {
            if (formatType == typeof(DateTimeFormatInfo))
            {
                return this;
            }
            else if (formatType == typeof(ICustomFormatter))
            {
                return this;
            }

            return this.basedOn;
        }
    }
}
