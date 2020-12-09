using ProbaDotnetSDK.SharedEnums;

namespace ProbaDotnetSDK.SharedClasses
{
    public class AdvertisementEventViewModel : BaseEventDataViewModel
    {
        public string AddId { get; set; }
        public bool Skipped { get; set; }
        public AdActions Action { get; set; }
        public bool FirstTime { get; set; }
        public AdFailShowReasons FailShowReason { get; set; }
        public int Duration { get; set; }
        public string SDKName { get; set; }
        public AdTypes Type { get; set; }
        public string Placement { get; set; }
    }
}
