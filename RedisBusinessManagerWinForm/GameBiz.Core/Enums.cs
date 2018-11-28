using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Core
{
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
    /// 方案来源
    /// </summary>
    public enum SchemeSource
    {
        /// <summary>
        /// 网站投注
        /// </summary>
        Web = 0,
        /// <summary>
        /// iPhone客户端投注
        /// </summary>
        Iphone = 101,
        /// <summary>
        /// Android客户端投注
        /// </summary>
        Android = 102,
        /// <summary>
        /// WAP网站投注
        /// </summary>
        Wap = 103,
        /// <summary>
        /// 触屏版投注
        /// </summary>
        Touch = 104,
        /// <summary>
        /// 摇钱树
        /// </summary>
        YQS = 105,
        /// <summary>
        /// 摇钱树广告
        /// </summary>
        YQS_Advertising = 106,
        /// <summary>
        /// 能士物业投注
        /// </summary>
        NS_Bet = 107,
        /// <summary>
        /// 摇钱树投注
        /// </summary>
        YQS_Bet = 108,
        /// <summary>
        /// 发布会现场活动
        /// </summary>
        Publisher_0321 = 109,
        /// <summary>
        /// 微信绑定身份证，手机号码送彩票
        /// </summary>
        WX_GiveLottery = 110,
        /// <summary>
        /// Web注册绑定身份证，手机号码送彩票
        /// </summary>
        Web_GiveLottery = 111,
        /// <summary>
        /// 轮盘抽奖
        /// </summary>
        LuckyDraw = 112,
    }
    /// <summary>
    /// 方案投注类别
    /// </summary>
    public enum SchemeBettingCategory
    {
        /// <summary>
        /// 普通投注
        /// </summary>
        GeneralBetting = 0,
        /// <summary>
        /// 单式上传
        /// </summary>
        SingleBetting = 1,
        /// <summary>
        /// 过滤投注
        /// </summary>
        FilterBetting = 2,
        /// <summary>
        /// 奖金优化投注
        /// </summary>
        YouHua = 3,
        /// <summary>
        /// 先发起后上传
        /// </summary>
        XianFaQiHSC = 4,
        /// <summary>
        /// 2选1
        /// </summary>
        ErXuanYi = 5,
        /// <summary>
        ///  一场致胜
        /// </summary>
        YiChangZS = 6,
        /// <summary>
        /// 赢家平台
        /// </summary>
        WinnerModel = 7,
        /// <summary>
        /// 混合单关(单关固定)
        /// </summary>
        HunHeDG = 8,
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
    /// 充值代理商接口
    /// </summary>
    public enum FillMoneyAgentType
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        Alipay = 10,
        /// <summary>
        /// 支付宝 - WAP支付
        /// </summary>
        AlipayWAP = 15,
        /// <summary>
        /// 易宝
        /// </summary>
        Yeepay = 20,
        /// <summary>
        /// 银宝
        /// </summary>
        YingBao = 21,
        /// <summary>
        /// 易极付
        /// </summary>
        YiJiFu = 22,
        /// <summary>
        /// 财付通
        /// </summary>
        Tenpay = 30,
        /// <summary>
        /// 快钱
        /// </summary>
        KuanQian = 40,
        /// <summary>
        /// 银联语音支付 - WEB支付
        /// </summary>
        ChinaPay = 50,
        /// <summary>
        /// 手机充值卡支付
        /// </summary>
        CallsPay = 60,
        /// <summary>
        /// 手工打款
        /// </summary>
        ManualAdd = 80,
        /// <summary>
        /// 手工扣款
        /// </summary>
        ManualDeduct = 90,
        /// <summary>
        /// 手工充值
        /// </summary>
        ManualFill = 99,
        /// <summary>
        /// 币付宝
        /// </summary>
        BiFuBao = 110,
        /// <summary>
        /// 渠道充值
        /// </summary>
        QuDao = 120,

        /// <summary>
        /// 环迅充值
        /// </summary>
        IPS = 122,
        /// <summary>
        /// 环迅网银
        /// </summary>
        IPS_Bank = 121,
        /// <summary>
        /// 智付网银
        /// </summary>
        ZF_Bank = 123,
        /// <summary>
        /// 汇潮网银
        /// </summary>
        HC_Bank = 124,
        /// <summary>
        /// 汇潮快捷
        /// </summary>
        HC_Quick = 125,

        /// <summary>
        /// 微信支付
        /// </summary>
        WXPay = 126,
        /// <summary>
        /// 银盛网银支付
        /// </summary>
        YS_Bank = 127,
        /// <summary>
        /// 微信(优付)
        /// </summary>
        YF_WEIXIN = 128,
        /// <summary>
        /// 中铁微信
        /// </summary>
        ZTPay = 129,
        /// <summary>
        /// 中铁支付宝
        /// </summary>
        ZTAlipay = 130,
        /// <summary>
        /// 汇旺支付宝
        /// </summary>
        HWAlipay = 131,
        /// <summary>
        /// 中铁银行支付
        /// </summary>
        ZT_Bank = 132,
        /// <summary>
        /// 汇旺网银
        /// </summary>
        HW_Bank = 133,
        /// <summary>
        /// 汇旺快捷
        /// </summary>
        HW_Quick = 134,
        /// <summary>
        /// 101卡快捷
        /// </summary>
        ka101_express = 135,
        /// <summary>
        /// 101卡网银
        /// </summary>
        ka101_bank = 136,
        /// <summary>
        /// 101卡微信
        /// </summary>
        ka101_weixin = 137,
        /// <summary>
        /// 101卡支付宝
        /// </summary>
        ka101_alipay = 138,
        /// <summary>
        /// 顺利付支付宝
        /// </summary>
        slf_alipay = 139,
        /// <summary>
        /// 顺利付微信
        /// </summary>
        slf_weixin = 140,
        /// <summary>
        /// 顺利付快捷
        /// </summary>
        slf_express = 141,
        /// <summary>
        /// 顺利付银行卡
        /// </summary>
        slf_bank = 142,
        /// <summary>
        /// 汇旺微信
        /// </summary>
        HWWeixin = 143,
        /// <summary>
        /// 多宝网银
        /// </summary>
        duobao_bank = 144,
        /// <summary>
        /// 多宝快捷
        /// </summary>
        duobao_express = 145,
        /// <summary>
        /// 多宝支付宝
        /// </summary>
        duobao_alipay = 146,
        /// <summary>
        /// 多宝微信
        /// </summary>
        duobao_weixin = 147,
        /// <summary>
        /// 银盛支付宝
        /// </summary>
        yespay_alipay = 148,
        /// <summary>
        /// 银盛微信
        /// </summary>
        yespay_weixin = 149,
        /// <summary>
        /// 银盛快捷
        /// </summary>
        yespay_express = 150,
        /// <summary>
        /// 银盛网银
        /// </summary>
        yespay_bank = 151,
        /// <summary>
        /// 摩宝快捷
        /// </summary>
        mobao_express = 152,
        /// <summary>
        /// 金海哲网银
        /// </summary>
        jhz_bank = 153,
        /// <summary>
        /// 金海哲支付宝
        /// </summary>
        jhz_alipay = 154,
        /// <summary>
        /// 金海哲微信
        /// </summary>
        jhz_weixin = 155,
        /// <summary>
        /// 杉德网银
        /// </summary>
        sandpay_bank = 156,
        /// <summary>
        /// 杉德快捷
        /// </summary>
        sandpay_express = 157,
        /// <summary>
        /// 杉德支付宝
        /// </summary>
        sandpay_alipay = 158,
        /// <summary>
        /// 杉德微信
        /// </summary>
        sandpay_weixin = 159,
        /// <summary>
        /// 华势网银
        /// </summary>
        payworth_bank = 160,
        /// <summary>
        /// 华势支付宝
        /// </summary>
        payworth_alipay = 161,
        /// <summary>
        /// 华势微信
        /// </summary>
        payworth_weixin = 162,
        /// <summary>
        /// 多得宝网银
        /// </summary>
        duodebao_bank = 163,
        /// <summary>
        /// 多得宝微信
        /// </summary>
        duodebao_weixin = 164,
        /// <summary>
        /// 多得宝支付宝
        /// </summary>
        duodebao_alipay = 165,
        /// <summary>
        /// 多得宝QQ
        /// </summary>
        duodebao_qq = 166,
        /// <summary>
        /// 天宝付网银
        /// </summary>
        txf_bank = 167,
        /// <summary>
        /// 天宝付微信
        /// </summary>
        txf_weixin = 168,
        /// <summary>
        /// 天宝付支付宝
        /// </summary>
        txf_alipay = 169,
        /// <summary>
        /// 速汇宝网银
        /// </summary>
        sfb_bank = 170,
        /// <summary>
        /// 速汇宝微信
        /// </summary>
        sfb_weixin = 171,
        /// <summary>
        /// 速汇宝支付宝
        /// </summary>
        sfb_alipay = 172,
        /// <summary>
        /// 天宝付QQ
        /// </summary>
        txf_qq = 173,
        /// <summary>
        /// 速汇宝qq
        /// </summary>
        sfb_qq = 174,
        /// <summary>
        /// 海鸥网银
        /// </summary>
        haio_bank = 175,
        /// <summary>
        /// 海鸥支付宝
        /// </summary>
        haio_alipay = 176,
        /// <summary>
        /// 海鸥微信
        /// </summary>
        haio_weixin = 177,
        /// <summary>
        /// 海鸥QQ
        /// </summary>
        haio_qq = 178,
        /// <summary>
        /// 中铁(通)QQ
        /// </summary>
        ZT_qq = 179,
        /// <summary>
        /// 合付宝网关
        /// </summary>
        hfb_bank = 180,
        /// <summary>
        /// 合付宝快捷
        /// </summary>
        hfb_express = 181,
        /// <summary>
        /// 天下付银联
        /// </summary>
        txf_upay = 184,
        /// <summary>
        /// 充值专员
        /// </summary>       
        czzy = 185,
        /// <summary>
        /// 艾付网关
        /// </summary>
        af_bank = 186,
        /// <summary>
        /// 艾付银联扫码
        /// </summary>
        af_upay = 187,
        /// <summary>
        /// 艾付微信
        /// </summary>
        af_weixin = 188,
        /// <summary>
        /// 艾付支付宝
        /// </summary>
        af_alipay = 189,
        /// <summary>
        /// 艾付qq
        /// </summary>
        af_qq = 190,
        /// <summary>
        /// 艾付H5QQ
        /// </summary>
        af_H5qqWap = 191,
        /// <summary>
        /// 艾付H5微信
        /// </summary>
        af_H5wxWap = 192,

        /// <summary>
        /// 顺利付qq
        /// </summary>
        slf_qq = 193,
        /// <summary>
        /// 顺利付银联
        /// </summary>
        slf_upay = 194,
        /// <summary>
        /// 好易微信扫码
        /// </summary>
        haoyi_weixin = 195,
        /// <summary>
        /// 好易支付宝扫码
        /// </summary>
        haoyi_alipay = 196,
        /// <summary>
        /// 好易触屏调用微信App支付
        /// </summary>
        haoyi_H5wxWap = 197,
        /// <summary>
        /// 好易触屏调用支付宝App支付
        /// </summary>
        haoyi_H5alipayWap = 198,

        /// <summary>
        /// 新付网银
        /// </summary>
        xinfu_bank = 199,
        /// <summary>
        /// 新付快捷
        /// </summary>
        xinfu_express = 200,
        /// <summary>
        /// 新付QQ
        /// </summary>
        xinfu_qq = 201,
        /// <summary>
        /// 新付微信
        /// </summary>
        xinfu_weixin = 202,
        /// <summary>
        /// 好易QQ
        /// </summary>
        haoyi_qq = 203,
        /// <summary>
        /// 好易快捷
        /// </summary>
        haoyi_express = 204,
        /// <summary>
        /// 神付快捷支付
        /// </summary>
        shenfu_express=205,
        /// <summary>
        /// 神付网银支付 
        /// </summary>
        shenfu_bank = 206,
        /// <summary>
        /// 神付微信支付
        /// </summary>
        shenfu_weixin = 207,
        /// <summary>
        /// 神付支付宝
        /// </summary>
        shenfu_alipay = 208,
        /// <summary>
        /// 神付QQ 钱包 
        /// </summary>
        shenfu_qq = 209,
        /// <summary>
        /// 神付银联
        /// </summary>
        shenfu_upay = 210
    }

    /// <summary>
    /// 充值状态
    /// </summary>
    public enum FillMoneyStatus
    {
        /// <summary>
        /// 请求中
        /// </summary>
        Requesting = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
        Failed = 2,
    }
    /// <summary>
    /// 提款代理商类型
    /// </summary>
    public enum WithdrawAgentType
    {
        /// <summary>
        /// 支付宝
        /// </summary>
        Alipay = 10,
        /// <summary>
        /// 易宝
        /// </summary>
        Yeepay = 20,
        /// <summary>
        /// 银行卡
        /// </summary>
        BankCard = 90,
        /// <summary>
        /// 银行卡_积分提现
        /// </summary>
        Integral_BankCard = 91,
        /// <summary>
        /// 银行卡_CPS返点提现
        /// </summary>
        CPS_BankCard = 100,
    }
    /// <summary>
    /// 提款状态
    /// </summary>
    public enum WithdrawStatus
    {
        /// <summary>
        /// 请求中
        /// </summary>
        Requesting = 1,
        /// <summary>
        /// 请求成功，但还没有返回
        /// </summary>
        Request = 2,
        /// <summary>
        /// 成功。已结算
        /// </summary>
        Success = 3,
        /// <summary>
        /// 失败。被拒绝
        /// </summary>
        Refused = 4,
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
        /// 订单分享送红包
        /// </summary>
        OrderRegister=7,
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
}
