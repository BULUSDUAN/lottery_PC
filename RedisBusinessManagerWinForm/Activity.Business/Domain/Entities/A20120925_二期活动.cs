using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    public class A20120925_加奖
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual decimal BonusMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20120925_认证送彩金
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual bool IsComlateMobile { get; set; }
        public virtual bool IsComlateRealName { get; set; }
        public virtual bool IsGiveMoney { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

    public class A20120925_奖金转入送1
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual decimal TransferMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20120925_Vip充值赠送2
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

    public class A20121009_CTZQ加奖
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual SchemeType SchemeType { get; set; }
        public virtual string SchemeId { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual int HitMatchCount { get; set; }
        public virtual decimal AddMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
