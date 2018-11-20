using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace FacaiActivity.Domain.Entities
{
    public class A20120101_通用加奖_AppendBonus
    {
        public virtual Int64 BonusId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual string UserId { get; set; }
        public virtual decimal IssueMoney { get; set; }
        public virtual decimal BonusMoney { get; set; }
        public virtual decimal AppendBonusMoney { get; set; }
        public virtual decimal AppendRatio { get; set; }
        public virtual decimal TotalBonusMoney { get; set; }
        public virtual int ColorBeanNumber { get; set; }
        public virtual string Desc { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
