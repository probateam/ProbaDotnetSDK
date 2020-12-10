using ProbaDotnetSDK.Configuration;
using ProbaDotnetSDK.Services;
using ProbaDotnetSDK.SharedClasses;
using Serilog;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProbaDotnetSDK.Client
{
    internal class ProbaHttpClient
    {
        private ILogger Logger { get; }
        private HttpClient Client { get; }
        private string SecretKet { get; }
        private string ProjectId { get; }
        private HmacService HmacService { get; }
        private CancellationTokenSource CancellationTokenSource { get; }
        private ConfigurationModel Configuration { get; }

        public ProbaHttpClient(ILogger logger, HttpClient client, string secretKet, string projectId, HmacService hmacService, CancellationTokenSource cancellationTokenSource, ConfigurationModel configuration)
        {
            Logger = logger;
            Client = client;
            SecretKet = secretKet;
            ProjectId = projectId;
            HmacService = hmacService;
            CancellationTokenSource = cancellationTokenSource;
            Configuration = configuration;
        }

        private string APIVersion => Configuration.CurrentAPIVersion;
        private string BaseURL => Configuration.BaseURL;

        #region Account
        public async Task<(bool sucess, HttpStatusCode statusCode, RegisterResponseViewModel sessionResponse)> RegisterAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/Register/{ProjectId}", baseEventDataViewModel.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    var res = content.FromJson<RegisterResponseViewModel>();
                    if (!string.IsNullOrWhiteSpace(res.Progress)) res.Progress = res.Progress.FromBase64String().ToUTF8();
                    if (!string.IsNullOrWhiteSpace(res.Configurations)) res.Configurations = res.Configurations.FromBase64String().ToUTF8();
                    return (sucess, statusCode, res);
                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode)> UpdateUserInfoAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/UpdateUserInfo/{ProjectId}", baseEventDataViewModel.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode);
                    return (sucess, statusCode);

                }
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode)> SaveUserProgressAsync(ProgressViewModel progress)
        {
            try
            {
                progress.Progress = progress.Progress.FromUTF8().ToBase64String();
                progress.Configurations = progress.Configurations.FromUTF8().ToBase64String();
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/UpdateUserProgress/{ProjectId}", progress.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode, RegisterResponseViewModel sessionResponse)> GetUserDataAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/GetUserData/{ProjectId}", baseEventDataViewModel.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    var res = content.FromJson<RegisterResponseViewModel>();
                    if (string.IsNullOrWhiteSpace(res.Progress)) res.Progress = res.Progress.FromBase64String().ToUTF8();
                    if (string.IsNullOrWhiteSpace(res.Configurations)) res.Configurations = res.Configurations.FromBase64String().ToUTF8();
                    return (sucess, statusCode, res);
                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode, IList<RemoteConfigurationsViewModel> remoteConfigurations)> GetRemoteConfigurationsAsync(BaseEventDataViewModel baseEventDataViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Account/RemoteConfigurations/{ProjectId}", baseEventDataViewModel.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode, content.FromJson<IList<RemoteConfigurationsViewModel>>());
                    return (sucess, statusCode, default);

                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Trophy
        public async Task<(bool sucess, HttpStatusCode statusCode, IList<AchievementViewModel> achievements)> GetAchievementsListAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetAchievementsList/{ProjectId}", trophyRequest.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode, content.FromJson<IList<AchievementViewModel>>());
                    return (sucess, statusCode, default);

                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode, IList<LeaderBoardViewModel> leaderBoards)> GetLeaderBoardsListAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetLeaderboardsList/{ProjectId}", trophyRequest.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode, content.FromJson<IList<LeaderBoardViewModel>>());
                    return (sucess, statusCode, default);

                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode, IList<UserAchievementViewModel> userAchievements)> GetUserAchievementsAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetUserAchievements/{ProjectId}", trophyRequest.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode, content.FromJson<IList<UserAchievementViewModel>>());
                    return (sucess, statusCode, default);

                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode, IList<UserLeaderBoardViewModel> userLeaderBoards)> GetUserLeaderBoardsAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/GetUserLeaderBoards/{ProjectId}", trophyRequest.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode, content.FromJson<IList<UserLeaderBoardViewModel>>());
                    return (sucess, statusCode, default);

                }
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode)> AddNewLeaderBoardScoreAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/AddNewLeaderBoardScore/{ProjectId}", trophyRequest.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode);
                    return (sucess, statusCode);

                }
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async Task<(bool sucess, HttpStatusCode statusCode)> AddUserNewAchievementAsync(TrophyRequest trophyRequest)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Trophies/UserNewAchievement/{ProjectId}", trophyRequest.ToJson(), CancellationTokenSource);
                if (sucess)
                {
                    if (statusCode == HttpStatusCode.OK)
                        return (sucess, statusCode);
                    return (sucess, statusCode);

                }
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion

        #region Events
        public async Task<(bool sucess, HttpStatusCode statusCode, CreateSessionResponseModel sessionResponse)> StartSessionAsync(StartSessionViewModel startSessionViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/SessionStart/{ProjectId}", startSessionViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode, content.FromJson<CreateSessionResponseModel>());
                //TODO: handdle errors
                return (default, statusCode, default);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> EndSessionAsync(EndSessionViewModel endSessionViewModel)
        {
            var endSessionToken = new CancellationTokenSource();
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/SessionEnd/{ProjectId}", endSessionViewModel.ToJson(), endSessionToken);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                endSessionToken.Dispose();
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendAchievementEventAsync(AchievementEventViewModel achievementEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/AchievementEvent/{ProjectId}", achievementEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendAdvertisementEventAsync(AdvertisementEventViewModel advertisementEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/AdvertisementEvent/{ProjectId}", advertisementEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendBusinessEventAsync(BusinessEventViewModel businessEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/BusinessEvent/{ProjectId}", businessEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendContentViewEventAsync(ContentViewEventViewModel contentViewEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/ContentViewEvent/{ProjectId}", contentViewEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendDesignEventAsync(DesignEventViewModel designEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/DesignEvent/{ProjectId}", designEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendProgressionEventAsync(ProgressionEventViewModel progressionEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/ProgressionEvent/{ProjectId}", progressionEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendSocialEventAsync(SocialEventViewModel socialEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/SocialEvent/{ProjectId}", socialEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendTapEventAsync(TapEventViewModel tapEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/TapEvent/{ProjectId}", tapEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }
        #endregion
        private async Task<(bool sucess, HttpStatusCode statusCode, string content)> PostJsonRequestAsync(string url, string message, CancellationTokenSource cts)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new StringContent(message, Encoding.UTF8, "application/json")
            };
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("hmac", HmacService.GenerateHmacSignature(SecretKet, message));
            try
            {
                var response = await Client.SendAsync(request, cts.Token).ConfigureAwait(false);
                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                if (!response.IsSuccessStatusCode)
                {
                    Logger.Warning("Unsuccessfull http request response for request {URL} with {statusCode} statuscode and response: {serverResponse}", url, response.StatusCode, content);
                }

                return (response.IsSuccessStatusCode, response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Erorr while sending http request {URL}", url);
                throw;
            }
        }
    }
}
