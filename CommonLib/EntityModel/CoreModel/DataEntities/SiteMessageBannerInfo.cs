using EntityModel.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EntityModel.CoreModel
{
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

    [ProtoContract]
    [Serializable]
    public class SiteMessageBannerInfo_Collection
    {
        public SiteMessageBannerInfo_Collection()
        {
            ListInfo = new List<SiteMessageBannerInfo>();
        }

        [ProtoMember(1)]
        public int TotalCount { get; set; }

        [ProtoMember(2)]
        public List<SiteMessageBannerInfo> ListInfo { get; set; }
    }
}
