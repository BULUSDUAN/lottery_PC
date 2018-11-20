using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 
    /// </summary>
    public class ActivityPrizeConfig
    {
        public virtual int ActivityId { get; set; }
        public virtual string ActivityTitle { get; set; }
        public virtual string ActivityContent { get; set; }
        public virtual bool IsEnabled { get; set; }
        public virtual string CreatorId { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
