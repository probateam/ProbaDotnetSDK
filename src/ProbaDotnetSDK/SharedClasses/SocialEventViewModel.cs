using ProbaDotnetSDK.SharedEnums;

namespace ProbaDotnetSDK.SharedClasses
{
    public class SocialEventViewModel : BaseEventDataViewModel
    {
        public string SocialMediaName { get; set; }

        public SocialEvenTypes SocialEvenType { get; set; }
        public int Value { get; set; }
    }
}
