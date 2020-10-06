using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Logging
{
    internal class LoggerFactory : IDisposable
    {
        public LogEventLevel LogLevel { get; set; } = LogEventLevel.Information;
        public static ILogger CreateLogger(LogEventLevel logLevel, string filePath = @"./logs/")
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Is(logLevel)
                .WriteTo.File(filePath)
                .CreateLogger();
            return log;
        }

        public ILogger Logger
        {
            get
            {
                if (Log.Logger is null) Log.Logger = CreateLogger(LogLevel);
                return Log.Logger;
            }
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
