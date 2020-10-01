using System;
using System.Collections.Generic;

namespace ProbaDotnetSDK.SharedClasses
{
    public class EndSessionViewModel
    {
        public Guid UserId { get; set; }
        public Guid SessionId { get; set; }

        public long ClientTs { get; set; }
        public long SessionLength { get; set; }

        public string OS { get; set; }
        public string Location { get; set; }

        public bool Error { get; set; }

        public string ErrorData { get; set; }
        public List<string> Exceptions { get; set; }
    }
}
