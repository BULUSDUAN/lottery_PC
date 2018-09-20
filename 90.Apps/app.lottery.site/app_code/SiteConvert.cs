using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using External.Core;
using GameBiz.Core;
using MatchBiz.Core;
using Common.Lottery;
using Common.Algorithms;

public static class SiteConvert
{
    public static string GetOZB_MatchName(string gameType, string match)
    {
        var gjArray = new string[] { "法国", "德国", "西班牙", "比利时", "英格兰", "葡萄牙", "意大利", "克罗地亚", "俄罗斯", "奥地利", "瑞士", "乌克兰", "波兰", "瑞典", "捷克", "土耳其", "罗马尼亚", "爱尔兰", "斯洛伐克", "威尔士", "冰岛", "匈牙利", "阿尔巴尼亚", "北爱尔兰" };
        var gyjArray = new string[] { "法国—德国", "法国—西班牙", "法国—英格兰", "法国—比利时", "法国—葡萄牙", "法国—意大利", "法国—克罗地亚", "法国—俄罗斯", "法国—奥地利", "法国—瑞士", "法国—乌克兰", "法国—波兰", "法国—瑞典", "法国—土耳其", "法国—罗马尼亚", "德国—西班牙", "德国—英格兰", "德国—比利时", "德国—葡萄牙", "德国—意大利", "德国—克罗地亚", "德国—俄罗斯", "德国—奥地利", "德国—瑞士", "德国—乌克兰", "德国—波兰", "德国—瑞典", "西班牙—英格兰", "西班牙—比利时", "西班牙—葡萄牙", "西班牙—意大利", "西班牙—克罗地亚", "西班牙—俄罗斯", "西班牙—奥地利", "西班牙—瑞士", "西班牙—乌克兰", "西班牙—波兰", "比利时—英格兰", "英格兰—葡萄牙", "英格兰—意大利", "英格兰—克罗地亚", "英格兰—俄罗斯", "英格兰—奥地利", "英格兰—瑞士", "比利时—葡萄牙", "比利时—意大利", "葡萄牙—意大利", "葡萄牙—克罗地亚", "葡萄牙—奥地利", "其它—其它" };
        var matchId = int.Parse(match);
        switch (gameType.ToUpper())
        {
            case "GJ":
            case "冠军":
                return gjArray[matchId - 1];
            case "GYJ":
            case "冠亚军":
                return gyjArray[matchId - 1];
            default:
                break;
        }
        return string.Empty;
    }

