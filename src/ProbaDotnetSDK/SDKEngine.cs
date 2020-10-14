using ProbaDotnetSDK.Client;
using ProbaDotnetSDK.Configuration;
using ProbaDotnetSDK.Data;
using ProbaDotnetSDK.Scheduler;
using ProbaDotnetSDK.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ProbaDotnetSDK
{
    internal static class SDKEngine
    {
        private static ConfigurationModel Configurations { get; set; }
        private static ILogger Logger { get; set; }
        private static ProbaHttpClient ProbaHttpClient { get; set; }
        private static HmacService HmacService { get; set; }
        private static CancellationTokenSource CancellationTokenSource { get; set; }
        private static string SecretKet { get; set; }
        private static string ProjectId { get; set; }
        private static UnitOfWork UnitOfWork { get; set; }
        private static AsyncTaskScheduler AsyncTaskScheduler { get; set; }
    }
}
