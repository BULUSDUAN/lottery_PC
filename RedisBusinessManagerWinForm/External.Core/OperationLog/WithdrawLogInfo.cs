using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.OperationLog
{
    [CommunicationObject]
    public class WithdrawLog_QueryInfo
    {
        public string UserDisplayName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
    }
    [CommunicationObject]
    public class WithdrawLog_QueryInfoCollection : List<WithdrawLog_QueryInfo>
    {
    }
}
