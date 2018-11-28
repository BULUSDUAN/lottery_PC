using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    [CommunicationObject]
    public class OneDayTogetherBettingUserInfo
    {
        public string UserId { get; set; }
        public int OrderCount { get; set; }
    }

    [CommunicationObject]
    public class OneDayTogetherBettingUserInfoCollection : List<OneDayTogetherBettingUserInfo>
    {
    }
}
