using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using System.Collections;

namespace External.Core.Login
{
    [CommunicationObject]
    [Serializable]
    public class ValidationMobileInfo
    {
        public long Id { get; set; }
        public string Mobile { get; set; }
        public string Category { get; set; }
        public string ValidateCode { get; set; }
        public int SendTimes { get; set; }
        public int RetryTimes { get; set; }
        public DateTime UpdateTime { get; set; }
    }

    [CommunicationObject]
    [Serializable]
    public class ValidationMobileInfoCollection : List<ValidationMobileInfo>
    {
    }
}
