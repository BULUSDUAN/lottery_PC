using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.OperationLog
{
    [CommunicationObject]
    public class SMSLogAddInfo
    {
        public string Category { get; set; }
        public string MoblieNumber { get; set; }
        public string Content { get; set; }
    }
}
