using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Configuration
{
    internal class ConfigurationModel
    {
        public string BaseURL { get; set; }
        public string ServerIPAddress { get; set; }
        public int CurrentAPIVersion { get; set; }
        public List<int> CompatibleAPIVesions { get; set; }
    }
}
