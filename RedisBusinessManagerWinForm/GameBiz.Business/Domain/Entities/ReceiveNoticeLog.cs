using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class ReceiveNoticeLog
    {
        public virtual long ReceiveNoticeId { get; set; }
        public virtual string AgentId { get; set; }
        public virtual int NoticeType { get; set; }
        public virtual string ReceiveUrlRoot { get; set; }
        public virtual string ReceiveDataString { get; set; }
        public virtual string Sign { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string Remark { get; set; }
        public virtual int SendTimes { get; set; }
    }

    public class ReceiveNoticeLog_Complate
    {
        public virtual long Id { get; set; }
        public virtual long ReceiveNoticeId { get; set; }
        public virtual string AgentId { get; set; }
        public virtual int NoticeType { get; set; }
        public virtual string ReceiveUrlRoot { get; set; }
        public virtual string ReceiveDataString { get; set; }
        public virtual string Sign { get; set; }
        public virtual DateTime CreateTime { get; set; }
        public virtual string Remark { get; set; }
        public virtual int SendTimes { get; set; }
        public virtual DateTime ComplateTime { get; set; }
    }
}