    public static string GetSJB_MatchName(string gameType, string match)
    {
        var gjArray = new string[] { "巴西", "德国", "西班牙", "阿根廷", "法国", "比利时", "葡萄牙", "英格兰", "乌拉圭", "哥伦比亚", "克罗地亚", "俄罗斯", "墨西哥", "波兰", 
                                    "瑞士", "丹麦", "塞尔维亚", "瑞典", "秘鲁", "日本", "尼日利亚", "塞内加尔", "埃及", "冰岛","突尼斯","澳大利亚","摩洛哥",
                                    "韩国","伊朗","哥斯达黎加","巴拿马","沙特"};
        var gyjArray = new string[] { "巴西—德国", "巴西—西班牙", "巴西—阿根廷", "巴西—法国", "巴西—比利时", "巴西—葡萄牙", "巴西—英格兰", "巴西—乌拉圭", 
                                    "巴西—哥伦比亚", "巴西—克罗地亚", "巴西—俄罗斯", "巴西—波兰", "巴西—瑞士", "德国—西班牙", "德国—阿根廷", "德国—法国", 
                                    "德国—比利时", "德国—葡萄牙", "德国—英格兰", "德国—乌拉圭", "德国—哥伦比亚", "德国—克罗地亚", "德国—俄罗斯", "西班牙—阿根廷", 
                                    "西班牙—法国", "西班牙—比利时", "西班牙—葡萄牙", "西班牙—英格兰", "西班牙—乌拉圭", "西班牙—哥伦比亚", "西班牙—克罗地亚", 
                                    "阿根廷—法国", "阿根廷—比利时", "阿根廷—葡萄牙", "阿根廷—英格兰", "阿根廷—乌拉圭", "阿根廷—哥伦比亚", "阿根廷—克罗地亚", 
                                    "法国—比利时", "法国—葡萄牙", "法国—英格兰", "法国—乌拉圭", "法国—哥伦比亚", "法国—克罗地亚", "比利时—葡萄牙", "比利时—英格兰", 
                                    "葡萄牙—英格兰", "葡萄牙—乌拉圭", "英格兰—乌拉圭", "其它" };
        var matchId = int.Parse(match);
        switch (gameType.ToUpper())
        {
            case "GJ":
            case "冠军":
                return gjArray[matchId - 1];
            case "GYJ":
            case "冠亚军":
                return gyjArray[matchId - 1];
            default:
                break;
        }
        return string.Empty;
    }
    //
    // GET: /SiteConvert/
    /// <summary>
    /// 把英文星期格式转换为中文格式
    /// </summary>
    /// <param name="week">Monday</param>
    /// <returns>星期一</returns>
    public static string Week_CN(DayOfWeek week)
    {
        var tmp = "";
        switch (week)
        {
            case DayOfWeek.Monday: tmp = "一";
                break;
            case DayOfWeek.Tuesday: tmp = "二";
                break;
            case DayOfWeek.Wednesday: tmp = "三";
                break;
            case DayOfWeek.Thursday: tmp = "四";
                break;
            case DayOfWeek.Friday: tmp = "五";
                break;
            case DayOfWeek.Saturday: tmp = "六";
                break;
            case DayOfWeek.Sunday: tmp = "日";
                break;
            default:
                tmp = week.ToString();
                break;
        }
        return "星期" + tmp;
    }

