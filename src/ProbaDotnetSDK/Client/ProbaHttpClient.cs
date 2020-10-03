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
        private string BaseURL { get; }
        private HmacService HmacService { get; }
        private int APIVersion { get; }
        private CancellationTokenSource CancellationTokenSource { get; }

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
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/EndSession/{ProjectId}", endSessionViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<(bool sucess, HttpStatusCode statusCode)> SendAchievementEventAsync(AchievementEventViewModel achievementEventViewModel)
        {
            try
            {
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/Achievement/{ProjectId}", achievementEventViewModel.ToJson(), CancellationTokenSource);
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
                var (sucess, statusCode, content) = await PostJsonRequestAsync($"{BaseURL}/{APIVersion}/Events/Advertisement/{ProjectId}", advertisementEventViewModel.ToJson(), CancellationTokenSource);
                if (sucess) return (sucess, statusCode);
                //TODO: handdle errors
                return (default, statusCode);
            }
            catch (Exception)
            {
                throw;
            }
        }

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
