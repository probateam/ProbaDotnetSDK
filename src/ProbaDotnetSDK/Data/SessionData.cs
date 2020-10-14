using ProbaDotnetSDK.SharedClasses;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Data
{
    internal class SessionData
    {
        public StartSessionViewModel StartSession { get; set; }
        public bool Ended { get; set; }
        public EndSessionViewModel EndSession { get; set; }
        public BatchEventViewModel Events { get; set; }
    }
}
