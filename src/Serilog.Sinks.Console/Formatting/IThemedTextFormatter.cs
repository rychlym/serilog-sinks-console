using Serilog.Formatting;
using Serilog.Sinks.SystemConsole.Themes;

namespace Serilog.Sinks.Console.Formatting
{
    /// <summary> Formats log events in a textual representation and allowing themes.</summary>
    public interface IThemedTextFormatter : ITextFormatter
    {
        /// <summary>
        /// Gets or sets the console theme.
        /// </summary>
        ConsoleTheme Theme { get; set; }
    }
}
