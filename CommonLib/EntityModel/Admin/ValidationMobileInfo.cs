using System;
using System.Collections.Generic;

namespace EntityModel
{
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

    [Serializable]
    public class ValidationMobileInfoCollection : List<ValidationMobileInfo>
    {
    }
}
