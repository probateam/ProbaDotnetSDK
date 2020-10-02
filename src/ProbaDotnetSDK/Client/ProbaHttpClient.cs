using ProbaDotnetSDK.Services;
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


        private async Task<(HttpStatusCode statusCode, string content)> PostJsonRequestAsync(string url, string message, CancellationTokenSource cts)
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

                return (response.StatusCode, content);
            }
            catch (Exception ex)
            {
                Logger.Error(ex.Message, "Erorr while sending http request {URL}", url);
                throw;
            }
        }

    }
}
