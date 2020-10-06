using ProbaDotnetSDK.SharedClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Scheduler
{
    internal enum TaskType
    {
        SendAchievementEvent,
        SendAdvertisementEvent,
        SendBusinessEvent,
        SendContentViewEvent,
        SendDesignEvent,
        SendProgressionEvent,
        SendSocialEvent,
        SendTapEvent,
        Wait
    }
    internal class TaskOrder
    {
        public TaskType Type { get; set; }
        public AchievementEventViewModel Achievement { get; set; }
        public AdvertisementEventViewModel Advertisement { get; set; }
        public BusinessEventViewModel Businesse { get; set; }
        public ContentViewEventViewModel ContentView { get; set; }
        public DesignEventViewModel DesignEvent { get; set; }
        public ProgressionEventViewModel Progression { get; set; }
        public SocialEventViewModel Social { get; set; }
        public TapEventViewModel Tap { get; set; }
    }
}
