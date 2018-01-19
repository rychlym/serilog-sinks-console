using Serilog;
using System;
using System.Threading;
using Serilog.Sinks.SystemConsole.Themes;
using System.Globalization;

namespace ConsoleDemo
{
    public class Program
    {
        public static void Main()
        {
            var iso1DateProvider = new CustomShortDateProvider();

            var iso2DateProvider = new IsoDateTimeProvider();

            //Log.Logger = new LoggerConfiguration()
            //    .MinimumLevel.Verbose()
            //    .WriteTo.Console(theme: AnsiConsoleTheme.Literate)
            //    .CreateLogger();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console(formatProvider: iso1DateProvider, theme: AnsiConsoleTheme.Code)
                .WriteTo.Console(formatProvider: iso2DateProvider, theme: AnsiConsoleTheme.Code)
                .CreateLogger();
            try
            {
                Log.Information("Time Now (UTC) : {TimeNow}", DateTime.UtcNow);
                Log.Information("Time Now (local): {TimeNow}", DateTime.Now);

                Log.Debug("Getting started");

                Log.Information("Hello {Name} from thread {ThreadId}", Environment.GetEnvironmentVariable("USERNAME"), Thread.CurrentThread.ManagedThreadId);

                Log.Warning("No coins remain at position {@Position}", new { Lat = 25, Long = 134 });

                Fail();
            }
            catch (Exception e)
            {
                Log.Error(e, "Something went wrong");
            }

            Log.CloseAndFlush();
        }

        static void Fail()
        {
            throw new DivideByZeroException();
        }
    }
}
