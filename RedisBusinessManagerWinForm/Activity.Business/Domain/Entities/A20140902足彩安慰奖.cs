using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    /// <summary>
    /// A20140902足彩安慰奖
    /// </summary>
    public class A20140902足彩安慰奖
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual string GameCode { get; set; }
        public virtual string GameType { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual decimal OrderMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual bool IsGive { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