    /// <summary>
    /// 根据彩种编码（玩法类型）获取彩种（玩法）名称
    /// </summary>
    /// <param name="gamecode">彩种编码</param>
    /// <param name="type">玩法编码，可为空</param>
    /// <returns>彩种（玩法）名称</returns>
    public static string GameName(string gamecode, string type = "")
    {
        if (string.IsNullOrEmpty(gamecode))
        {
            return "";
        }
        type = string.IsNullOrEmpty(type) ? gamecode : type;
        //根据彩种编号获取彩种名称
        switch (gamecode.ToLower())
        {
            case "cqssc": return "重庆时时彩";
            case "jxssc": return "新时时彩";
            case "sd11x5": return "山东11选5";
            case "gd11x5": return "广东11选5";
            case "jx11x5": return "江西11选5";
            case "pl3": return "排列三";
            case "fc3d": return "福彩3D";
            case "ssq": return "双色球";
            case "qxc": return "七星彩";
            case "qlc": return "七乐彩";
            case "dlt": return "大乐透";
            case "sdqyh": return "群英会";
            case "gdklsf": return "广东快乐十分";
            case "gxklsf": return "广西快乐十分";
            case "jsks": return "江苏快3";
            case "sdklpk3": return "山东快乐扑克3";
            case "ozb": return "欧洲杯";
            case "sjb": return "世界杯";
            case "jczq":
                switch (type.ToLower())
                {
                    case "spf": return "竞彩让球胜平负";
                    case "brqspf": return "竞彩胜平负";
                    case "bf": return "竞彩比分";
                    case "zjq": return "竞彩总进球数";
                    case "bqc": return "竞彩半全场";
                    case "hh": return "足球混合过关";
                    default: return "竞彩足球";
                }
            case "jclq":
                switch (type.ToLower())
                {
                    case "sf": return "篮球胜负";
                    case "rfsf": return "篮球让分胜负";
                    case "sfc": return "篮球胜分差";
                    case "dxf": return "篮球大小分";
                    case "hh": return "篮球混合过关";
                    default: return "竞彩篮球";
                }
            case "ctzq":
                switch (type.ToLower())
                {
                    case "t14c": return "14场胜负";
                    case "tr9": return "任9场";
                    case "t6bqc": return "6场半全";
                    case "t4cjq": return "4场进球";
                    default: return "传统足球";
                }
            case "bjdc":
                switch (type.ToLower())
                {
                    case "sxds": return "单场上下单双";
                    case "spf": return "单场胜平负";
                    case "zjq": return "单场总进球";
                    case "bf": return "单场比分";
                    case "bqc": return "单场半全场";
                    default: return "北京单场";
                }
            default: return gamecode;
        }
    }
    /// <summary>
    /// 合买名人处彩种名称
    /// </summary>
    /// <param name="gamecode"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static string HMGameName(string gamecode, string type = "")
    {
        if (string.IsNullOrEmpty(gamecode))
        {
            return "";
        }
        type = string.IsNullOrEmpty(type) ? gamecode : type;
        //根据彩种编号获取彩种名称
        switch (gamecode.ToLower())
        {
            case "cqssc": return "重庆时时彩";
            case "jxssc": return "新时时彩";
            case "sd11x5": return "老11选5";
            case "gd11x5": return "新11选5";
            case "jx11x5": return "江西11选5";
            case "pl3": return "排列三";
            case "fc3d": return "福彩3D";
            case "ssq": return "双色球";
            case "qxc": return "七星彩";
            case "qlc": return "七乐彩";
            case "dlt": return "大乐透";
            case "sdqyh": return "群英会";
            case "gdklsf": return "快乐十分";
            case "gxklsf": return "广西快乐十分";
            case "jsks": return "江苏快3";
            case "jczq":
                switch (type.ToLower())
                {
                    case "spf": return "竞彩让球胜平负";
                    case "brqspf": return "竞彩胜平负";
                    case "bf": return "竞彩比分";
                    case "zjq": return "足球总进球";
                    case "bqc": return "竞彩半全场";
                    case "hh": return "足球混合";
                    default: return "竞彩足球";
                }
            case "jclq":
                switch (type.ToLower())
                {
                    case "sf": return "篮球让球";
                    case "rfsf": return "篮球不让分";
                    case "sfc": return "篮球胜分差";
                    case "dxf": return "篮球大小分";
                    case "hh": return "篮球混合";
                    default: return "竞彩篮球";
                }
            case "ctzq":
                switch (type.ToLower())
                {
                    case "t14c": return "14场胜负";
                    case "tr9": return "任9场";
                    case "t6bqc": return "6场半全";
                    case "t4cjq": return "4场进球";
                    default: return "传统足球";
                }
            case "bjdc":
                switch (type.ToLower())
                {
                    case "sxds": return "上下单双";
                    case "spf": return "单场胜平负";
                    case "zjq": return "单场总进球";
                    case "bf": return "单场比分";
                    case "bqc": return "单场半全场";
                    default: return "北京单场";
                }
            default: return gamecode;
        }
    }
    /// <summary>
    /// 转换充值接口类型名称
    /// </summary>
    /// <param name="fillAgent">充值接口类型</param>
    /// <returns>充值接口类型名称</returns>
    public static string FillAgentName(FillMoneyAgentType fillAgent)
    {
        //充值订单接口
        string agent = fillAgent.ToString();
        switch (fillAgent)
        {
            case FillMoneyAgentType.Alipay:
                agent = "支付宝";
                break;
            case FillMoneyAgentType.AlipayWAP:
                agent = "支付宝-WAP";
                break;
            case FillMoneyAgentType.CallsPay:
                agent = "手机充值卡";
                break;
            case FillMoneyAgentType.ChinaPay:
                agent = "网银在线";
                break;
            case FillMoneyAgentType.KuanQian:
                agent = "快钱支付";
                break;
            case FillMoneyAgentType.ManualDeduct:
                agent = "系统扣款";
                break;
            case FillMoneyAgentType.ManualFill:
                agent = "手工充值";
                break;
            case FillMoneyAgentType.Tenpay:
                agent = "财付通";
                break;
            case FillMoneyAgentType.Yeepay:
                agent = "***支付";//"易宝支付";
                break;
            case FillMoneyAgentType.IPS:
                agent ="***充值"; //"环迅充值";
                break;
            case FillMoneyAgentType.IPS_Bank:
                agent = "环迅网银";//"环迅网银";
                break;
            case FillMoneyAgentType.ZF_Bank:
                agent = "环迅网银";//"智付网银";
                break;
            case FillMoneyAgentType.HC_Bank:
                agent = "环迅网银";//"汇潮网银";
                break;
            case FillMoneyAgentType.HC_Quick:
                agent = "快捷支付";
                break;
            case FillMoneyAgentType.WXPay:
                agent = "微信支付";
                break;
            case FillMoneyAgentType.YS_Bank:
                agent = "***网银";//"银盛支付";
                break;
            case FillMoneyAgentType.YF_WEIXIN:
                agent = "微信(扫码)";
                break;
            case FillMoneyAgentType.HWAlipay:
                agent = "支付宝(扫码)";
                break;
            case FillMoneyAgentType.ZTPay:
                  agent = "支付宝(扫码)";
                break;
            case FillMoneyAgentType.ZT_Bank:
                agent = "***网银";//"中铁网银";
                break;
            case FillMoneyAgentType.HW_Bank:
                agent = "***网银";//"汇旺网银";
                break;
            case FillMoneyAgentType.HW_Quick:
                agent = "***快捷";//"汇旺快捷";
                break;
            case FillMoneyAgentType.ka101_express:
                agent = "***快捷";//"101卡快捷";
                break;
            case FillMoneyAgentType.ka101_bank:
                agent = "***网银";//"101卡网银";
                break;
            case FillMoneyAgentType.ka101_weixin:
                agent = "***微信扫码";//"101卡微信";
                break;
            case FillMoneyAgentType.ka101_alipay:
                agent = "***支付宝扫码";//"101卡支付宝";
                break;
            case FillMoneyAgentType.slf_alipay:
                agent = "***支付宝扫码";//"顺利付支付宝";
                break;
            case FillMoneyAgentType.slf_weixin:
                agent = "***微信扫码";//"顺利付微信";
                break;
            case FillMoneyAgentType.slf_express:
                agent = "***快捷";//"顺利付快捷";
                break;
            case FillMoneyAgentType.slf_bank:
                agent = "***网银";//"顺利付网银";
                break;
            case FillMoneyAgentType.mobao_express:
                agent ="***快捷";// "摩宝快捷";
                break;
            case FillMoneyAgentType.payworth_weixin:
                agent = "***微信扫码";//"华势微信";
                break;
            case FillMoneyAgentType.payworth_alipay:
                agent = "***支付宝扫码";//"华势支付宝";
                break;
            case FillMoneyAgentType.payworth_bank:
                agent = "***网银";//"华势网银";
                break;
            case FillMoneyAgentType.jhz_weixin:
                agent = "***微信扫码";//"金海哲微信";
                break;
            case FillMoneyAgentType.jhz_alipay:
                agent = "***支付宝扫码";//"金海哲支付宝";
                break;
            case FillMoneyAgentType.jhz_bank:
                agent = "***网银";//"金海哲网银";
                break;
            case FillMoneyAgentType.sandpay_bank:
                agent = "***网银";//"杉德网银";
                break;
            case FillMoneyAgentType.sandpay_express:
                agent ="***快捷";// "杉德快捷";
                break;
            case FillMoneyAgentType.sandpay_alipay:
                agent = "***支付宝扫码";//"杉德支付宝";
                break;
            case FillMoneyAgentType.sandpay_weixin:
                agent = "***微信扫码";//"杉德微信";
                break;
            case FillMoneyAgentType.duodebao_bank:
                agent ="***网银";// "多得宝网银";
                break;
            case FillMoneyAgentType.duodebao_weixin:
                agent = "***微信扫码";//"多得宝微信";
                break;
            case FillMoneyAgentType.duodebao_alipay:
                agent = "***支付宝扫码";//"多得宝支付宝";
                break;
            case FillMoneyAgentType.duodebao_qq:
                agent = "***QQ扫码";//"多得宝QQ";
                break;
            case FillMoneyAgentType.txf_bank:
                agent = "***网银";//"天下付网银";
                break;
            case FillMoneyAgentType.txf_weixin:
                agent ="***微信扫码";// "天下付微信";
                break;
            case FillMoneyAgentType.txf_alipay:
                agent = "***支付宝扫码";//"天下付支付宝";
                break;
            case FillMoneyAgentType.txf_qq:
                agent = "***QQ扫码";//"天下付QQ";
                break;
            case FillMoneyAgentType.sfb_bank:
                agent = "***网银";//"速汇宝网银";
                break;
            case FillMoneyAgentType.sfb_weixin:
                agent = "***微信扫码";//"速汇宝微信";
                break;
            case FillMoneyAgentType.sfb_alipay:
                agent = "***支付宝扫码";//"速汇宝支付宝";
                break;
            case FillMoneyAgentType.sfb_qq:
                agent = "***QQ扫码";//"速汇宝QQ";
                break;
            case FillMoneyAgentType.haio_bank:
                agent = "***网银";//"海鸥网银";
                break;
            case FillMoneyAgentType.haio_weixin:
                agent = "***微信扫码";//"海鸥微信";
                break;
            case FillMoneyAgentType.haio_alipay:
                agent = "***支付宝扫码";//"海鸥支付宝";
                break;
            case FillMoneyAgentType.haio_qq:
                agent = "***QQ扫码";//"海鸥QQ";
                break;
            //case FillMoneyAgentType.ZT_qq:
            //    agent = "***QQ扫码";//"中铁QQ";
            //    break;
            case FillMoneyAgentType.hfb_bank:
                agent = "**网关";
                break;
            case FillMoneyAgentType.hfb_express:
                agent = "***银联";
                break;
            case FillMoneyAgentType.af_bank:
                agent = "***网银";
                break;
            case FillMoneyAgentType.af_upay:
                agent = "***银联";
                break;
            case FillMoneyAgentType.af_alipay:
                agent = "***支付宝";
                break;
            case FillMoneyAgentType.af_weixin:
                agent = "***微信";
                break;
            case FillMoneyAgentType.af_qq:
                agent = "***qq";
                break;
            case FillMoneyAgentType.af_H5qqWap:
                agent = "***H5qq";
                break;
            case FillMoneyAgentType.af_H5wxWap:
                agent = "***H5微信";
                break;
            case FillMoneyAgentType.haoyi_weixin:
                agent = "***微信";
                break;
            case FillMoneyAgentType.haoyi_alipay:
                agent = "***支付宝";
                break;
            case FillMoneyAgentType.haoyi_H5wxWap:
                agent = "***H5微信";
                break;
            case FillMoneyAgentType.haoyi_H5alipayWap:
                agent = "***H5支付宝";
                break;
            case FillMoneyAgentType.xinfu_bank://新付网银
                agent = "***网银";
                break;
            case FillMoneyAgentType.xinfu_express://新付快捷
                agent = "***快捷";
                break;
            case FillMoneyAgentType.xinfu_qq://新付QQ
                agent = "***H5QQ";
                break;
            case FillMoneyAgentType.xinfu_weixin://新付微信
                agent = "***H5微信";
                break;
            case FillMoneyAgentType.shenfu_alipay:
                agent = "**支付宝";
                break;
            case FillMoneyAgentType.shenfu_weixin:
                agent = "**微信";
                break;
            case FillMoneyAgentType.shenfu_express:
                agent = "**快捷";
                break;
            case FillMoneyAgentType.shenfu_bank:
                agent = "**网银";
                break;
            case FillMoneyAgentType.shenfu_qq:
                agent = "**QQ";
                break;
            case FillMoneyAgentType.shenfu_upay:
                agent = "**银联";
                break;
            case FillMoneyAgentType.hfb_alipay:
                 agent = "**支付宝";
                break;
            case FillMoneyAgentType.ylt_express:
                agent = "**快捷";
                break;
            case FillMoneyAgentType.lyf_alipay:
                agent = "**支付宝";
                break;
            case FillMoneyAgentType.xinpay_bank:
                agent = "**网关";
                break;
            case FillMoneyAgentType.xinpay_express:
                agent = "**快捷";
                break;
            case FillMoneyAgentType.ysd_alipay:
                agent = "**支付宝";
                break;
            case FillMoneyAgentType.shayu_alipay:
                agent = "**支付宝";
                break;
            case FillMoneyAgentType.jht_alipay:
                agent = "**支付宝";
                break;
            case FillMoneyAgentType.jhz_upay:
                agent = "***银联";
                break;
            case FillMoneyAgentType.lagoufu_alipay:
                agent = "**支付宝";
                break;
            default:
                agent = fillAgent.ToString();
                break;
        }
        return agent;
    }
    /// <summary>
    /// 转换充值订单状态
    /// </summary>
    /// <param name="fillMoneyStatus">充值订单状态</param>
    /// <returns>充值订单状态显示名称</returns>
    public static string FillMoneyStatusName(FillMoneyStatus fillMoneyStatus)
    {
        switch (fillMoneyStatus)
        {
            case FillMoneyStatus.Success:
                return "已成功";
            case FillMoneyStatus.Failed:
                return "已失败";
            case FillMoneyStatus.Requesting:
                return "待付款";
            default:
                return fillMoneyStatus.ToString();
        }
    }
    /// <summary>
    /// 转换提现状态名称
    /// </summary>
    /// <param name="withdrawStatus">提现状态</param>
    /// <returns>提现状态名称</returns>
    public static string WithdrawStatusName(WithdrawStatus withdrawStatus)
    {
        //转换提现状态
        string status = string.Empty;
        switch (withdrawStatus)
        {
            case WithdrawStatus.Requesting:
                status = "请求中";
                break;
            case WithdrawStatus.Success:
                status = "已成功";
                break;
            case WithdrawStatus.Refused:
                status = "已拒绝";
                break;
            case WithdrawStatus.Request:
                status = "已提交银行代付";
                break;
            default:
                status = withdrawStatus.ToString();
                break;
        }
        return status;
    }
    /// <summary>
    /// 转换提款方式名称
    /// </summary>
    /// <param name="withdrawAgentType">提款方式</param>
    /// <returns>提款方式名称</returns>
    public static string WithdrawAgentTypeName(WithdrawAgentType withdrawAgentType)
    {
        //转换提款方式
        string typeName = string.Empty;
        switch (withdrawAgentType)
        {
            case WithdrawAgentType.Alipay:
                typeName = "支付宝";
                break;
            case WithdrawAgentType.Yeepay:
                typeName = "易宝支付";
                break;
            case WithdrawAgentType.BankCard:
                typeName = "银行卡";
                break;
            default:
                typeName = withdrawAgentType.ToString();
                break;
        }
        return typeName;
    }

