using ProbaDotnetSDK.SharedEnums;
using System;

namespace ProbaDotnetSDK.SharedClasses
{
    public class ProgressionEventViewModel : BaseEventDataViewModel
    {
        public ProgressionTypes ProgressioType { get; set; }
        public int Attempts { get; set; }
        public double Score { get; set; }
        public string GameLevelName1 { get; set; }
        public string GameLevelName2 { get; set; }
        public string GameLevelName3 { get; set; }
        public string GameLevelName4 { get; set; }
        public Guid EventId { get; set; }
        public bool ArenaMode { get; set; }
        public string ArenaName { get; set; }
    }
}
