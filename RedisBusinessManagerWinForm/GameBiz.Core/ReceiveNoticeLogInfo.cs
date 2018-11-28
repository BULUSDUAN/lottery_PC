using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class ReceiveNoticeLogInfo
    {
        public long ReceiveNoticeId { get; set; }
        public string AgentId { get; set; }
        public int NoticeType { get; set; }
        public string ReceiveUrlRoot { get; set; }
        public string ReceiveDataString { get; set; }
        public string Sign { get; set; }
        public DateTime CreateTime { get; set; }
        public string Remark { get; set; }
        public int SendTimes { get; set; }
        public DateTime ComplateTime { get; set; }
    }
    [CommunicationObject]
    public class ReceiveNoticeLogInfo_Collection
    {
        public ReceiveNoticeLogInfo_Collection()
        {
            ListInfo = new List<ReceiveNoticeLogInfo>();
        }
        public int TotalCount { get; set; }
        public List<ReceiveNoticeLogInfo> ListInfo { get; set; }
    }
}
