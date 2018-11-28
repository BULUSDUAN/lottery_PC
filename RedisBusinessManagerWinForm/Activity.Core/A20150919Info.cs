using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace Activity.Core
{
    /// <summary>
    /// 充值赠送红包配置
    /// </summary>
    [CommunicationObject]
    public class FillMoneyGiveRedBagConfigInfo
    {
        public int Id { get; set; }
        public decimal FillMoney { get; set; }
        public decimal GiveMoney { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class FillMoneyGiveRedBagConfigInfoCollection : List<FillMoneyGiveRedBagConfigInfo>
    {

    }

    /// <summary>
    /// 彩种加奖配置
    /// </summary>
    [CommunicationObject]
    public class AddBonusMoneyConfigInfo
    {
        public int Id { get; set; }
        public int OrderIndex { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string PlayType { get; set; }
        public decimal AddBonusMoneyPercent { get; set; }
        public decimal MaxAddBonusMoney { get; set; }
        public string AddMoneyWay { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class AddBonusMoneyConfigInfoCollection : List<AddBonusMoneyConfigInfo>
    {

    }

    /// <summary>
    /// 指定用户对应彩种不加奖
    /// </summary>
    [CommunicationObject]
    public class UserGameCodeNotAddMoneyInfo
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 串关方式
        /// </summary>
        public string PlayType { get; set; }
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class UserGameCodeNotAddMoneyInfoCollection : List<UserGameCodeNotAddMoneyInfo>
    {

    }
}
