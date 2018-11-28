using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;

namespace GameBiz.Domain.Entities
{
    public class SiteMessageBanner
    {
        public virtual int BannerId { get; set; }
        public virtual int BannerIndex { get; set; }
        public virtual string BannerTitle { get; set; }
        public virtual string ImageUrl { get; set; }
        public virtual BannerType BannerType { get; set; }
        public virtual string JumpUrl { get; set; }
        public virtual bool IsEnable { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }
}
