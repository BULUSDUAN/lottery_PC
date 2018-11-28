using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class ActivityPrizeConfigInfo
    {
        public  int ActivityId { get; set; }
        public  string ActivityTitle { get; set; }
        public  string ActivityContent { get; set; }
        public  bool IsEnabled { get; set; }
        public  string CreatorId { get; set; }
        public  DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ActivityPrizeConfigInfo_Collection
    {
        public ActivityPrizeConfigInfo_Collection()
        {
            ActConfigList = new List<ActivityPrizeConfigInfo>();
        }
        public int TotalCount { get; set; }
        public List<ActivityPrizeConfigInfo> ActConfigList { get; set; }
    }
}
