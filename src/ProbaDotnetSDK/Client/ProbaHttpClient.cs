using Serilog;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace ProbaDotnetSDK.Client
{
    internal class ProbaHttpClient
    {
        private ILogger Logger { get; }
        private HttpClient Client { get; }

        public ProbaHttpClient(ILogger logger, HttpClient client)
        {
            Logger = logger;
            Client = client;
        }



    }
}