    /// <summary>
    /// 意见反馈状态
    /// </summary>
    /// <param name="status"></param>
    /// <returns></returns>
    public static string FeebStatus(string status)
    {
        switch (status)
        {
            case "Request":
                return "未处理";
            case "Handle":
                return "已处理";
            default: return status;
        }
    }

    /// <summary>
    /// 转换过关方式名称
    /// </summary>
    /// <param name="playType">过关方式</param>
    /// <returns>过关方式名称</returns>
    public static string PlayTypeName(string playType)
    {
        var plays = playType.Replace("P", "").Replace("|", ",").Split(',');
        var tmp = new List<string>();
        foreach (var item in plays)
        {
            if (item == "1_1")
            {
                tmp.Add("单关");
            }
            else if (item == "0_1")
            {
                tmp.Add("混合过关");
            }
            else
            {
                tmp.Add(item.Replace("_", "串"));
            }
        }
        return string.Join(",", tmp);
    }

    /// <summary>
    /// 转换账户类型名称
    /// </summary>
    /// <param name="type">账户类型</param>
    /// <returns>账户类型名称</returns>
    public static string AccountTypeName(AccountType type)
    {
        switch (type)
        {
            //case AccountType.Activity:
            //    return "活动账户";
            case AccountType.Bonus:
                return "奖金账户";
            case AccountType.Commission:
                return "返点账户";
            //case AccountType.Common:
            //    return "通用账户";
            case AccountType.Freeze:
                return "冻结账户";
            case AccountType.FillMoney:
                return "充值账户";
            case AccountType.Experts:
                return "名家账户";
            case AccountType.RedBag:
                return "红包用户";
            default:
                return type.ToString();
        }
    }
    /// <summary>
    /// 转换投注方式名称
    /// </summary>
    /// <param name="schemeBettingCategory">投注方式类型</param>
    /// <returns>投注方式名称</returns>
    public static string SchemeBettingCategoryName(SchemeBettingCategory schemeBettingCategory)
    {
        switch (schemeBettingCategory)
        {
            case SchemeBettingCategory.GeneralBetting:
                return "普通投注";
            case SchemeBettingCategory.SingleBetting:
                return "单式上传";
            case SchemeBettingCategory.FilterBetting:
                return "过滤投注";
            case SchemeBettingCategory.YouHua:
                return "奖金优化投注";
            case SchemeBettingCategory.XianFaQiHSC:
                return "先发起后上传";
            case SchemeBettingCategory.ErXuanYi:
                return "2选1";
            case SchemeBettingCategory.YiChangZS:
                return "一场致胜";
            case SchemeBettingCategory.WinnerModel:
                return "赢家平台";
            case SchemeBettingCategory.HunHeDG:
                return "混合单关";  

            default:
                return "普通投注";
        }
    }
    /// <summary>
    /// 转换投注方式名称
    /// </summary>
    /// <param name="withdrawCategory">投注方式类型</param>
    /// <returns>投注方式名称</returns>
    public static string WithdrawCategoryName(WithdrawCategory withdrawCategory)
    {
        switch (withdrawCategory)
        {
            case WithdrawCategory.General:
                return "普通提现";
            case WithdrawCategory.Acceptable:
                return "可接受的提现";
            case WithdrawCategory.Compulsory:
                return "异常提款";
            case WithdrawCategory.Error:
                return "错误的提现";
            default:
                return withdrawCategory.ToString();
        }

    }
    /// <summary>
    /// 分析投注号码，返回注数
    /// </summary>
    /// <param name="gamecode"></param>
    /// <param name="type"></param>
    /// <param name="anteCode"></param>
    /// <returns></returns>
    public static int AnalyzeAnteCodeBetCount(string gamecode, string type, string anteCode)
    {
        var analyzer = AnalyzerFactory.GetAntecodeAnalyzer(gamecode, type);
        return analyzer.AnalyzeAnteCode(anteCode);
    }
    /// <summary>
    ///     投注方案类别
    /// </summary>
    /// <param name="schemeType"></param>
    /// <returns></returns>
    public static string SchemeTypeName(SchemeType schemeType)
    {
        switch (schemeType)
        {
            case SchemeType.GeneralBetting:
                return "普通投注";
            case SchemeType.ChaseBetting:
                return "追号投注";
            case SchemeType.TogetherBetting:
                return "合买投注";
            case SchemeType.ExperterScheme:
                return "专家方案";
            case SchemeType.SaveScheme:
                return "保存的订单";
            default:
                return schemeType.ToString();
        }
    }


