using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace External.Domain.Entities.AppendBonusConfig
{
    public class AppendBonusConfig
    {
        public virtual Int64 ConfigID { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual decimal AppendBonusMoney { get; set; }
        public virtual decimal AppendRatio { get; set; }
        public virtual int StartMultiple { get; set; }
        public virtual int ColorBeanNumber { get; set; }
        public virtual decimal ColorBeanRatio { get; set; }
        public virtual int ColorBeanStartMultiple { get; set; }
        public virtual DateTime ModifyTime { get; set; }

        public virtual int StartIssueNumber { get; set; }
        public virtual int EndIssueNumber { get; set; }
        public virtual int BonusMoneyStartMultiple { get; set; }
    }
}
