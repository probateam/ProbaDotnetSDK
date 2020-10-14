using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Data
{
    internal class BasicData
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }
        public long SessionCount { get; set; }
        public long PurchesesCount { get; set; }
        public long VirtualPurchesesCount { get; set; }
        public DateTime FirstSessionStartTime { get; set; }
        public DateTime CurrentSessionStartTime { get; set; }
        public long OverallPlayTime { get; set; }

    }
}
