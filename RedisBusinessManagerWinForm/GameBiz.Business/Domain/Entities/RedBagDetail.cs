using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 红包明细
    /// </summary>
    public class RedBagDetail
    {
        public virtual int Id { get; set; }
        public virtual RedBagCategory RedBagCategory { get; set; }
        public virtual string OrderId { get; set; }
        public virtual string UserId { get; set; }
        public virtual decimal RedBagMoney { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
