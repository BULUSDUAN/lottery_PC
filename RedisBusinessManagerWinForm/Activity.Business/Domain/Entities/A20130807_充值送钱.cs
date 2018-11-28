using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace Activity.Domain.Entities
{
    public class A20130807_充值送钱
    {
        public virtual long Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual DateTime UpdateTime { get; set; }
    }

}
