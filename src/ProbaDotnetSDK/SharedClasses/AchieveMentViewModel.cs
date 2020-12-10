using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.SharedClasses
{
    public class AchievementViewModel
    {
        public Guid Id { get; set; }
        public string AchievementName { get; set; }
        public string AchievementDescription { get; set; }
        public Dictionary<int, string> Icons { get; set; }
        public bool Incrimental { get; set; }
        public int Steps { get; set; }
        public bool Hidden { get; set; }
        public long Points { get; set; }
        public int ListOrder { get; set; }
        public bool Deactive { get; set; }
    }
}
