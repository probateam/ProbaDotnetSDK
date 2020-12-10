using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.SharedClasses
{
    public enum ELeaderBoardUnitTypes
    {
        numerical,
        Time
    }
    public class LeaderBoardViewModel
    {
        public Guid Id { get; set; }
        public string LeaderBoardName { get; set; }
        public string LeaderBoardDescription { get; set; }
        public bool Descending { get; set; }
        public ELeaderBoardUnitTypes LeaderBoardUnitType { get; set; }
        public string DefaultUnit { get; set; }
        public bool TamperProtection { get; set; }
        public long LimiteHigh { get; set; }
        public long LimitLow { get; set; }
        public int Order { get; set; }
        public string Icon { get; set; }
        public bool Deactive { get; set; }
    }
}
