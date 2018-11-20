using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.OperationLog
{
    [CommunicationObject]
    public class OrderOperationAddInfo
    {
        public string UserId { get; set; }
        public string ActionName { get; set; }
        public string OrderId { get; set; }
        public string Description { get; set; }
    }

    [CommunicationObject]
    public class OrderOperationLog_QueryInfo
    {
        public string UserDisplayName { get; set; }
        public DateTime CreateTime { get; set; }
        public string Description { get; set; }
    }
    [CommunicationObject]
    public class OrderOperationLog_QueryInfoCollection : List<OrderOperationLog_QueryInfo>
    {
    }
}
