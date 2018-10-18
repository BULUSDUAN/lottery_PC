using System;
using System.Collections.Generic;

namespace EntityModel
{
    public class ActivityPrizeConfigInfo
    {
        public  int ActivityId { get; set; }
        public  string ActivityTitle { get; set; }
        public  string ActivityContent { get; set; }
        public  bool IsEnabled { get; set; }
        public  string CreatorId { get; set; }
        public  DateTime CreateTime { get; set; }
    }
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
