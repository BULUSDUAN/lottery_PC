using System;
using System.Collections.Generic;
using System.Text;
using EntityModel.Enum;
namespace EntityModel.CoreModel
{
   //public class Sports_TogetherSchemeQueryInfoCollection:Page
   // {
   //     public List<Sports_TogetherSchemeQueryInfo> List { get; set; }
   // }
    public class Sports_TogetherSchemeQueryInfo
    {
        public string SchemeId { get; set; }
        /// <summary>
        /// 方案来源
        /// </summary>
        public SchemeSource SchemeSource { get; set; }
        public bool IsTop { get; set; }
        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 发起人编号
        /// </summary>
        public string CreateUserId { get; set; }
        /// <summary>
        /// 发起人名称
        /// </summary>
        public string CreaterDisplayName { get; set; }
        public int CreaterHideDisplayNameCount { get; set; }
        /// <summary>
        /// 参与密码
        /// </summary>
        public string JoinPwd { get; set; }

        /// <summary>
        /// 方案保密性
        /// </summary>
        public TogetherSchemeSecurity Security { get; set; }
        /// <summary>
        /// 总金额
        /// </summary>
        public decimal TotalMoney { get; set; }
        /// <summary>
        /// 总份数
        /// </summary>
        public int TotalCount { get; set; }
        public int SoldCount { get; set; }
        public int SurplusCount { get; set; }
        public int JoinUserCount { get; set; }
        /// <summary>
        /// 每份单价
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 中奖提成 0-10
        /// </summary>
        public int BonusDeduct { get; set; }
        /// <summary>
        /// 方案提成
        /// </summary>
        public decimal SchemeDeduct { get; set; }
        /// <summary>
        /// 发起人认购份数
        /// </summary>
        public int Subscription { get; set; }
        /// <summary>
        /// 发起人保底份数
        /// </summary>
        public int Guarantees { get; set; }
        /// <summary>
        /// 系统保底份数
        /// </summary>
        public int SystemGuarantees { get; set; }
        /// <summary>
        /// 进度面分比
        /// </summary>
        public decimal Progress { get; set; }
        /// <summary>
        /// 进度状态
        /// </summary>
        public TogetherSchemeProgress ProgressStatus { get; set; }

        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string GameDisplayName { get; set; }
        public string GameTypeDisplayName { get; set; }
        public string PlayType { get; set; }
        public string IssuseNumber { get; set; }
        public int TotalMatchCount { get; set; }
        public int Amount { get; set; }
        public int BetCount { get; set; }
        public DateTime StopTime { get; set; }

        public decimal PreTaxBonusMoney { get; set; }
        public decimal AfterTaxBonusMoney { get; set; }
        public string WinNumber { get; set; }
        public int BonusCount { get; set; }
        public int HitMatchCount { get; set; }

        public TicketStatus TicketStatus { get; set; }
        public SchemeBettingCategory SchemeBettingCategory { get; set; }
        public DateTime CreateTime { get; set; }
        public BonusStatus BonusStatus { get; set; }
        public bool IsPrizeMoney { get; set; }
        public decimal AddMoney { get; set; }
        public string AddMoneyDescription { get; set; }
        public bool IsVirtualOrder { get; set; }
        public string Attach { get; set; }
        public decimal MinBonusMoney { get; set; }
        public decimal MaxBonusMoney { get; set; }
        public string ExtensionOne { get; set; }

        public int GoldCrownCount { get; set; }
        public int GoldCupCount { get; set; }
        public int GoldDiamondsCount { get; set; }
        public int GoldStarCount { get; set; }
        public int SilverCrownCount { get; set; }
        public int SilverCupCount { get; set; }
        public int SilverDiamondsCount { get; set; }
        public int SilverStarCount { get; set; }
        public bool IsAppend { get; set; }
        public DateTime? TicketTime { get; set; }

    }
}
