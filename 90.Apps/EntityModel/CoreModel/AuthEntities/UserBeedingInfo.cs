using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntityModel.CoreModel
{ 
     /// <summary>
     /// 用户战绩列表对象
     /// </summary>
public class UserBeedingListInfo
{
    public string GameCode { get; set; }
    public string GameType { get; set; }
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int UserHideDisplayNameCount { get; set; }
    /// <summary>
    /// 金星个数
    /// </summary>
    public int GoldStarCount { get; set; }
    /// <summary>
    /// 金钻个数
    /// </summary>
    public int GoldDiamondsCount { get; set; }
    /// <summary>
    /// 金杯个数
    /// </summary>
    public int GoldCupCount { get; set; }
    /// <summary>
    /// 金冠个数
    /// </summary>
    public int GoldCrownCount { get; set; }
    /// <summary>
    /// 银星个数
    /// </summary>
    public int SilverStarCount { get; set; }
    /// <summary>
    /// 银钻个数
    /// </summary>
    public int SilverDiamondsCount { get; set; }
    /// <summary>
    /// 银杯个数
    /// </summary>
    public int SilverCupCount { get; set; }
    /// <summary>
    /// 银冠个数
    /// </summary>
    public int SilverCrownCount { get; set; }
    /// <summary>
    /// 被订制跟单人数
    /// </summary>
    public int BeFollowerUserCount { get; set; }
    /// <summary>
    /// 已被跟单总金额
    /// </summary>
    public decimal BeFollowedTotalMoney { get; set; }

    /// <summary>
    /// 总订单数
    /// </summary>
    public int TotalOrderCount { get; set; }
    /// <summary>
    /// 总投注金额
    /// </summary>
    public decimal TotalBetMoney { get; set; }
    /// <summary>
    /// 总中奖次数
    /// </summary>
    public int TotalBonusTimes { get; set; }
    /// <summary>
    /// 总中奖金额
    /// </summary>
    public decimal TotalBonusMoney { get; set; }
}

public class UserBeedingListInfoCollection
{
    public UserBeedingListInfoCollection()
    {
        List = new List<UserBeedingListInfo>();
    }
    public List<UserBeedingListInfo> List { get; set; }
    public int TotalCount { get; set; }
}


public class UserAttentionSummaryInfo
{
    public string UserId { get; set; }
    public string UserDisplayName { get; set; }
    public int HideDisplayNameCount { get; set; }
    //public int OrderIndex { get; set; }
    public int BeAttentionUserCount { get; set; }
    /// <summary>
    /// 用户中奖显示名
    /// </summary>
    public string MaxLevelName { get; set; }
}


public class UserAttentionSummaryInfoCollection
{
    public UserAttentionSummaryInfoCollection()
    {
        List = new List<UserAttentionSummaryInfo>();
    }
    public List<UserAttentionSummaryInfo> List { get; set; }
    public int TotalCount { get; set; }
}

public class UserAttentionInfo
{
    public long Id { get; set; }
    /// <summary>
    /// 被关注人编号
    /// </summary>
    public string BeAttentionUserId { get; set; }
    /// <summary>
    /// 关注人编号(粉丝)
    /// </summary>
    public string FollowerUserId { get; set; }
    public DateTime CreateTime { get; set; }
}

public class UserAttention_Collection
{
    public UserAttention_Collection()
    {
        AttentionList = new List<UserAttentionInfo>();
    }
    public int TotalCount { get; set; }
    public List<UserAttentionInfo> AttentionList { get; set; }
}
}
