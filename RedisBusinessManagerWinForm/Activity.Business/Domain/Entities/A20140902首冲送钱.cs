using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    /// <summary>
    /// A20140902首冲送钱
    /// </summary>
    public class A20140902首冲送钱
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual decimal CurrentGiveMoney { get; set; }
        public virtual decimal NextGiveMoney { get; set; }
        public virtual bool IsGiveComplate { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
