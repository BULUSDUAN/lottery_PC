using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Core
{
    /// <summary>
    /// Redis票对象
    /// </summary>
    public class RedisTicketInfo
    {
        public string SchemeId { get; set; }
        public string TicketId { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string IssuseNumber { get; set; }
        public string PlayType { get; set; }
        public string MatchIdList { get; set; }
        public int BetUnits { get; set; }
        public int Amount { get; set; }
        public decimal BetMoney { get; set; }
        public string BetContent { get; set; }
        public string LocOdds { get; set; }
        public bool IsAppend { get; set; }
        public BonusStatus  BonusStatus { get; set; }
        public decimal PreBonusMoney { get; set; }
        public decimal AfterBonusMoney { get; set; }
    }

    /// <summary>
    /// Redis订单对象
    /// </summary>
    public class RedisOrderInfo
    {
        public string SchemeId { get; set; }
        public string KeyLine { get; set; }
        public SchemeType SchemeType { get; set; }
        public bool StopAfterBonus { get; set; }

        //public string GameCode { get; set; }
        //public string GameType { get; set; }
        //public string IssuseNumber { get; set; }
        //public int Amount { get; set; }
        //public decimal TotalMoney { get; set; }
        //public decimal RedBagMoney { get; set; }
        public List<RedisTicketInfo> TicketList { get; set; }

        public RedisOrderInfo()
        {
            TicketList = new List<RedisTicketInfo>();
        }
    }
}
