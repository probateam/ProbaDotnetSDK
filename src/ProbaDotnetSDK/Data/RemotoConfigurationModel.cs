using ProbaDotnetSDK.SharedClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Data
{
    internal class RemotoConfigurationModel
    {
        public int Id { get; set; }
        public IList<RemoteConfigurationsViewModel> RemotoConfigurations { get; set; }
    }
}
