using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace External.Core
{
    /// <summary>
    /// 投票类别
    /// </summary>
    public enum VoteCategory
    {
        /// <summary>
        /// 球队斗志
        /// </summary>
        TheTeamMorale = 0,
        /// <summary>
        /// 赛前状态
        /// </summary>
        PreCompetitionState = 1,
        /// <summary>
        /// 对阵往绩
        /// </summary>
        AgainstTheTrack = 2,
        /// <summary>
        /// 主客战绩
        /// </summary>
        TeamRecord = 3,
        /// <summary>
        /// 欧赔取向
        /// </summary>
        EuropeanCompensate = 4,
        /// <summary>
        /// 亚赔取向
        /// </summary>
        Asia = 5,
    }

    /// <summary>
    /// 任务类别
    /// </summary>
    public enum TaskCategory
    {
        /// <summary>
        /// 实名认证
        /// </summary>
        RealName = 10,
        /// <summary>
        /// 手机绑定
        /// </summary>
        MobilBinding = 20,
        /// <summary>
        /// 银行卡绑定
        /// </summary>
        BankCar = 30,
        /// <summary>
        /// 邮箱绑定
        /// </summary>
        EmailBinding = 40,
        /// <summary>
        /// 设置资金密码
        /// </summary>
        SetBalancePassword = 50,
        /// <summary>
        /// 每日购彩≥50元
        /// </summary>
        EverDayBuyLottery = 60,
        /// <summary>
        /// 首次充值
        /// </summary>
        FistTopUp = 70,
        /// <summary>
        /// 首次购彩
        /// </summary>
        FistBuyLottery = 80,
        /// <summary>
        /// 第一次累计消费10元
        /// </summary>
        FistConsumptionTenYuan = 90,
        /// <summary>
        /// 首次购买竞彩二串一（竞彩篮球、竞彩足球）
        /// </summary>
        FistJingcaiP2_1 = 100,
        /// <summary>
        /// 购买混投2串1
        /// </summary>
        FistHHP2_1 = 110,
        /// <summary>
        /// 购买竞彩2串1满5次(每日一次)
        /// </summary>
        JingcaiP2_1Totle5 = 120,
        /// <summary>
        /// 奖金优化投注满5次
        /// </summary>
        BonusBuyLotteryTotle5 = 130,
        /// <summary>
        /// 竞彩首次中奖
        /// </summary>
        JingcaiFistWin = 140,
        /// <summary>
        /// 首次追号投注
        /// </summary>
        FistZhuihaoBuy = 150,
        /// <summary>
        /// 首次参与合买
        /// </summary>
        FistHeMai = 160,
        /// <summary>
        /// 首次关注彩友
        /// </summary>
        FistFocusOnFriend = 170,
        /// <summary>
        /// 首次使用奖金优化
        /// </summary>
        BonusOptimize = 180,
        /// <summary>
        /// 在线过滤
        /// </summary>
        OnlineFilter = 190,
        /// <summary>
        /// 特别中奖奖励(奖金优化)
        /// </summary>
        SpecialWinReward = 200,
        /// <summary>
        /// 每日登录
        /// </summary>
        EveryDayLogin = 210,
        /// <summary>
        /// 中奖100次
        /// </summary>
        Win100Count = 220,
        /// <summary>
        /// 竞彩2串1中奖100次
        /// </summary>
        JCWin100Count = 230,
        /// <summary>
        /// 用户总中奖金额1000以上
        /// </summary>
        Win1000Yuan = 240,
    }

    /// <summary>
    /// 名家类别
    /// </summary>
    public enum CelebrityType
    {
        /// <summary>
        /// 后台名家
        /// </summary>
        AdminUser = 10,
        /// <summary>
        /// 网站名家
        /// </summary>
        CaibbUser = 20,
    }

    /// <summary>
    /// 票样申请状态
    /// </summary>
    public enum ApplyState
    {
        /// <summary>
        /// 申请中
        /// </summary>
        Pending = 0,
        /// <summary>
        /// 派送中
        /// </summary>
        Sending = 1,
        /// <summary>
        /// 派送完成
        /// </summary>
        SendCarryout = 2,
        /// <summary>
        /// 已拒绝
        /// </summary>
        Refusal = 3,
        /// <summary>
        /// 已取消
        /// </summary>
        Canceled = 4,
    }

    #region 新赢家平台

    /// <summary>
    /// 模型类别
    /// </summary>
    public enum ModelType
    {
        /// <summary>
        /// 模型_自选模型
        /// </summary>
        OptionalModel = 10,
    }
    /// <summary>
    /// 模型每期方案_每期方案状态
    /// </summary>
    public enum ModelProgressStatus
    {
        /// <summary>
        /// 认购中
        /// </summary>
        ModelBuy = 10,
        /// <summary>
        /// 未开奖
        /// </summary>
        NotLottery = 40,
        /// <summary>
        /// 未中奖
        /// </summary>
        NoBonus = 20,
        /// <summary>
        /// 已中奖
        /// </summary>
        Winning = 30,

    }
    /// <summary>
    /// 追号计划_追号计划状态
    /// </summary>
    public enum SchemeProgressStatus
    {
        /// <summary>
        /// 执行中
        /// </summary>
        ModelRunning = 10,
        /// <summary>
        /// 已停止
        /// </summary>
        ModelStop = 20,
    }
    /// <summary>
    /// 追号计划_支付状态
    /// </summary>
    public enum PayStatus
    {
        /// <summary>
        /// 已支付
        /// </summary>
        SuccessPay = 10,
        /// <summary>
        /// 未支付
        /// </summary>
        WaitingPay = 20,
        /// <summary>
        /// 支付失败
        /// </summary>
        FailPay = 30,
    }
    public enum ModelSecurity
    {
        /// <summary>
        /// 公开
        /// </summary>
        Public = 10,
        /// <summary>
        /// 截止后公开
        /// </summary>
        CompletePublic = 20,
    }
    public enum RiskType
    {
        /// <summary>
        /// 适中
        /// </summary>
        Moderate = 10,
        /// <summary>
        /// 低风险
        /// </summary>
        Low = 20,
    }
    /// <summary>
    /// 投注类型
    /// </summary>
    public enum BettingType
    {
        /// <summary>
        /// 先行赔付
        /// </summary>
        FirstPayment = 10,
        /// <summary>
        /// 盈利计划
        /// </summary>
        Profit = 20,
    }
    /// <summary>
    /// 投注模式
    /// 主要包括盈利计划里面：推荐计划：（低风险、适中）；高级设置：（低风险翻倍、固定翻倍、固定盈利率）
    /// </summary>
    public enum ProfitBettingCategory
    {
        /// <summary>
        /// 低风险
        /// </summary>
        DiFX = 10,
        /// <summary>
        /// 适中
        /// </summary>
        ShiZ = 20,
        /// <summary>
        /// 低风险翻倍
        /// </summary>
        DiFengXianFB = 30,
        /// <summary>
        /// 固定翻倍
        /// </summary>
        GuDingFB = 40,
        /// <summary>
        /// 固定盈利率
        /// </summary>
        GuDingYLL = 50,
    }
    /// <summary>
    /// 购买类型
    /// </summary>
    public enum BuyPayType
    {
        /// <summary>
        /// 分期支付
        /// </summary>
        Staging = 10,
        /// <summary>
        /// 全额支付
        /// </summary>
        AllMoney = 20,
    }
    /// <summary>
    /// 菜单类型
    /// </summary>
    public enum MenuType
    {
        /// <summary>
        /// 所有站点都显示
        /// </summary>
        All = 1,
        /// <summary>
        /// caibb网站管理后台菜单
        /// </summary>
        Web_Menu = 10,
        /// <summary>
        /// 代理商管理后台菜单
        /// </summary>
        Agent_Menu = 20,
    }

    #endregion

}
