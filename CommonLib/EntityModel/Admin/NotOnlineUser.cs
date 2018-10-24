using System;
using System.Collections.Generic;

namespace EntityModel
{

    [Serializable]
    public class NotOnlineUser
    {
        public string UserId { get; set; }
        public string Mobile { get; set; }
        public string RealName { get; set; }
        public decimal TotalFillMoney { get; set; }
        public decimal TotalBonusMoney { get; set; }
        public decimal TotalWithdraw { get; set; }
        public decimal FillMoneyBalance { get; set; }
        public decimal Earnings { get; set; }
        public string DisplayName { get; set; }
        public decimal BonusBalance { get; set; }
    }

    [Serializable]
    public class NotOnlineUserCollection
    {
        public NotOnlineUserCollection()
        {
            UserList = new List<NotOnlineUser>();
        }
        public int TotalCount { get; set; }

        public List<NotOnlineUser> UserList { get; set; }
    }

    [Serializable]
    public class NotOnlineUserGiftItem
    {
        public bool IsProfit { get; set; }
        public decimal ProfitMoney { get; set; }
        public decimal GiveBonus { get; set; }
        public decimal GiveRedPackets { get; set; }
    }
    [Serializable]
    public class NotOnlineUserGiftConfig
    {
        public NotOnlineUserGiftConfig()
        {
            GiftList = new List<NotOnlineUserGiftItem>();
        }
        public List<NotOnlineUserGiftItem> GiftList { get; set; }
        public bool IsEnable { get; set; }

    }
}
