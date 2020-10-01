using ProbaDotnetSDK.SharedEnums;

namespace ProbaDotnetSDK.SharedClasses
{
    public class TapEventViewModel : BaseEventDataViewModel
    {
        public string BtnName { get; set; }
        public TapTypes TapType { get; set; }
    }
}
