using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.SharedClasses
{
    public class UserLeaderBoard
    {
        public Guid LeaderBoardId { get; set; }
        public long Score { get; set; }
        public string UserName { get; set; }
        public bool Deactive { get; set; }
    }
}
