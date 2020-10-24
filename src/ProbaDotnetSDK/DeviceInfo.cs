using Mapster;
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

        public static void WriteBaseEventDataViewModel<T>(Guid userId, Guid sessionId, string Class, T eventData) where T : BaseEventDataViewModel
        {
            eventData.SDKVersion = SDKVersion;
            eventData.OS = OSInfo;
            eventData.Battery = 100;
            eventData.Charging = false;
            eventData.ClientTs = DateTime.UtcNow.Ticks;
            eventData.ConnectionType = ConnectionType;
            eventData.Device = DeviceName;
            eventData.Engine = EngineName;
            eventData.Manufacturer = Manufacturer;
            eventData.Nonce = Guid.NewGuid().ToString();
            eventData.Platform = Platform;
            eventData.Version = ApplicationVersion;
            eventData.Build = ApplicationBuild;
            eventData.Google_AID = string.Empty;
            eventData.IOS_IDFA = string.Empty;
            eventData.LogOnGooglePlay = default;
            eventData.ProbaGameCenter = default;
            eventData.UserId = userId;
            eventData.SessionHanddle = sessionId;
            eventData.Class = Class;
        }
    }
}
