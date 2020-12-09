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
using System.Data.SqlTypes;
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
    public static class SDKEngine
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
        private static string UserName { get; set; }
        private static bool NewUser { get; set; }
        public static async Task InitializeAsync(string projectId, string secretKey, string userName = "", bool newUser = false)
        {
            LoggerFactory = new LoggerFactory();
            ConfigurationProvider = new ConfigurationProvider();
            HmacService = new HmacService();
            CancellationTokenSource = new CancellationTokenSource();
            ProjectId = projectId;
            SecretKet = secretKey;
            var handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => { return true; };
            mainClient = new HttpClient(handler);
            UnitOfWork = new UnitOfWork(LoggerFactory.Logger);
            var c = await ConfigurationProvider.LoadConfigurationAsync();
            if (!c) throw new Exception("Configuration not found!!!");
            ProbaHttpClient = new ProbaHttpClient(LoggerFactory.Logger, mainClient, SecretKet, ProjectId, HmacService, CancellationTokenSource, ConfigurationProvider.Configuration);
            AsyncTaskScheduler = new AsyncTaskScheduler(CancellationTokenSource, ProbaHttpClient);
            AsyncTaskScheduler.StartAsync();
            UserName = userName != "" ? userName : Guid.NewGuid().ToString();
            NewUser = newUser;
            await RegisterAsync();
        }
        public static int QueueCount => AsyncTaskScheduler?.QueueCount ?? 0;
        private static Guid UserId { get; set; }
        private static Guid SessionId { get; set; }
        private static string Class { get; set; }
        private static bool ActiveSession { get; set; }

        private static BasicData EnsureUserCreated()
        {
            var user = UnitOfWork.BasicData.FindOne(x => true);
            if ((user?.UserId ?? Guid.Empty) == default)
            {
                return default;
            }
            UserId = user.UserId;
            UserName = user.CurrentUserName;
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

        #region Account
        private static async Task<(string progress, string configurations)> RegisterAsync()
        {
            var user = EnsureUserCreated();
            if (!(user is null)) return default;
            var evenData = new BaseEventDataViewModel
            {

            };
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, evenData);
            evenData.UserName = UserName;
            evenData.NewUser = NewUser;
            try
            {
                var (sucess, statusCode, registerResponse) = await ProbaHttpClient.RegisterAsync(evenData);
                if (!sucess)
                {
                    //TODO save in databse
                }
                user = new BasicData
                {
                    UserId = registerResponse.UserId,
                    SessionCount = 0,
                    PurchesesCount = 0,
                    VirtualPurchesesCount = 0,
                    CreationTime = DateTime.UtcNow,
                    OverallPlayTime = 0,
                    CurrentUserName = registerResponse.UserName,
                };
                UnitOfWork.BasicData.Insert(user);
                UserId = user.UserId;
                return (registerResponse.Progress, registerResponse.Configurations);
            }
            catch (Exception)
            {
                //TODO save in databse
            }
            return default;
        }
        public static async Task UpdateUserInfo()
        {
            var eventData = new BaseEventDataViewModel();
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);
            eventData.UserName = UserName;
            try
            {
                var (sucess, statusCode) = await ProbaHttpClient.UpdateUserInfoAsync(eventData);
                if (!sucess)
                {
                }
            }

            catch { }
        }
        public static async Task SaveUserProgress(string progress, string configurations)
        {
            var eventData = new ProgressViewModel();
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);
            eventData.Configurations = configurations;
            eventData.Progress = progress;
            try
            {
                var (sucess, statusCode) = await ProbaHttpClient.SaveUserProgressAsync(eventData);
                if (!sucess)
                {
                }
            }

            catch { }
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
        public static async Task<(string progress, string configurations)> GetUserData()
        {
            var eventData = new ProgressViewModel();
            DeviceInfo.WriteBaseEventDataViewModel(UserId, Guid.Empty, Class, eventData);
            try
            {
                var (sucess, statusCode, registerResponse) = await ProbaHttpClient.GetUserDataAsync(eventData);
                if (!sucess)
                {
                }
                return (registerResponse.Progress, registerResponse.Configurations);
            }

            catch { }
            return default;
        }
        #endregion

        #region Trophy
        public static async Task<IList<AchievementViewModel>> GetAchievementsList()
        {
            var request = new TrophyRequest();
            try
            {
                var (sucess, statusCode, achievements) = await ProbaHttpClient.GetAchievementsListAsync(request);
                if (!sucess)
                {
                }
                return achievements;
            }

            catch { }
            return default;
        }
        public static async Task<IList<LeaderBoardViewModel>> GetLeaderBoardsList()
        {
            var request = new TrophyRequest();
            try
            {
                var (sucess, statusCode, leaderBoards) = await ProbaHttpClient.GetLeaderBoardsListAsync(request);
                if (!sucess)
                {
                }
                return leaderBoards;
            }

            catch { }
            return default;
        }
        public static async Task<IList<UserAchievementViewModel>> GetUserAchievements()
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");

            var request = new TrophyRequest
            {
                UserId = UserId
            };
            try
            {
                var (sucess, statusCode, userAchievements) = await ProbaHttpClient.GetUserAchievementsAsync(request);
                if (!sucess)
                {
                }
                return userAchievements;
            }

            catch { }
            return default;
        }
        public static async Task<IList<UserLeaderBoardViewModel>> GetUserLeaderBoardsAsync(bool self, Guid leaderBoardId)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");

            var request = new TrophyRequest
            {
                LeaderBoardId = leaderBoardId,
                UserId = self ? UserId : default
            };
            try
            {
                var (sucess, statusCode, userLeaderBoards) = await ProbaHttpClient.GetUserLeaderBoardsAsync(request);
                if (!sucess)
                {
                }
                return userLeaderBoards;
            }

            catch { }
            return default;
        }
        public static async Task NewLeaderBoardScore(Guid leaderBoardId, long Score, string userName = "")
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            var request = new TrophyRequest()
            {
                UserId = UserId,
                LeaderBoardId = leaderBoardId,
                UserName = string.IsNullOrWhiteSpace(userName) ? UserName : userName,
                ThrophyScore = Score
            };
            var job = new TaskOrder
            {
                TrophyData = request,
                Type = TaskType.SubmitNewLeaderBoardScore
            };
            AsyncTaskScheduler.Schedule(job);
        }
        public static async Task UserNewAchievement(Guid achievementId, long Score, int achievementStep)
        {
            await EnsureSessionAsync();
            if (!ActiveSession) throw new InvalidOperationException("there is no active session available. you need to start a new session or load one.");
            var request = new TrophyRequest()
            {
                UserId = UserId,
                AchievementId = achievementId,
                ThrophyScore = Score,
                AchievementStep = achievementStep
            };
            var job = new TaskOrder
            {
                TrophyData = request,
                Type = TaskType.SubmitNewAchievement
            };
            AsyncTaskScheduler.Schedule(job);
        }
        #endregion

        #region Events
        public static async Task StartSessionAsync()
        {
            if (ActiveSession) throw new InvalidOperationException("An open session exist, you need to close it first.");
            var user = EnsureUserCreated();
            if (user.HasActiveSession) throw new InvalidOperationException("An open session exist in database, you need to close it first.");
            user.SessionCount++;
            var fst = user.FirstSessionStartTime.Ticks;
            if (fst == 0) fst = DateTime.UtcNow.Ticks;
            var evenData = new StartSessionViewModel
            {
                SessionCount = user.SessionCount,
                FirstSessionTime = fst
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
                if (user.FirstSessionStartTime == default) user.FirstSessionStartTime = new DateTime(fst);
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
        public static async Task EndSessionAsync(bool soft = true, bool error = false, string errorData = "")
        {
            var user = EnsureUserCreated();
            if (!user.HasActiveSession) throw new InvalidOperationException("There is no active session in db, you need to create one first.");
            if (soft)
            {
                while (!AsyncTaskScheduler.IsQueueEmpty)
                {
                    await Task.Delay(5000);
                }
                await Task.Delay(10000);
            }
            CancellationTokenSource?.Cancel();
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
                SessionLength = user.CurrentSessionStartTime.Ticks - time,
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
        #endregion
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
            AsyncTaskScheduler?.Dispose();
            UnitOfWork?.Dispose();
            mainClient?.Dispose();
            CancellationTokenSource?.Dispose();
            LoggerFactory?.Dispose();
        }
    }
}
