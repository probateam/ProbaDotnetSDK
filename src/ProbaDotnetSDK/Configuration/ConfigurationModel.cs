using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Configuration
{
    internal class ConfigurationModel
    {
        public string BaseURL { get; set; }
        public string ServerIPAddress { get; set; }
        public string CurrentAPIVersion { get; set; }
        public List<string> CompatibleAPIVersions { get; set; }
    }
}
