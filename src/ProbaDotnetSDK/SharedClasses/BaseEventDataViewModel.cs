using ProbaDotnetSDK.SharedEnums;
using System;

namespace ProbaDotnetSDK.SharedClasses
{
    public class BaseEventDataViewModel
    {
        public Guid SessionHanddle { get; set; }
        public Guid UserId { get; set; }
        public string Version { get; set; }
        public string OS { get; set; }
        public string Build { get; set; }
        public string Class { get; set; }
        public string Nonce { get; set; }
        public string Device { get; set; }
        public string SDKVersion { get; set; }
        public string Manufacturer { get; set; }
        public EPlatforms Platform { get; set; }
        public bool ProbaGameCenter { get; set; }
        public bool LogOnGooglePlay { get; set; }
        public string Engine { get; set; }
        public string ConnectionType { get; set; }
        public string IOS_IDFA { get; set; }
        public string Google_AID { get; set; }
        public string Proba_GCID { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }

        public long ClientTs { get; set; }
        public double Battery { get; set; }

        public bool Charging { get; set; }
        public string UserName { get; set; }

        public bool NewUser { get; set; }
        public string UniqueId { get; set; }
    }
}
