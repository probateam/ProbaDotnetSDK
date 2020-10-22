using ProbaDotnetSDK.SharedEnums;
using System;
using System.Collections.Generic;

namespace ProbaDotnetSDK.SharedClasses
{
    public class AchievementEventViewModel : BaseEventDataViewModel
    {
        public AchievementTypes AchievementType { get; set; }
        public string GameLevelName1 { get; set; }
        public string GameLevelName2 { get; set; }
        public string GameLevelName3 { get; set; }
        public string GameLevelName4 { get; set; }
        public List<Guid> RelatedProgressionEventIds { get; set; }
        public long NewPlayerLevel { get; set; }
        public long PrevRank { get; set; }
        public long NewRank { get; set; }
        public string LeaderBoardName { get; set; }
        public bool ArenaMode { get; set; }
        public string ArenaName { get; set; }
       
    }
}
