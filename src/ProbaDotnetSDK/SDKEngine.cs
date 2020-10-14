﻿using ProbaDotnetSDK.Client;
using ProbaDotnetSDK.Configuration;
using ProbaDotnetSDK.Data;
using ProbaDotnetSDK.Logging;
using ProbaDotnetSDK.Scheduler;
using ProbaDotnetSDK.Services;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProbaDotnetSDK
{
    internal static class SDKEngine
    {
        private static ConfigurationProvider ConfigurationProvider { get; set; }
        private static LoggerFactory LoggerFactory { get; set; }
        private static HmacService HmacService { get; set; }
        private static CancellationTokenSource CancellationTokenSource { get; set; }
        private static HttpClient mainClient { get; set; }
        private static string SecretKet { get; set; }
        private static string ProjectId { get; set; }
        private static ProbaHttpClient ProbaHttpClient { get; set; }
        private static UnitOfWork UnitOfWork { get; set; }
        private static AsyncTaskScheduler AsyncTaskScheduler { get; set; }

        public static async Task Initialize(string projectId, string secretKey)
        {
            LoggerFactory = new LoggerFactory();
            ConfigurationProvider = new ConfigurationProvider();
            HmacService = new HmacService();
            CancellationTokenSource = new CancellationTokenSource();
            ProjectId = projectId;
            SecretKet = secretKey;
            mainClient = new HttpClient();
            UnitOfWork = new UnitOfWork(LoggerFactory.Logger);
            var c = await ConfigurationProvider.LoadConfigurationAsync();
            if (!c) throw new Exception("Configuration not found!!!");
            ProbaHttpClient = new ProbaHttpClient(LoggerFactory.Logger, mainClient, SecretKet, ProjectId, HmacService, CancellationTokenSource, ConfigurationProvider.Configuration);
            AsyncTaskScheduler = new AsyncTaskScheduler(CancellationTokenSource, ProbaHttpClient);
        }

    }
}
