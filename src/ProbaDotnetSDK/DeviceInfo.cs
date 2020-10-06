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


    }
}
