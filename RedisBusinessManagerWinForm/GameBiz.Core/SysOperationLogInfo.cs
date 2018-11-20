using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class SysOperationLogInfo
    {
        public int Id { get; set; }
        public string MenuName { get; set; }
        public string OperUserId { get; set; }
        public string OperUserName { get; set; }
        public string UserId { get; set; }
        public string Description { get; set; }

        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class SysOperationLog_Collection
    {
        public SysOperationLog_Collection()
        {
            LogInfoList = new List<SysOperationLogInfo>();
        }
        public int TotalCount { get; set; }
        public List<SysOperationLogInfo> LogInfoList { get; set; }
    }
}
