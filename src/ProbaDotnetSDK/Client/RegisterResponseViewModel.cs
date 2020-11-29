using System;
using System.Collections.Generic;
using System.Text;

namespace ProbaDotnetSDK.Client
{
    internal class RegisterResponseViewModel
    {
        public Guid UserId { get; set; }
        public string Progress { get; set; }
        public string Configurations { get; set; }
    }
}
