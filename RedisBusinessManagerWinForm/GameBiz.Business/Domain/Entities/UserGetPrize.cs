using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    public class UserGetPrize
    {
        public virtual int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string PrizeType { get; set; }
        public virtual int PayInegral { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual decimal OrderMoney { get; set; }
        public virtual string Summary { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
