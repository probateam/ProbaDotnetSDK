using ProbaDotnetSDK.SharedEnums;

namespace ProbaDotnetSDK.SharedClasses
{
    public class BusinessEventViewModel : BaseEventDataViewModel
    {
        public BusinessTypes BusinessType { get; set; }
        public double Value { get; set; }
        public string Currency { get; set; }
        public string ItemName { get; set; }
        public int TransactionCount { get; set; }
        public string CartName { get; set; }
        public string ExtraDetails { get; set; }
        public PaymentTypes PaymentType { get; set; }
        public bool SpecialEvent { get; set; }
        public string SpecialEventName { get; set; }
        public double Amount { get; set; }
        public bool VirtualCurrency { get; set; }
    }
}
