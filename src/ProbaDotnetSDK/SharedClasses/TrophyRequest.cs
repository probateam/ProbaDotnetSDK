using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.SharedClasses
{
    public class TrophyRequest
    {
        public string Nonce { get; set; }
        public Guid UserId { get; set; }

        public Guid LeaderBoardId { get; set; }

        public long ThrophyScore { get; set; }

        public string UserName { get; set; }
        public Guid AchievementId { get; set; }

        public int AchievementStep { get; set; }
    }
}
