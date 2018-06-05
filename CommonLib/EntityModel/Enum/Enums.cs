using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Enum
{  
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
        Requesting = 0,
        /// <summary>
        /// 成功
        /// </summary>
        Success = 1,
        /// <summary>
        /// 失败
        /// </summary>
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
        LuckyDraw = 112
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
}
