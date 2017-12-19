using Serilog.Events;
using Serilog.Sinks.SystemConsole.Output;
using Serilog.Sinks.SystemConsole.Themes;
using System;
using System.IO;

namespace Serilog.Sinks.Console.Formatting
{
    /// <summary>
    /// The Event Level based console formatter.
    /// </summary>
    public class LevelBasedFormatter : IThemedTextFormatter
    {
        private readonly OutputTemplateRenderer simpleModeFormatter;
        private readonly OutputTemplateRenderer formatter;
        private readonly LogEventLevel simpleModeLevel;

        private ConsoleTheme _theme;

        /// <summary>
        /// Gets or sets a console theme.
        /// </summary>
        public ConsoleTheme Theme
        {
            get => _theme;
            set
            {
                if (_theme != value)
                {
                    _theme = value;
                    ApplyTheme();
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="LevelBasedFormatter"/> class.
        /// </summary>
        /// <param name="formatProvider">The format provider.</param>
        /// <param name="outputTemplate">The classic output template.</param>
        /// <param name="simpleOutputTemplate">The simple variant of output template.</param>
        /// <param name="simpleModeLevel">The log event using the simple variant of the output template.</param>
        public LevelBasedFormatter(IFormatProvider formatProvider, string outputTemplate, string simpleOutputTemplate = "{Message}{NewLine}{Exception}", LogEventLevel simpleModeLevel = LogEventLevel.Information)
        {
            this.formatter = new OutputTemplateRenderer(outputTemplate, formatProvider);
            this.simpleModeFormatter = new OutputTemplateRenderer(simpleOutputTemplate, formatProvider);
            this.simpleModeLevel = simpleModeLevel;
        }

        /// <summary>
        /// Formats a log event to an output.
        /// </summary>
        /// <param name="logEvent">The log event.</param>
        /// <param name="output">The output text writer.</param>
        public void Format(LogEvent logEvent, TextWriter output)
        {
            if (logEvent.Level == this.simpleModeLevel)
            {
                this.simpleModeFormatter.Format(logEvent, output);
            }
            else
            {
                this.formatter.Format(logEvent, output);
            }
        }


        private void ApplyTheme()
        {
            this.simpleModeFormatter.Theme = _theme;
            this.formatter.Theme = _theme;
        }
    }
}
