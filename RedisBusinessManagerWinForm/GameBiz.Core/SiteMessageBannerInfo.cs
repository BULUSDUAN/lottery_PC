﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class SiteMessageBannerInfo
    {
        public int BannerId { get; set; }
        public int BannerIndex { get; set; }
        public string BannerTitle { get; set; }
        public string ImageUrl { get; set; }
        public BannerType BannerType { get; set; }
        public string JumpUrl { get; set; }
        public bool IsEnable { get; set; }
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class SiteMessageBannerInfo_Collection
    {
        public SiteMessageBannerInfo_Collection()
        {
            ListInfo = new List<SiteMessageBannerInfo>();
        }
        public int TotalCount { get; set; }
        public List<SiteMessageBannerInfo> ListInfo { get; set; }
    }
}
