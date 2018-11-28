using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    /// <summary>
    /// A20140902购彩不花钱
    /// </summary>
    public class A20140902购彩不花钱
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal OrderMoney { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual bool IsGive { get; set; }
        public virtual string CurrentTime { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
