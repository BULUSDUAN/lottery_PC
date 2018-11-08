using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace EntityModel.Enum
{

    /// <summary>
    /// 日志类型，日志按严重程度分成提示、警告和错误。
    /// </summary>
    [Serializable]
    public enum LogType
    {
        /// <summary>
        /// 提示
        /// </summary>
        Information = 0,
        /// <summary>
        /// 警告
        /// </summary>
        Warning = 1,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 2
    }
    /// <summary>
    /// 投注方案类别
    /// </summary>
    public enum SchemeType
    {
        /// <summary>
        /// 普通投注
        /// </summary>
        GeneralBetting = 1,
        /// <summary>
        /// 追号投注
        /// </summary>
        ChaseBetting = 2,
        /// <summary>
        /// 合买投注
        /// </summary>
        TogetherBetting = 3,
        /// <summary>
        /// 专家方案
        /// </summary>
        ExperterScheme = 4,
        /// <summary>
        /// 保存的订单
        /// </summary>
        SaveScheme = 5,
        /// <summary>
        /// 宝单
        /// </summary>
        SingleTreasure = 6,
        /// <summary>
        /// 抄单
        /// </summary>
        SingleCopy = 7,
    }
    /// <summary>
    /// 出票状态
    /// </summary>
    public enum TicketStatus
    {
        /// <summary>
        /// 等待投注
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 投注中
        /// </summary>
        Ticketing = 10,
        /// <summary>
        /// 已打票
        /// </summary>
        PrintTicket = 20,
        /// <summary>
        /// 被终止
        /// </summary>
        Abort = 50,
        /// <summary>
        /// 被跳过
        /// </summary>
        Skipped = 70,
        /// <summary>
        /// 已投注完成
        /// </summary>
        Ticketed = 90,
        /// <summary>
        /// 有错误发生
        /// </summary>
        Error = 99,
    }

    /// <summary>
    /// 进行状态
    /// </summary>
    public enum ProgressStatus
    {
        /// <summary>
        /// 等待中
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 进行中
        /// </summary>
        Running = 10,
        /// <summary>
        /// 自动停止
        /// </summary>
        AutoStop = 20,
        /// <summary>
        /// 被中断
        /// </summary>
        Aborted = 80,
        /// <summary>
        /// 已完成
        /// </summary>
        Complate = 90,
    }
    public enum ResponseCode
    {
        成功 = 101,
        失败 = 201,
        //验证码错误
        ValiteCodeError = 301,
    }

    public enum AdminResponseCode
    {
        成功 = 101,
        失败 = 201,

        //验证码错误
        ValiteCodeError = 301,
        未登录 = 501
    }
    /// <summary>
    /// 中奖状态
    /// </summary>
    public enum BonusStatus
    {
        /// <summary>
        /// 未开奖
        /// </summary>
        Waitting = 0,
        /// <summary>
        /// 正在计算派奖
        /// </summary>
        Awarding = 10,
        /// <summary>
        /// 已中奖
        /// </summary>
        Win = 20,
        /// <summary>
        /// 未中奖
        /// </summary>
        Lose = 30,
        /// <summary>
        /// 错误
        /// </summary>
        Error = 99,
    }
    /// <summary>
    /// 收支类型
    /// </summary>
    public enum PayType
    {
        /// <summary>
        /// 支出
        /// </summary>
        Payout = 10,
        /// <summary>
        /// 收入
        /// </summary>
        Payin = 20
    }
    /// <summary>
    /// 账户类型。
    /// </summary>
    public enum AccountType
    {
        /// <summary>
        /// 通用
        /// </summary>
        //Common = 0,
        /// <summary>
        /// 奖金
        /// </summary>
        Bonus = 10,
        /// <summary>
        /// 冻结
        /// </summary>
        Freeze = 20,
        /// <summary>
        /// 佣金
        /// </summary>
        Commission = 30,
        /// <summary>
        /// 活动
        /// </summary>
        //Activity = 40,


        /// <summary>
        /// 充值
        /// </summary>
        FillMoney = 50,
        /// <summary>
        /// 名家
        /// </summary>
        Experts = 60,
        /// <summary>
        /// 红包
        /// </summary>
        RedBag = 70,
        /// <summary>
        /// CPS余额
        /// </summary>
        CPS = 80,

        /// <summary>
        /// 会员成长值
        /// </summary>
        UserGrowth = 90,
        /// <summary>
        /// 会员豆豆值
        /// </summary>
        DouDou = 91,
    }
    public enum FillMoneyStatus
    {
        /// <summary>
        /// 充值中
        /// </summary>
        /// 
        [Remark("转帐中")]
        Requesting = 0,
        /// <summary>
        /// 成功
        /// </summary>
        /// 
        [Remark("成功")]
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        /// 
        [Remark("失败")]
        Failed = 2
    }
    /// <summary>
    /// 充值代理商类型
    /// </summary>
    public enum FillMoneyAgentType
    {
        Alipay = 10,
        AlipayWAP = 15,
        Yeepay = 20,
        YingBao = 21,
        YiJiFu = 22,
        Tenpay = 30,
        KuanQian = 40,
        ChinaPay = 50,
        CallsPay = 60,
        ManualAdd = 80,
        ManualDeduct = 90,
        ManualFill = 99,
        BiFuBao = 110,
        QuDao = 120,
        IPS_Bank = 121,
        IPS = 122,
        ZF_Bank = 123,
        HC_Bank = 124,
        HC_Quick = 125,
        WXPay = 126,
        YS_Bank = 127,
        YF_WEIXIN = 128,
        ZTPay = 129,
        ZTAlipay = 130,
        HWAlipay = 131,
        ZT_Bank = 132,
        HW_Bank = 133,
        HW_Quick = 134,
        ka101_express = 135,
        ka101_bank = 136,
        ka101_weixin = 137,
        ka101_alipay = 138,
        slf_alipay = 139,
        slf_weixin = 140,
        slf_express = 141,
        slf_bank = 142,
        HWWeixin = 143,
        duobao_bank = 144,
        duobao_express = 145,
        duobao_alipay = 146,
        duobao_weixin = 147,
        yespay_alipay = 148,
        yespay_weixin = 149,
        yespay_express = 150,
        yespay_bank = 151,
        mobao_express = 152,
        jhz_bank = 153,
        jhz_alipay = 154,
        jhz_weixin = 155,
        sandpay_bank = 156,
        sandpay_express = 157,
        sandpay_alipay = 158,
        sandpay_weixin = 159,
        payworth_bank = 160,
        payworth_alipay = 161,
        payworth_weixin = 162,
        duodebao_bank = 163,
        duodebao_weixin = 164,
        duodebao_alipay = 165,
        duodebao_qq = 166,
        txf_bank = 167,
        txf_weixin = 168,
        txf_alipay = 169,
        sfb_bank = 170,
        sfb_weixin = 171,
        sfb_alipay = 172,
        txf_qq = 173,
        sfb_qq = 174,
        haio_bank = 175,
        haio_alipay = 176,
        haio_weixin = 177,
        haio_qq = 178,
        ZT_qq = 179,
        hfb_bank = 180,
        hfb_express = 181,
        txf_upay = 184,
        czzy = 185,
        af_bank = 186,
        af_upay = 187,
        af_weixin = 188,
        af_alipay = 189,
        af_qq = 190,
        af_H5qqWap = 191,
        af_H5wxWap = 192,
        slf_qq = 193,
        slf_upay = 194,
        haoyi_weixin = 195,
        haoyi_alipay = 196,
        haoyi_H5wxWap = 197,
        haoyi_H5alipayWap = 198,
        xinfu_bank = 199,
        xinfu_express = 200,
        xinfu_qq = 201,
        xinfu_weixin = 202,
        haoyi_qq = 203,
        haoyi_express = 204,
        shenfu_express = 205,
        shenfu_bank = 206,
        shenfu_weixin = 207,
        shenfu_alipay = 208,
        shenfu_qq = 209,
        shenfu_upay = 210
    }
    /// <summary>
    /// 方案来源
    /// </summary>

    public enum SchemeSource
    {
        Web = 0,
        Iphone = 101,
        Android = 102,
        Wap = 103,
        Touch = 104,
        YQS = 105,
        YQS_Advertising = 106,
        NS_Bet = 107,
        YQS_Bet = 108,
        Publisher_0321 = 109,
        WX_GiveLottery = 110,
        Web_GiveLottery = 111,
        LuckyDraw = 112,
        NewIphone = 113,
        NewAndroid = 115,
        NewWeb = 116
    }
    public enum SchemeBettingCategory
    {
        GeneralBetting = 0,
        SingleBetting = 1,
        FilterBetting = 2,
        YouHua = 3,
        XianFaQiHSC = 4,
        ErXuanYi = 5,
        YiChangZS = 6,
        WinnerModel = 7,
        HunHeDG = 8
    }
    public enum WithdrawAgentType
    {
        Alipay = 10,
        Yeepay = 20,
        BankCard = 90,
        Integral_BankCard = 91,
        CPS_BankCard = 100
    }
    public enum WithdrawStatus
    {
        Requesting = 1,
        Request = 2,
        Success = 3,
        Refused = 4
    }

    /// <summary>
    /// 使用状态
    /// </summary>
    public enum EnableStatus
    {
        /// <summary>
        /// 可用的
        /// </summary>
        Enable = 0,
        /// <summary>
        /// 禁用的
        /// </summary>
        Disable = 1,
        /// <summary>
        /// 未知的
        /// </summary>
        Unknown = 9,
    }

    /// <summary>
    /// 系统角色类型
    /// </summary>
    public enum RoleType
    {
        /// <summary>
        /// 前台页面角色
        /// </summary>
        WebRole = 1,
        /// <summary>
        /// 后台管理角色
        /// </summary>
        BackgroundRole = 2,
    }

    /// <summary>
    /// 奖期状态
    /// </summary>
    public enum IssuseStatus
    {
        /// <summary>
        /// 未截至（销售期）
        /// </summary>
        OnSale = 10,
        /// <summary>
        /// 正在计算派奖
        /// </summary>
        Awarded = 20,
        /// <summary>
        /// 已截至(派奖完成)
        /// </summary>
        Stopped = 30,
    }


    /// <summary>
    /// 合买方案保密性
    /// </summary>
    public enum TogetherSchemeSecurity
    {
        /// <summary>
        /// 未知
        /// </summary>
        Unkown = 0,
        /// <summary>
        /// 公开
        /// </summary>
        Public = 1,
        /// <summary>
        /// 参与后公开
        /// </summary>
        JoinPublic = 2,
        /// <summary>
        /// 完成后公开
        /// </summary>
        CompletePublic = 3,
        /// <summary>
        /// 永久保密
        /// </summary>
        KeepSecrecy = 4,
        /// <summary>
        /// 上传方案公开
        /// </summary>
        UpLoadSchemePublic = 5,
        /// <summary>
        /// 第一场比赛截止投注后公开(宝单分享)
        /// </summary>
        FirstMatchStopPublic = 6,
    }
    /// <summary>
    /// 合买方案进度
    /// </summary>
    public enum TogetherSchemeProgress
    {
        /// <summary>
        /// 销售中
        /// </summary>
        SalesIn = 10,
        /// <summary>
        /// 达到目标
        /// </summary>
        Standard = 20,
        /// <summary>
        /// 满员
        /// </summary>
        Finish = 30,
        /// <summary>
        /// 撤销
        /// </summary>
        Cancel = 80,
        /// <summary>
        /// 已完成
        /// </summary>
        Completed = 90,
        /// <summary>
        /// 自动停止
        /// </summary>
        AutoStop = 99,
    }
    /// <summary>
    /// 参与合买种类
    /// </summary>
    public enum TogetherJoinType
    {
        /// <summary>
        /// 发起合买认购
        /// </summary>
        Subscription = 1,
        /// <summary>
        /// 订制跟单 参与合买
        /// </summary>
        FollowerJoin = 2,
        /// <summary>
        /// 参与合买
        /// </summary>
        Join = 3,
        /// <summary>
        /// 发起人保底
        /// </summary>
        Guarantees = 4,
        /// <summary>
        /// 系统保底
        /// </summary>
        SystemGuarantees = 5,
    }



    /// <summary>
    /// 
    /// </summary>
    public enum Taskstates
    {
        /// <summary>
        /// 未领取
        /// </summary>
        Unclaimed = -1,
        /// <summary>
        /// 进行中
        /// </summary>
        Process = 0,
        /// <summary>
        /// 完成
        /// </summary>
        Finish = 1
    }
    /// <summary>
    /// 收益状态
    /// </summary>
    public enum ProceedsStatus
    {
        /// <summary>
        /// 正常
        /// </summary>
        Normal = 0,
        /// <summary>
        /// 申请提现中
        /// </summary>
        RequestWithdraw = 1,
        /// <summary>
        /// 拒绝提现
        /// </summary>
        RefusedWithdraw = 2,
        /// <summary>
        /// 完成提现
        /// </summary>
        CompleteWithdraw = 3,
    }


    /// <summary>
    /// 红包类别
    /// </summary>
    public enum RedBagCategory
    {
        /// <summary>
        /// 充值赠送
        /// </summary>
        FillMoney = 0,
        /// <summary>
        /// 活动加奖
        /// </summary>
        Activity = 1,
        /// <summary>
        /// 会员等级提升
        /// </summary>
        UserUpLevel = 2,
        /// <summary>
        /// 合作奖励
        /// </summary>
        Partner = 3,
        /// <summary>
        /// 手工补偿
        /// </summary>
        Manual = 4,
        /// <summary>
        /// yqid邀请注册红包
        /// </summary>
        YqidRegister = 5,
        /// <summary>
        /// 分享推广送红包
        /// </summary>
        FxidRegister = 6,
        /// <summary>
        /// 分享中奖订单
        /// </summary>
        OrderRegister = 7
    }

    /// <summary>
    /// 冻结类别
    /// </summary>
    public enum FrozenCategory
    {
        /// <summary>
        /// 申请提现
        /// </summary>
        RequestWithdraw = 0,
        /// <summary>
        /// 追号投注
        /// </summary>
        ChaseBet = 1,
        /// <summary>
        /// 手工处理
        /// </summary>
        Manual = 2,
    }

    /// <summary>
    /// 提现类别
    /// </summary>
    public enum WithdrawCategory
    {
        /// <summary>
        /// 普通提现，正常提现
        /// </summary>
        General = 0,
        /// <summary>
        /// 可接受的提现，充值金额参与提现
        /// </summary>
        Acceptable = 1,
        /// <summary>
        /// 强制的，需要扣除手续费10%
        /// </summary>
        Compulsory = 2,
        /// <summary>
        /// 错误的提现，金额不满足
        /// </summary>
        Error = 9,
    }

    /// <summary>
    /// 加奖奖金分配方式
    /// </summary>
    public enum AddMoneyDistributionWay
    {
        /// <summary>
        /// 平均分配
        /// </summary>
        Average = 0,
        /// <summary>
        /// 发起者独享
        /// </summary>
        CreaterOnly = 1,
        /// <summary>
        /// 参与者分享
        /// </summary>
        JoinerOnly = 2,
    }

    /// <summary>
    /// 排序方式
    /// </summary>
    public enum OrderByCategory
    {
        DESC = 0,
        ASC = 1,
    }
    /// <summary>
    /// 用户战绩列表查询排序属性
    /// </summary>
    public enum QueryUserBeedingListOrderByProperty
    {
        TotalBonusMoney = 0,
        TotalBonusTimes = 1,
        BeFollowedTotalMoney = 2,
        BeFollowerUserCount = 3,
    }
    /// <summary>
    /// 合买大厅查询排序属性
    /// </summary>
    public enum QueryTogetherListOrderByProperty
    {
        Default = 0,
        DisplayName = 1,
        TogetherSchemeSuccessGainMoney = 2,
        TotalMoney = 3,
        Progress = 4,
        SurplusCount = 5,
        StopTime = 6,
    }

    public enum RequestTicketConfigCategory
    {
        ByGameCode = 0,
        ByGameCodeAndGameType = 1,
        ByUser = 2,
        ByUserAndGameCode = 3,
        ByUsweAndGameCodeAndGameType = 4,
    }

    public enum RequestTicketGateWay
    {
        Local = 0,
        ZHM = 1,
        Shop = 2,
        CQCenter = 3,
    }

    /// <summary>
    /// 专家方案订购类型
    /// </summary>
    public enum BookingExperterCategory
    {
        /// <summary>
        /// 一周
        /// </summary>
        OneWeek = 0,
        /// <summary>
        /// 一月
        /// </summary>
        OneMonth = 1,
        /// <summary>
        /// 三月
        /// </summary>
        ThreeMonth = 2,
        /// <summary>
        /// 一年
        /// </summary>
        OneYear = 3,
    }

    /// <summary>
    /// 公告代理类别
    /// </summary>
    public enum BulletinAgent
    {
        /// <summary>
        /// 新体彩(阿里彩)
        /// </summary>
        Local = 0,
        /// <summary>
        /// 114彩票
        /// </summary>
        CaiPiao114 = 1,
        /// <summary>
        /// 手机SDK广告
        /// </summary>
        SDKBulletion = 2,
    }
    /// <summary>
    /// 用户积分操作
    /// </summary>
    public enum IntegralExchangeType
    {
        /// <summary>
        /// 收入
        /// </summary>
        IntegralIn = 100,
        /// <summary>
        /// 支出
        /// </summary>
        IntegralOut = 110,
    }

    /// <summary>
    /// 三峡付交易类型
    /// </summary>
    public enum SXFTradingType
    {
        /// <summary>
        /// 支出
        /// </summary>
        Spending = 0,
        /// <summary>
        /// 收入
        /// </summary>
        Income = 1,
    }

    /// <summary>
    /// 广告类别
    /// </summary>
    public enum BannerType
    {
        /// <summary>
        /// 首页
        /// </summary>
        Index = 10,
        /// <summary>
        /// APP
        /// </summary>
        APP = 20,
        /// <summary>
        /// SDK
        /// </summary>
        SDK = 30,
        /// <summary>
        /// 资讯
        /// </summary>
        Index_ZiXun = 40,
        /// <summary>
        /// 三峡上传图片
        /// </summary>
        Tgbank_UpLoadImg = 50,
        /// <summary>
        /// 触屏首页
        /// </summary>
        Touch_Index = 60,
        /// <summary>
        /// 苹果端
        /// </summary>
        IOS = 70,
    }

    /// <summary>
    /// 专家类型
    /// </summary>
    public enum ExperterType
    {
        /// <summary>
        /// 网站自己的专家
        /// </summary>
        AdminUser = 10,
        /// <summary>
        /// 用户专家
        /// </summary>
        OocaiUser = 20,
    }

    /// <summary>
    /// 代理类别
    /// </summary>
    public enum OCAgentCategory
    {
        /// <summary>
        /// 公司
        /// </summary>
        Company = 0,
        /// <summary>
        /// 市场
        /// </summary>
        Market = 1,
        /// <summary>
        /// 普通代理
        /// </summary>
        GeneralAgent = 2,
        /// <summary>
        /// 体彩门店代理
        /// </summary>
        SportLotteryAgent = 3,
        /// <summary>
        /// 体彩门店下级代理
        /// </summary>
        SportLotterySubAgent = 4,
        /// <summary>
        /// 普通用户
        /// </summary>
        User = 5,
        /// <summary>
        /// 推广员
        /// </summary>
        Extension = 6,
    }

    /// <summary>
    /// 吐槽类别
    /// </summary>
    public enum CommentsTpye
    {
        /// <summary>
        /// 首页
        /// </summary>
        HomeIndx = 10,
        /// <summary>
        /// 专家
        /// </summary>
        Experter = 20,
        /// <summary>
        /// 所有专家
        /// </summary>
        AllExperter = 30,
        /// <summary>
        /// 推荐方案
        /// </summary>
        RecommendScheme = 40,
        /// <summary>
        /// 分析方案
        /// </summary>
        AnalyzeScheme = 50,
    }

    /// <summary>
    /// 优化方式
    /// </summary>
    public enum YouHuaCategory
    {
        /// <summary>
        /// 平稳优化
        /// </summary>
        Average = 0,
        /// <summary>
        /// 博热优化
        /// </summary>
        HotFirst = 1,
        /// <summary>
        /// 博冷优化
        /// </summary>
        ColdFirst = 2,
    }

    /// <summary>
    /// 方案支持与反对
    /// </summary>
    public enum Vote
    {
        /// <summary>
        /// 支持
        /// </summary>
        Support = 0,
        /// <summary>
        /// 反对
        /// </summary>
        Against = 1,
    }

    /// <summary>
    /// 处理类别
    /// </summary>
    public enum DealWithType
    {
        /// <summary>
        /// 未处理
        /// </summary>
        NoneDealWith = 0,
        /// <summary>
        /// 已处理
        /// </summary>
        HasDealWith = 1,
        /// <summary>
        /// 未通过
        /// </summary>
        NoneThrough = 2,
    }

    /// <summary>
    /// 命中率
    /// </summary>
    public enum ShootingType
    {
        /// <summary>
        /// 周命中率
        /// </summary>
        Week = 0,
        /// <summary>
        /// 月命中率
        /// </summary>
        Month = 1,
        /// <summary>
        /// 总命中率
        /// </summary>
        Total = 2,
    }

    /// <summary>
    /// 活动三选一
    /// </summary>
    public enum ActivityType
    {
        /// <summary>
        /// 疯狂加奖百分之一十八
        /// </summary>
        AddAward = 0,
        /// <summary>
        /// 购彩返利
        /// </summary>
        Rebate = 1,
        /// <summary>
        /// 不参与
        /// </summary>
        NoParticipate = 2,
    }

    /// <summary>
    /// 投票状态
    /// </summary>
    public enum IsVote
    {
        /// <summary>
        /// 未投票
        /// </summary>
        NoVote = 0,
        /// <summary>
        /// 反对
        /// </summary>
        Against = 1,
        /// <summary>
        /// 支持
        /// </summary>
        Support = 2,
    }
    /// <summary>
    /// 订单失败类型
    /// </summary>
    public enum OrderFailType
    {
        /// <summary>
        /// 网络繁忙
        /// </summary>
        TimeOut = 10,
        /// <summary>
        /// 部分票失败
        /// </summary>
        PartTicket = 20,
        /// <summary>
        /// 全部张票失败
        /// </summary>
        AllTicket = 30,
        /// <summary>
        /// 合买状态错误
        /// </summary>
        TogetherStatusError = 40,
        /// <summary>
        /// 合买未达到目标
        /// </summary>
        TogetherMeetConditions = 50,
        /// <summary>
        /// 订单异常
        /// </summary>
        OrderAbnormal = 60,
        /// <summary>
        /// 退票
        /// </summary>
        ReturnTicket = 70,
    }
    /// <summary>
    /// UrlType使用端口
    /// </summary>
    public enum UrlType
    {
        /// <summary>
        /// 所有站通用
        /// </summary>
        All = 10,
        /// <summary>
        /// 安卓
        /// </summary>
        Android = 20,
        /// <summary>
        /// 苹果
        /// </summary>
        IOS = 30
    }
    /// <summary>
    /// 网站服务项
    /// </summary>
    public enum ServiceType
    {
        /// <summary>
        /// 提款通知
        /// </summary>
        DrawingNotice = 10,
    }


    /// <summary>
    /// 站内信处理类型
    /// </summary>
    public enum InnerMailHandleType
    {
        /// <summary>
        /// 未读
        /// </summary>
        UnRead = 0,
        /// <summary>
        /// 已读
        /// </summary>
        Readed = 1,
        /// <summary>
        /// 删除
        /// </summary>
        Deleted = 2,
    }
    /// <summary>
    /// 站内信接收者类型
    /// </summary>
    public enum InnerMailReceiverType
    {
        /// <summary>
        /// 所有
        /// </summary>
        All = 0,
        /// <summary>
        /// 指定角色
        /// </summary>
        Roles = 1,
        /// <summary>
        /// 指定用户
        /// </summary>
        Users = 2,
    }

    /// <summary>
    /// 站内信息类别
    /// </summary>
    public enum SiteMessageCategory
    {
        /// <summary>
        /// 不通知
        /// </summary>
        None = 0,
        /// <summary>
        /// 手机短信
        /// </summary>
        MobileSMS = 1,
        /// <summary>
        /// 站内信
        /// </summary>
        InnerMail = 2,
    }



    /// <summary>
    /// Web Wap生成静态页事件通知
    /// </summary>
    public enum WebBuildStaticFileEventCategory
    {
        /// <summary>
        /// 更新网站公告 
        /// </summary>
        OnUpdateBulletin = 10,
        /// <summary>
        /// 更新网站文章
        /// </summary>
        OnUpdateArticle = 11,
        /// <summary>
        /// 更新广告图
        /// </summary>
        OnUpdateBanner = 12,
        /// <summary>
        /// 更新数字彩开奖结果
        /// </summary>
        OnUpdateLotteryNumber = 13,
        /// <summary>
        /// 更新竞彩足球开奖结果
        /// </summary>
        OnUpdateJCZQResult = 14,
        /// <summary>
        /// 更新竞彩篮球开奖结果
        /// </summary>
        OnUpdateJCLQResult = 15,
        /// <summary>
        /// 更新北京单场开奖结果
        /// </summary>
        OnUpdateBJDCResult = 16,
        /// <summary>
        /// 订单派奖
        /// </summary>
        OnOrderPrize = 17,

    }

    /// <summary>
    /// 通知类型
    /// </summary>
    public enum NoticeType
    {
        /// <summary>
        /// 接收票成功
        /// </summary>
        ReceiveTicket = 9,
        /// <summary>
        /// 出票结果通知
        /// </summary>
        TicketNotice = 10,
        /// <summary>
        /// 出票结果通知
        /// </summary>
        TicketNotice_BJDC = 11,
        /// <summary>
        /// 出票结果通知
        /// </summary>
        TicketNotice_JCZQ = 12,
        /// <summary>
        /// 出票结果通知
        /// </summary>
        TicketNotice_JCLQ = 13,
        /// <summary>
        /// 出票结果通知
        /// </summary>
        TicketNotice_CTZQ = 14,
        /// <summary>
        /// 奖期通知
        /// </summary>
        IssuseNotice = 20,
        /// <summary>
        /// 删除奖期通知
        /// </summary>
        DeleteIssuse = 21,
        /// <summary>
        /// 开奖通知
        /// </summary>
        AwardNotice = 30,
        /// <summary>
        /// 派奖通知
        /// </summary>
        BonusNotice = 40,
        /// <summary>
        /// 派奖通知新
        /// </summary>
        BonusNotice_New = 400,
        /// <summary>
        /// 派奖通知
        /// </summary>
        BonusNotice_Order = 401,
        /// <summary>
        /// 派奖通知
        /// </summary>
        BonusNotice_BJDC = 41,
        /// <summary>
        /// 派奖通知
        /// </summary>
        BonusNotice_JCZQ = 42,
        /// <summary>
        /// 派奖通知
        /// </summary>
        BonusNotice_JCLQ = 43,

        /// <summary>
        /// 北京单场奖期队伍
        /// </summary>
        BJDC_Issuse = 50,
        /// <summary>
        /// 北京单场比赛队伍（包括：添加、更新）
        /// </summary>
        BJDC_Match = 51,
        /// <summary>
        /// 北京单场比赛结果（包括：添加、更新）
        /// </summary>
        BJDC_MatchResult = 52,
        /// <summary>
        /// 北单胜负过关
        /// </summary>
        BJDC_Match_SFGG = 55,
        /// <summary>
        /// 北京单场比赛结果（包括：添加、更新）
        /// </summary>
        BJDC_MatchResult_SFGG = 56,

        /// <summary>
        /// 传统足球奖期（包括：添加、更新）
        /// </summary>
        CTZQ_Issuse = 60,
        /// <summary>
        /// 传统足球比赛队伍（包括：添加、更新）
        /// </summary>
        CTZQ_Match = 61,
        /// <summary>
        /// 传统足球比赛SP（包括：添加、更新）
        /// </summary>
        CTZQ_Odds = 62,
        /// <summary>
        /// 传统足球奖池结果（包括：添加、更新）
        /// </summary>
        CTZQ_MatchPool = 63,

        /// <summary>
        /// 竞彩足球比赛队伍
        /// </summary>
        JCZQ_Match = 70,
        /// <summary>
        /// 竞彩足球比赛结果
        /// </summary>
        JCZQ_MatchResult = 71,

        JCZQ_SPF_SP = 72,
        JCZQ_SPF_SP_DS = 720,
        JCZQ_BF_SP = 73,
        JCZQ_BF_SP_DS = 730,
        JCZQ_ZJQ_SP = 74,
        JCZQ_ZJQ_SP_DS = 740,
        JCZQ_BQC_SP = 75,
        JCZQ_BQC_SP_DS = 750,
        JCZQ_BRQSPF_SP = 76,
        JCZQ_BRQSPF_SP_DS = 760,

        /// <summary>
        /// 竞彩篮球比赛队伍
        /// </summary>
        JCLQ_Match = 80,
        /// <summary>
        /// 竞彩篮球比赛结果
        /// </summary>
        JCLQ_MatchResult = 81,

        JCLQ_SF_SP = 82,
        JCLQ_SF_SP_DS = 820,
        JCLQ_RFSF_SP = 83,
        JCLQ_RFSF_SP_DS = 830,
        JCLQ_SFC_SP = 84,
        JCLQ_SFC_SP_DS = 840,
        JCLQ_DXF_SP = 85,
        JCLQ_DXF_SP_DS = 850,

        JCSJB_GJ = 110,
        JCOZB_GJ = 111,
        JCSJB_GYJ = 130,
        JCOZB_GYJ = 131,

        /// <summary>
        /// 数字彩奖池结果（包括：添加、更新）
        /// </summary>
        SZC_MatchPool = 100,


        /// <summary>
        /// 出票成功通知
        /// </summary>
        TicketSuccess = 90,
        /// <summary>
        /// 打票成功通知
        /// </summary>
        PrintTicketSuccess = 91,
        /// <summary>
        /// 出票成功通知
        /// </summary>
        TicketSuccess_New = 900,
        /// <summary>
        /// 退票通知
        /// </summary>
        Fund = 120
    }

    /// <summary>
    /// 抄单来源
    /// </summary>
    public enum CopyOrderSource
    {
        /// <summary>
        /// 暂无来源
        /// </summary>
        NotStatus = 0,
        /// <summary>
        /// 大单
        /// </summary>
        BigOrder = 10,
        /// <summary>
        /// 宝单分享
        /// </summary>
        BDFX = 20,
    }

    /// <summary>
    /// CPS模式
    /// </summary>
    public enum CPSMode
    {
        /// <summary>
        /// 返点
        /// </summary>
        PayRebate = 0,
        /// <summary>
        /// 分红
        /// </summary>
        PayBonus = 1,
    }
    /// <summary>
    /// 传统足球场次状态
    /// </summary>
    public enum CTZQMatchState
    {
        Waiting = 0,
        Running = 10,
        Finished = 20,
    }
    public enum BJDCMatchState
    {
        Sales = 0,
        Stop = 1,
    }

    #region Extension项目的枚举    
    /// <summary>
    /// <summary>    /// 任务类别
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
    }    /// <summary>


    /// <summary>    
    /// /// 新闻类别
    /// </summary>
    public enum NewsCategory
    {
        /// <summary>
        /// 高频彩
        /// </summary>
        GPC = 1,
        /// <summary>
        /// 福彩
        /// </summary>
        FC = 2,
        /// <summary>
        /// 体彩
        /// </summary>
        TC = 3,
        /// <summary>
        /// 地方彩
        /// </summary>
        DFC = 4,
        /// <summary>
        /// 焦点资讯
        /// </summary>
        JDZX = 5,
        /// <summary>
        /// 热点资讯
        /// </summary>
        RDZX = 6,
        /// <summary>
        /// all
        /// </summary>
        All = 7,
        /// <summary>
        /// 高频彩技巧
        /// </summary>
        GPCJQ = 8,
    }

    /// <summary>
    /// 中国工商银行	1	ICBC
    ///    招商银行	2	CMB
    //   中国建设银行  3	CCB
    //   中国农业银行  4	ABC
    //   中国银行    5	BOC
    //   交通银行    6	BCM
    //  中国民生银行  7	CMBC
    //  中信银行    8	ECC
    //   上海浦东发展银行    9	SPDB
    //  邮政储汇    10	PSBC
    //  中国光大银行  11	CEB
    // 平安银行 （原深圳发展银行）	12	PINGAN
    // 广发银行股份有限公司  13	CGB
    // 华夏银行    14	HXB
    // 福建兴业银行  15	CIB

    /// </summary>
    public enum BankCode : int
    {
        ICBC = 1,
        CMB = 2,
        CCB = 3,
        ABC = 4,
        BOC = 5,
        BCM = 6,
        CMBC = 7,
        ECC = 8,
        SPDB = 9,
        PSBC = 10,
        CEB = 11,
        PINGAN = 12,
        CGB = 13,
        HXB = 14,
        CIB = 15
    }
    #endregion

    public enum GameTransferType
    {
        /// <summary>
        /// 充值
        /// </summary>
        /// 
        [Remark("充值")]
        Recharge = 0,
        /// <summary>
        /// 提款
        /// </summary>
        /// 
        [Remark("提款")]
        Withdraw = 1
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
    public enum AdminSelectType
    {
        /// <summary>
        /// 未选中
        /// </summary>
        NotSelect=0,
        /// <summary>
        /// 已选中
        /// </summary>
        Selecct=1,
        /// <summary>
        /// 必须选中
        /// </summary>
        MustSelect=2
    }

    public enum MGGameType
    {
        /// <summary>
        /// 普通游戏
        /// </summary>
        /// 
        [Remark("SMG")]
        SMG = 0,
        /// <summary>
        /// 捕鱼
        /// </summary>
        /// 
        [Remark("SMF")]
        SMF = 1
    }

    #region 枚举标签
    /// <summary>
    /// 备注特性
    /// </summary>
    public class RemarkAttribute : Attribute
    {
        private string m_remark;
        public RemarkAttribute(string remark)
        {
            this.m_remark = remark;
        }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark
        {
            get { return m_remark; }
            set { m_remark = value; }
        }
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        /// <param name="val">枚举值</param>
        /// <returns></returns>
        public static string GetEnumRemark(System.Enum val)
        {
            Type type = val.GetType();
            FieldInfo fd = type.GetField(val.ToString());
            if (fd == null)
                return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(RemarkAttribute), false);
            string name = string.Empty;
            foreach (RemarkAttribute attr in attrs)
            {
                name = attr.Remark;
            }
            return name;
        }
    }
    /// <summary>
    /// 枚举扩展类
    /// </summary>
    public static class EnumExtension
    {
        /// <summary>
        /// 获取枚举的备注信息
        /// </summary>
        /// <param name="em"></param>
        /// <returns></returns>
        public static string GetRemark(this System.Enum em)
        {
            Type type = em.GetType();
            FieldInfo fd = type.GetField(em.ToString());
            if (fd == null)
                return string.Empty;
            object[] attrs = fd.GetCustomAttributes(typeof(RemarkAttribute), false);
            string name = string.Empty;
            foreach (RemarkAttribute attr in attrs)
            {
                name = attr.Remark;
            }
            return name;
        }
    }
    #endregion
}