    /// <summary>
    /// 资讯类型编码转换为中文名称
    /// </summary>
    /// <param name="category">资讯:INFO  技巧:TECH  预测:Expert</param>
    /// <returns>中文编码</returns>
    public static string ZX_Category(string category)
    {
        switch (category)
        {
            case "CPZJ":
                return "彩票中奖";
            case "HOT":
                return "热点彩讯";
            case "ZDZT":
                return "置顶主题";
            case "ZQDP":
                return "足球点评";
            case "LQDP":
                return "篮球点评";
            case "INFO":
                return "赛事资讯";
            case "SZZX":
                return "数字资讯";
            case "CPBK":
                return "彩票百科";
            default:
                return category;
        }
    }
    public static string TicketStatusName(TicketStatus ticket)
    {
        switch (ticket)
        {
            case TicketStatus.Abort:
                return "终止出票";
            case TicketStatus.Error:
                return "出票失败";
            case TicketStatus.Skipped:
                return "被跳过";
            case TicketStatus.Ticketed:
                return "出票成功";
            case TicketStatus.Ticketing:
                return "出票中";
            case TicketStatus.Waitting:
                return "等待出票";
            default:
                return ticket.ToString();

        }
    }
    public static string BonusStatusName(BonusStatus bonus)
    {
        switch (bonus)
        {
            case BonusStatus.Awarding:
                return "正在派奖";
            case BonusStatus.Error:
                return "出票失败";
            case BonusStatus.Lose:
                return "未中奖";
            case BonusStatus.Waitting:
                return "未开奖";
            case BonusStatus.Win:
                return "已中奖";
            default:
                return bonus.ToString();
        }

    }
    public static string JoinTypeName(TogetherJoinType jointype)
    {
        switch (jointype)
        {
            case TogetherJoinType.Subscription:
                return "方案预投";
            case TogetherJoinType.FollowerJoin:
                return "自动跟单";
            case TogetherJoinType.Join:
                return "参与合买";
            case TogetherJoinType.SystemGuarantees:
                return "系统保底";
            case TogetherJoinType.Guarantees:
                return "方案保底";
            default:
                return jointype.ToString();
        }
    }
    /// <summary>
    /// 模型类型
    /// </summary>
    /// <returns></returns>
    public static string ModelType(ModelType type)
    {
        switch (type)
        {
            case External.Core.ModelType.OptionalModel:
                return "自选模型";
            default:
                return "自选模型";
        }
    }

    //public static string GetTicketMinMoneyOrMaxMoney(string betContent, string betOdds)
    //{
    //    var oddstrArray = betOdds.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries);

    //    //投注内容
    //    var winMoneyList = new List<decimal>();
    //    var betContent2Array = SplitBetContentTo2Array(betContent);
    //    new ArrayCombination().Calculate(betContent2Array, (array) =>
    //    {
    //        var currentSp = 2M;
    //        foreach (var item in array)
    //        {
    //            var itemArray = item.Split(new string[] { "_" }, StringSplitOptions.RemoveEmptyEntries);
    //            if (itemArray.Length != 2)
    //                continue;
    //            var sp = GetMatchOdd(oddstrArray, itemArray[0], itemArray[1]);
    //            currentSp *= sp;
    //        }
    //        winMoneyList.Add(currentSp);
    //    });
    //    minMoney = winMoneyList.Min();
    //    maxMoney = winMoneyList.Max();
    //    return winMoneyList;
    //}

}