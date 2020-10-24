using Mapster;
using ProbaDotnetSDK.Client;
using ProbaDotnetSDK.Configuration;
using ProbaDotnetSDK.Data;
using ProbaDotnetSDK.Logging;
using ProbaDotnetSDK.Scheduler;
using ProbaDotnetSDK.Services;
using ProbaDotnetSDK.SharedClasses;
using ProbaDotnetSDK.SharedEnums;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Security;
using System.Security.Cryptography.X509Certificates;
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

        public static async Task InitializeAsync(string projectId, string secretKey)
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
            AsyncTaskScheduler.StartAsync();
        }
        private static Guid UserId { get; set; }
        private static Guid SessionId { get; set; }
        private static string Class { get; set; }
        private static bool ActiveSession { get; set; }
        private static BasicData EnsureUserCreated()
        {
            var user = UnitOfWork.BasicData.Query().First();
            if ((user?.UserId ?? Guid.Empty) == default)
            {
                user = new BasicData
                {
                    UserId = Guid.NewGuid(),
                    SessionCount = 0,
                    PurchesesCount = 0,
                    VirtualPurchesesCount = 0,
                    CreationTime = DateTime.UtcNow,
                    OverallPlayTime = 0
                };
                UnitOfWork.BasicData.Insert(user);
            }
            UserId = user.UserId;
            return user;
        }

        private static async Task EnsureSessionAsync()
        {
            if (ActiveSession) return;
            var user = EnsureUserCreated();
            if (user.HasActiveSession)
            {
                ActiveSession = true;
                UserId = user.UserId;
                SessionId = user.CurrentSessionId;
                return;
            }
            await StartSessionAsync();
        }
        public static async Task StartSessionAsync()
        {
            if (ActiveSession) throw new InvalidOperationException("An open session exist, you need to close it first.");
            var user = EnsureUserCreated();
            if (user.HasActiveSession) throw new InvalidOperationException("An open session exist in database, you need to close it first.");
            user.SessionCount++;
            var evenData = new StartSessionViewModel
            {
                SessionCount = user.SessionCount
            };
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, evenData);
            try
            {
                var (sucess, statusCode, sessionResponse) = await ProbaHttpClient.StartSessionAsync(evenData);
                if (!sucess)
                {
                    //TODO save in databse
                }
                SessionId = sessionResponse.SessionId;
                ActiveSession = true;
                user.CurrentSessionId = SessionId;
                if (user.FirstSessionStartTime == default) user.FirstSessionStartTime = DateTime.UtcNow;
                user.CurrentSessionStartTime = DateTime.UtcNow;
                user.HasActiveSession = true;
                user.CurrentSessionLocation = sessionResponse.Location;
                UnitOfWork.BasicData.Update(user);
            }
            catch (Exception)
            {
                //TODO save in databse
            }
        }
        public static async Task EndSessionAsync(bool error = false, string errorData = "")
        {
            var user = EnsureUserCreated();
            if (!user.HasActiveSession) throw new InvalidOperationException("There is no active session in db, you need to create one first.");
            var (remainingTasks, exceptions, responses) = AsyncTaskScheduler.Finalize();
            //TODO send remaining tasks
            var time = DateTime.UtcNow.Ticks;
            var endSessionInfo = new EndSessionViewModel
            {
                Battery = 100,
                ClientTs = time,
                Error = error,
                ErrorData = errorData,
                Location = user.CurrentSessionLocation,
                OS = DeviceInfo.OSInfo,
                SessionId = user.CurrentSessionId,
                UserId = user.UserId,
                SessionLength = time - user.CurrentSessionStartTime.Ticks,
                Exceptions = exceptions.Select(x => x.ToString()).ToList()
            };
            try
            {
                var (sucess, statusCode) = await ProbaHttpClient.EndSessionAsync(endSessionInfo);
                if (!sucess)
                {
                    //TODO save in databse
                }
                SessionId = Guid.Empty;
                ActiveSession = false;
                user.CurrentSessionId = Guid.Empty;
                user.CurrentSessionStartTime = DateTime.MinValue;
                user.HasActiveSession = false;
                user.CurrentSessionLocation = string.Empty;
                user.OverallPlayTime += time;
                UnitOfWork.BasicData.Update(user);
            }
            catch (Exception)
            {
                //TODO save in databse
            }
        }
        public static async Task<IList<RemoteConfigurationsViewModel>> GetRemoteConfigAsync()
        {
            var eventData = new BaseEventDataViewModel();
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);
            try
            {
                var (sucess, statusCode, remoteConfigurations) = await ProbaHttpClient.GetRemoteConfigurationsAsync(eventData);
                if (!sucess)
                {
                }
                var rm = new RemotoConfigurationModel
                {
                    RemotoConfigurations = remoteConfigurations
                };
                UnitOfWork.RemoteConfigurations.Insert(rm);
                return (remoteConfigurations);
            }

            catch { }
            return default;
        }
        public static async Task SendAchievementEventAsync(AchievementEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                Achievement = eventData,
                Type = TaskType.SendAchievementEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendAdvertisementEventAsync(AdvertisementEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                Advertisement = eventData,
                Type = TaskType.SendAdvertisementEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendBusinessEventAsync(BusinessEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                Businesse = eventData,
                Type = TaskType.SendBusinessEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendContentViewEventAsync(ContentViewEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                ContentView = eventData,
                Type = TaskType.SendContentViewEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendDesignEventAsync(DesignEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                DesignEvent = eventData,
                Type = TaskType.SendDesignEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendProgressionEventAsync(ProgressionEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                Progression = eventData,
                Type = TaskType.SendProgressionEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendSocialEventAsync(SocialEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                Social = eventData,
                Type = TaskType.SendSocialEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task SendTapEventAsync(TapEventViewModel eventData)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);

            var job = new TaskOrder
            {
                Tap = eventData,
                Type = TaskType.SendTapEvent
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task<bool> IsConnectedToInternet(bool sendPing = true)
        {
            if (NetworkInterface.GetIsNetworkAvailable())
            {
                if (!sendPing) return true;
                using (var ping = new Ping())
                {
                    try
                    {
                        return (await ping.SendPingAsync(ConfigurationProvider.Configuration.BaseURL)).Status == IPStatus.Success;
                    }
                    catch { }
                }
            }
            return false;
        }

        public static void Dispose()
        {
            CancellationTokenSource?.Cancel();
            AsyncTaskScheduler?.Dispose();
            UnitOfWork?.Dispose();
            mainClient?.Dispose();
            CancellationTokenSource?.Dispose();
            LoggerFactory?.Dispose();
        }
    }
}
