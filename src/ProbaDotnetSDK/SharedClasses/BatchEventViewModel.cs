using System.Collections.Generic;

namespace ProbaDotnetSDK.SharedClasses
{
    public class BatchEventViewModel
    {
        public List<AchievementEventViewModel> Achievements { get; set; }
        public List<AdvertisementEventViewModel> Advertisements { get; set; }
        public List<BusinessEventViewModel> Businesses { get; set; }
        public List<ContentViewEventViewModel> ContentViews { get; set; }
        public DesignEventViewModel DesignEvent { get; set; }
        public List<ProgressionEventViewModel> Progressions { get; set; }
        public List<SocialEventViewModel> Socials { get; set; }
        public List<TapEventViewModel> Taps { get; set; }

    }
}
