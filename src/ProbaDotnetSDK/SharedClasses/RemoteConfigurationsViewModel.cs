using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.SharedClasses
{
    public class RemoteConfigurationsViewModel
    {
        public string ConfigKey { get; set; }

        public string ConfigDescription { get; set; }

        public string Value { get; set; }

        public DateTime StartTime { get; set; }

        public DateTime EndTime { get; set; }

        #region OS Filter
        public string OS { get; set; }
        public bool OS_Exclude { get; set; }
        #endregion

        #region Country Filter
        public string Country { get; set; }
        public bool Country_Exclude { get; set; }
        #endregion

        #region Build Filter
        public string Build { get; set; }
        public bool Build_Exclude { get; set; }
        #endregion

        #region Class Filter
        public string TimeClass { get; set; }
        public bool TimeClass_Exclude { get; set; }
        #endregion
        public int Priority { get; set; }

        public bool Active { get; set; }
    }
}
