using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class BlogEntity
    {
        public DateTime CreateTime { get; set; }
        public ProfileUserInfo ProfileUserInfo { get; set; }
        public ProfileBonusLevelInfo ProfileBonusLevel { get; set; }
        public ProfileLastBonusCollection ProfileLastBonus { get; set; }
        public ProfileDataReport ProfileDataReport { get; set; }
        public BonusOrderInfoCollection BonusOrderInfo { get; set; }
        public int FollowerCount { get; set; }
        public UserCurrentOrderInfoCollection UserCurrentOrderInfo { get; set; }
        public UserBeedingListInfoCollection UserBeedingListInfo { get; set; }
    }
}
