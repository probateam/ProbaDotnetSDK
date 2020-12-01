using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.SharedClasses
{
    public class UserAchievementViewModel
    {     
        public Guid AchievementId { get; set; }   
        public long Score { get; set; }      
        public int Step { get; set; } 
        public bool Deactive { get; set; }
    }
}
