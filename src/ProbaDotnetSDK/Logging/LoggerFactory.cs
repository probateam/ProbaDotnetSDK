using Serilog;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Logging
{
    internal class LoggerFactory : IDisposable
    {
        public static ILogger CreateLogger(string filePath = @"./logs/")
        {
            var log = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.File(filePath)
                .CreateLogger();
            return log;
        }

        public ILogger Logger
        {
            get
            {
                if (Log.Logger is null) Log.Logger = CreateLogger();
                return Log.Logger;
            }
        }

        public void Dispose()
        {
            Log.CloseAndFlush();
        }
    }
}
