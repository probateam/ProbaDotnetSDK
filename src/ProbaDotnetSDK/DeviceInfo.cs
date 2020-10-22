using ProbaDotnetSDK.SharedClasses;
using ProbaDotnetSDK.SharedEnums;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace ProbaDotnetSDK
{
    public static class DeviceInfo
    {
        public static string SDKVersion => Assembly.GetEntryAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion;
        public static string OSInfo => Environment.OSVersion.ToString();
        public static string DeviceName => "Surface book";
        public static string ApplicationBuild { get; set; }
        public static string ApplicationVersion { get; set; }
        public static string Manufacturer => "Microsoft";
        public static EPlatforms Platform => EPlatforms.None;
        public static string EngineName => "None";
        public static string ConnectionType => "WIFI";

        public static T GetBaseEventDataViewModel<T>(Guid userId, Guid sessionId, string Class) where T : BaseEventDataViewModel => (T)new BaseEventDataViewModel
        {
            SDKVersion = SDKVersion,
            OS = OSInfo,
            Battery = 100,
            Charging = false,
            ClientTs = DateTime.UtcNow.Ticks,
            ConnectionType = ConnectionType,
            Device = DeviceName,
            Engine = EngineName,
            Manufacturer = Manufacturer,
            Nonce = Guid.NewGuid().ToString(),
            Platform = Platform,
            Version = ApplicationVersion,
            Build = ApplicationBuild,
            Google_AID = string.Empty,
            IOS_IDFA = string.Empty,
            LogOnGooglePlay = default,
            ProbaGameCenter = default,
            UserId = userId,
            SessionHanddle = sessionId,
            Class = Class
        };
    }
}
