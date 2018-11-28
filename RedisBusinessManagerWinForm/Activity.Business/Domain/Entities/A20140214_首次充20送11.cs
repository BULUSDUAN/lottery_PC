using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Activity.Domain.Entities
{
    public class A20140214_首次充20送11
    {
        public virtual int Id { get; set; }
        public virtual string UserId { get; set; }
        public virtual string OrderId { get; set; }
        public virtual decimal FillMoney { get; set; }
        public virtual decimal GiveMoney { get; set; }
        public virtual string Content { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
