using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;
namespace KaSon.FrameWork.Helper
{
    public class ConvertHelper
    {
        
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public static int ConvertDateTimeInt(DateTime time)
        {
            DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        /// <summary>
        /// 删除Html
        /// </summary>
        public static string DeleteHtml(string htmlStream, int length = 0)
        {
            if (string.IsNullOrEmpty(htmlStream))
                return string.Empty;
            string strText = System.Text.RegularExpressions.Regex.Replace(htmlStream, "<[^>]+>", "");
            strText = System.Text.RegularExpressions.Regex.Replace(strText, "&[^;]+;", "");

            if (length > 0 && strText.Length > length)
                return strText.Substring(0, length);
            return strText.Replace(" ", "");
        }


        #region 竞彩编码解析

        private static Dictionary<string, string> GetAnteCodeDic(string[] array1, string[] array2)
        {
            Dictionary<string, string> dicAnteCode = new Dictionary<string, string>();
            for (int i = 0; i < array1.Length; i++)
            {
                for (int j = i; j < array2.Length; j++)
                {
                    dicAnteCode.Add(array1[i], array2[j]);
                    break;
                }
            }
            return dicAnteCode;
        }
        /// <summary>
        /// 北单投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_BJDC(string type, string code, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "spf":
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负";
                    case "zjq":
                        return GetAnteCodeDic("0,1,2,3,4,5,6,7".Split(','), "0,1,2,3,4,5,6,7+".Split(','))[code];
                    //return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
                    case "sxds":
                        return GetAnteCodeDic("SD,SS,XD,XS".Split(','), "上单,上双,下单,下双".Split(','))[code];
                    //return isCode ? "SD,SS,XD,XS" : "上单,上双,下单,下双";
                    case "bf":
                        return GetAnteCodeDic("00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,X0,XX,0X".Split(','), "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,胜其他,平其他,负其他".Split(','))[code];

                    //return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,胜其他,平其他,负其他";
                    case "bqc":
                        return GetAnteCodeDic("33,31,30,13,11,10,03,01,00".Split(','), "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负".Split(','))[code];
                    //return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
                    default:
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        //return isCode ? "3,1,0" : "胜,平,负";
                }
            }
            catch
            { return string.Empty; }
        }

        /// <summary>
        /// 竞彩足球投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_JCZQ(string type, string code, int letball = 0, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "spf":
                        //return isCode ? "3,1,0" : "胜,平,负";
                        if (letball == 0)
                            return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        else
                            return GetAnteCodeDic("3,1,0".Split(','), "让球胜,让球平,让球负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负,让球胜,让球平,让球负";
                    case "bqcspf":
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负";
                    case "zjq":
                        return GetAnteCodeDic("0,1,2,3,4,5,6,7".Split(','), "0,1,2,3,4,5,6,7+".Split(','))[code];
                    //return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
                    case "bf":
                        return GetAnteCodeDic("00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X".Split(','), "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他".Split(','))[code];
                    //return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他";
                    case "bqc":
                        return GetAnteCodeDic("33,31,30,13,11,10,03,01,00".Split(','), "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负".Split(','))[code];
                    //return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
                    case "hh":
                        return GetAnteCodeDic("3,1,0,00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X,0,1,2,3,4,5,6,7,33,31,30,13,11,10,03,01,00,3,1,0".Split(','), "让球胜,让球平,让球负,0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他,0,1,2,3,4,5,6,7+,胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负,胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0,00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X,0,1,2,3,4,5,6,7,33,31,30,13,11,10,03,01,00,3,1,0" : "让球胜,让球平,让球负,0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他,0,1,2,3,4,5,6,7+,胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负,胜,平,负";
                    default:
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        //return isCode ? "3,1,0" : "胜,平,负";
                }
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 竞彩篮球投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_JCLQ(string type, string code, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "sf":
                    case "rfsf":
                        return GetAnteCodeDic("3,0".Split(','), "主胜,客胜".Split(','))[code];
                    //return isCode ? "3,0" : "主胜,客胜";
                    case "sfc":
                        return GetAnteCodeDic("01,02,03,04,05,06,11,12,13,14,15,16".Split(','), "胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上".Split(','))[code];
                    //return isCode ? "01,02,03,04,05,06,11,12,13,14,15,16" : "胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上";
                    case "dxf":
                        return GetAnteCodeDic("3,0".Split(','), "大,小".Split(','))[code];
                    //return isCode ? "3,0" : "大,小";
                    case "hh":
                        return GetAnteCodeDic("3,0,3,0,01,02,03,04,05,06,11,12,13,14,15,16,3,0".Split(','), "主胜,客胜,主胜,客胜,胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上,大,小".Split(','))[code];
                    //return isCode ? "3,0,3,0,01,02,03,04,05,06,11,12,13,14,15,16,3,0" : "主胜,客胜,主胜,客胜,胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上,大,小";
                    default:
                        return GetAnteCodeDic("3,0".Split(','), "主胜,客胜".Split(','))[code];
                        //return isCode ? "3,0" : "主胜,客胜";
                }
            }
            catch { return string.Empty; }
        }

        /// <summary>
        /// 传统足球投注编码及显示码
        /// </summary>
        /// <param name="type">玩法类型</param>
        /// <param name="isCode">是否投注编码</param>
        /// <returns>返回投注编码或显示码</returns>
        public static string ANTECODES_CTZQ(string type, string code, bool isCode = false)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(type))
                return string.Empty;
            try
            {
                switch (type)
                {
                    case "t6bqc":
                    case "tr9":
                    case "t14c":
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                    //return isCode ? "3,1,0" : "胜,平,负";
                    case "t4cjq":
                        return GetAnteCodeDic("0,1,2,3".Split(','), "0,1,2,3+".Split(','))[code];
                    //return isCode ? "0,1,2,3" : "0,1,2,3+";
                    default:
                        return GetAnteCodeDic("3,1,0".Split(','), "胜,平,负".Split(','))[code];
                        //return isCode ? "3,1,0" : "胜,平,负";
                }
            }
            catch { return string.Empty; }
        }

        #endregion

        public static string GetAccountType(int type)
        {
            string str = string.Empty;
            switch (type)
            {
                case 10:
                    str = "奖金";
                    break;
                case 20:
                    str = "冻结";
                    break;
                case 30:
                    str = "佣金";
                    break;
                case 50:
                    str = "充值";
                    break;
                case 60:
                    str = "名家";
                    break;
                case 70:
                    str = "红包";
                    break;
            }
            return str;
        }
        public static string GetDomain()
        {
            return ConfigHelper.ConfigInfo["Domain"].ToString();
        }
        public static string GetFillMoneyAgentType(FillMoneyAgentType type)
        {
            string str = string.Empty;
            switch (type)
            {
                case FillMoneyAgentType.Alipay:
                    str = "支付宝";
                    break;
                case FillMoneyAgentType.AlipayWAP:
                    str = "支付宝-WAP支付";
                    break;
                case FillMoneyAgentType.Yeepay:
                    str = "易宝";
                    break;
                case FillMoneyAgentType.YingBao:
                    str = "银宝";
                    break;
                case FillMoneyAgentType.YiJiFu:
                    str = "易极付";
                    break;
                case FillMoneyAgentType.Tenpay:
                    str = "财付通";
                    break;
                case FillMoneyAgentType.KuanQian:
                    str = "快钱";
                    break;
                case FillMoneyAgentType.ChinaPay:
                    str = "银联语音支付";
                    break;
                case FillMoneyAgentType.CallsPay:
                    str = "手机充值卡支付";
                    break;
                case FillMoneyAgentType.ManualAdd:
                    str = "手工打款";
                    break;
                case FillMoneyAgentType.ManualDeduct:
                    str = "手工扣款";
                    break;
                case FillMoneyAgentType.ManualFill:
                    str = "手工充值";
                    break;
                case FillMoneyAgentType.BiFuBao:
                    str = "币付宝";
                    break;
            }
            return str;
        }
        public static string GetFillMoneyStatus(FillMoneyStatus status)
        {
            string str = string.Empty;
            switch (status)
            {
                case FillMoneyStatus.Success:
                    str = "已成功";
                    break;
                case FillMoneyStatus.Requesting:
                    str = "待付款";
                    break;
                case FillMoneyStatus.Failed:
                    str = "已失败";
                    break;
            }
            return str;
        }
        public static string GetSchemeBettingCategory(SchemeBettingCategory type)
        {
            string str = string.Empty;
            switch (type)
            {
                case SchemeBettingCategory.GeneralBetting:
                    str = "普通投注";
                    break;
                case SchemeBettingCategory.SingleBetting:
                    str = "单式上传";
                    break;
                case SchemeBettingCategory.FilterBetting:
                    str = "过滤投注";
                    break;
                case SchemeBettingCategory.YouHua:
                    str = "奖金优化投注";
                    break;
                case SchemeBettingCategory.XianFaQiHSC:
                    str = "先发起后上传";
                    break;
                case SchemeBettingCategory.ErXuanYi:
                    str = "主客二选一";
                    break;
                case SchemeBettingCategory.YiChangZS:
                    str = "一场致胜";
                    break;
            }
            return str;
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
        public static string GetWithdrawStatus(WithdrawStatus state)
        {
            switch (state)
            {
                case WithdrawStatus.Refused:
                    return "已失败";
                case WithdrawStatus.Success:
                    return "已成功";
                case WithdrawStatus.Requesting:
                    return "请求中";
            }
            return "";
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
                case "cqssc": return "时时彩";
                case "jxssc": return "新时时彩";
                case "sd11x5": return "老11选5";
                case "gd11x5": return "新11选5";
                case "jx11x5": return "11选5";
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
                case "sjb":
                    switch (type.ToLower())
                    {
                        case "gj": return "世界杯冠军";
                        case "gyj": return "世界杯冠亚军";
                        default: return "世界杯";
                    }
                case "ozb":
                    switch (type.ToLower())
                    {
                        case "gj": return "欧洲杯冠军";
                        case "gyj": return "欧洲杯冠亚军";
                        default: return "欧洲杯";
                    }
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
        /// 转换中奖显示名称
        /// </summary>
        /// <param name="bonusStatus">中奖状态</param>
        /// <returns>中奖显示名称</returns>
        public static string BonusStatusName(BonusStatus bonusStatus)
        {
            switch (bonusStatus)
            {
                case BonusStatus.Awarding: return "开奖中";
                case BonusStatus.Error: return "错误";
                case BonusStatus.Lose: return "未中奖";
                case BonusStatus.Waitting: return "未开奖";
                case BonusStatus.Win: return "已中奖";
                default: return string.Empty;
            }
        }
        /// <summary>
        /// 转换订单显示状态
        /// </summary>
        /// <param name="progressStatus">方案状态</param>
        /// <param name="ticketStatus">出票状态</param>
        /// <returns>显示状态</returns>
        public static string GetOrderStatusName(SchemeType schemeType, ProgressStatus progressStatus, TicketStatus ticketStatus = TicketStatus.Ticketed, BonusStatus bonusStatus = BonusStatus.Waitting, bool isPrizeMoney = false, bool isMine = true, bool isViturl = false)
        {
            if (schemeType == SchemeType.SaveScheme && isViturl && progressStatus == ProgressStatus.Waitting) return "待购买";  //如果是保存订单则表示为待购买
            if (isViturl) return "已撤单";  //如果是虚拟订单，则该订单为已撤单

            switch (progressStatus)
            {
                case ProgressStatus.Waitting: return "待开始";
                case ProgressStatus.AutoStop: return "自动停止";
                case ProgressStatus.Aborted: return "撤单";
                case ProgressStatus.Running:
                case ProgressStatus.Complate:
                    switch (ticketStatus)
                    {
                        case TicketStatus.Waitting: return "待投注";
                        case TicketStatus.Ticketing: return "投注中";
                        case TicketStatus.Ticketed:
                            if (progressStatus == ProgressStatus.Complate)
                            {
                                switch (bonusStatus)
                                {
                                    case BonusStatus.Waitting: return "未开奖";
                                    case BonusStatus.Awarding: return "开奖中";
                                    case BonusStatus.Error: return "开奖错误";
                                    case BonusStatus.Lose: return "未中奖";
                                    case BonusStatus.Win: return isPrizeMoney ? "已派奖" : "已中奖";
                                    default: return "已完成";
                                }
                            }
                            else
                            {
                                return (schemeType == SchemeType.TogetherBetting && !isMine ? "已跟单" : "已出票");
                            }
                        case TicketStatus.PrintTicket: return "已打票";
                        case TicketStatus.Skipped: return "被跳过";
                        case TicketStatus.Error: return "出票失败";
                        case TicketStatus.Abort: return "撤单";
                        default: return "待投注";
                    }
                default: return "待开始";
            }
        }
        /// <summary>
        /// 解析彩种为中文名称
        /// </summary>
        public static string FormatGameCode(string gameCode)
        {
            switch (gameCode)
            {
                case "BJDC":
                    return "北京单场";
                case "JCZQ":
                    return "竞彩足球";
                case "JCLQ":
                    return "竞彩篮球";
                case "CTZQ":
                    return "传统足球";
                case "SSQ":
                    return "双色球";
                case "DLT":
                    return "大乐透";
                case "FC3D":
                    return "福彩3D";
                case "PL3":
                    return "排列3";
                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
                case "CQSSC":
                    return "重庆时时彩";
                case "JX11X5":
                    return "江西11选五";
                case "SD11X5":
                    return "山东11选5";
                case "GD11X5":
                    return "广东11选5";
                case "GDKLSF":
                    return "广东快乐十分";
                case "JSKS":
                    return "江苏快三";
                case "SDKLPK3":
                    return "山东快乐扑克3";

            }
            return gameCode;
        }
        /// <summary>
        /// 解析玩法为中文名称
        /// </summary>
        public static string FormatGameType(string gameCode, string gameType)
        {
            var nameList = new List<string>();
            var typeList = gameType.Split(',', '|');
            foreach (var t in typeList)
            {
                nameList.Add(FormatGameType_Each(gameCode, t));
            }
            return string.Join(",", nameList.ToArray());
        }
        /// <summary>
        /// 隐藏用户名
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="showCount">显示长度</param>
        /// <returns>已隐藏部分用户名</returns>
        public static string HideUserName(string userName, int hideCount = 2)
        {
            if (string.IsNullOrEmpty(userName))
            {
                return "";
            }
            hideCount = hideCount > userName.Length ? userName.Length : hideCount;

            var regex = new System.Text.RegularExpressions.Regex("^1\\d{10}$");
            bool isMobile = regex.IsMatch(userName);
            if (isMobile)
                return EncodeMobile(userName);
            return EncodeString(userName, userName.Length - hideCount, userName.Length, hideCount);
        }
        public static string FormatGameType_Each(string gameCode, string gameType)
        {
            switch (gameCode)
            {
                #region 足彩

                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":
                            return "胜平负";
                        case "ZJQ":
                            return "总进球";
                        case "SXDS":
                            return "上下单双";
                        case "BF":
                            return "比分";
                        case "BQC":
                            return "半全场";
                    }
                    break;
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":
                            return "让球胜平负";
                        case "BRQSPF":
                            return "胜平负";
                        case "BF":
                            return "比分";
                        case "ZJQ":
                            return "总进球";
                        case "BQC":
                            return "半全场";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":
                            return "胜负";
                        case "RFSF":
                            return "让分胜负";
                        case "SFC":
                            return "胜分差";
                        case "DXF":
                            return "大小分";
                        case "HH":
                            return "混合过关";
                    }
                    break;
                case "CTZQ":
                    switch (gameType)
                    {
                        case "T14C":
                            return "14场胜负";
                        case "TR9":
                            return "胜负任9";
                        case "T6BQC":
                            return "6场半全场";
                        case "T4CJQ":
                            return "4场进球";
                    }
                    break;

                #endregion

                #region 重庆时时彩

                case "CQSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "3XHZ":    // 三星和值
                            return "三星和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XBAODAN":   // 二星组选包胆
                            return "二星组选包胆";
                        case "3XBAODAN":   // 三星组选包胆
                            return "三星组选包胆";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "3XBAODIAN":   // 三星组选包点
                            return "三星组选包点";
                        case "2XZXFS":   // 二星组选复式
                            return "二星组选复式";
                        case "2XZXFW":   // 二星组选分位
                            return "二星组选分位";
                        case "3XZXZH":   // 三星直选组合
                            return "三星直选组合";
                    }
                    break;

                #endregion

                #region 江西时时彩

                case "JXSSC":
                    switch (gameType)
                    {
                        case "1XDX":    // 一星单选
                            return "一星单选";
                        case "2XDX":    // 二星单选
                            return "二星单选";
                        case "3XDX":    // 三星直选
                            return "三星直选";
                        case "4XDX":
                            return "四星直选";
                        case "5XDX":    // 五星直选
                            return "五星直选";
                        case "5XTX":    // 五星通选
                            return "五星通选";
                        case "DXDS":    // 大小单双
                            return "大小单双";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                        case "2XHZ":    // 二星和值
                            return "二星和值";
                        case "2XBAODIAN":   // 二星组选包点
                            return "二星组选包点";
                        case "2XZX":   // 二星组选
                            return "二星组选";
                        case "RX1":   // 任选一
                            return "任选一";
                        case "RX2":   // 任选二
                            return "任选二";
                    }
                    break;

                #endregion

                #region 山东十一选五、广东十一选五、江西十一选五

                case "SD11X5":
                case "GD11X5":
                case "JX11X5":
                    switch (gameType)
                    {
                        case "RX1":
                            return "任选一";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "RX7":
                            return "任选七";
                        case "RX8":
                            return "任选八";
                        case "Q2ZHIX":
                            return "前二直选";
                        case "Q3ZHIX":
                            return "前三直选";
                        case "Q2ZUX":
                            return "前二组选";
                        case "Q3ZUX":
                            return "前三组选";
                    }
                    break;

                #endregion

                #region 广东快乐十分

                case "GDKLSF":
                    switch (gameType)
                    {
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "X1HT":
                            return "选一红投";
                        case "X1ST":
                            return "选一数投";
                        case "X2LZHI":
                            return "选二连直";
                        case "X2LZU":
                            return "选二连组";
                        case "X3QZHI":
                            return "选三连直";
                        case "X3QZU":
                            return "选三连组";
                    }
                    break;

                #endregion

                #region 江苏快三

                case "JSKS":
                    switch (gameType)
                    {
                        case "2BTH":
                            return "二不同号";
                        case "2BTHDT":
                            return "二不同号单选";
                        case "2THDX":
                            return "二同号单选";
                        case "2THFX":
                            return "二同号复选";
                        case "3BTH":
                            return "三不同号";
                        case "3BTHDT":
                            return "三不同号单选";
                        case "3LHTX":
                            return "三连号通选";
                        case "3THDX":
                            return "三同号单选";
                        case "3THTX":
                            return "三同号通选";
                        case "HZ":
                            return "和值";
                    }
                    break;

                #endregion

                #region 山东快乐扑克3

                case "SDKLPK3":
                    switch (gameType)
                    {
                        case "BZ":
                            return "豹子";
                        case "DZ":
                            return "对子";
                        case "RX1":
                            return "任选一";
                        case "RX2":
                            return "任选二";
                        case "RX3":
                            return "任选三";
                        case "RX4":
                            return "任选四";
                        case "RX5":
                            return "任选五";
                        case "RX6":
                            return "任选六";
                        case "SZ":
                            return "顺子";
                        case "TH":
                            return "同花";
                        case "THS":
                            return "同花顺";
                    }
                    break;

                #endregion


                #region 福彩3D、排列三

                case "FC3D":
                case "PL3":
                    switch (gameType)
                    {
                        case "FS":
                            return "复式";
                        case "HZ":
                            return "和值";
                        case "ZX3DS":   // 组三单式
                            return "组三单式";
                        case "ZX3FS":   // 组三复式
                            return "组三复式";
                        case "ZX6":     // 组选六
                            return "组选六";
                    }
                    break;

                #endregion

                #region 双色球

                case "SSQ":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                    }
                    break;

                #endregion

                #region 大乐透

                case "DLT":
                    switch (gameType)
                    {
                        case "DS":
                            return "单式";
                        case "FS":
                            return "复式";
                        case "DT":
                            return "胆拖";
                        case "12X2DS":
                            return "12生肖";
                        case "12X2FS":
                            return "12生肖";
                    }
                    break;


                #endregion

                case "JCSJBGJ":
                    return "世界杯冠军";
                case "JCYJ":
                    return "世界杯冠亚军";
            }
            return gameType;
        }
        /// <summary>
        /// 加密手机号，只显示前三位和后四位
        /// </summary>
        /// <param name="mobile">手机号码</param>
        /// <returns>加密后的手机号码</returns>
        public static string EncodeMobile(string mobile)
        {
            if (string.IsNullOrEmpty(mobile)) return string.Empty;
            return EncodeString(mobile, 3, mobile.Length - 4);
        }
        /// <summary>
        /// 加密字符串，隐藏指定位置用替换符代替
        /// </summary>
        /// <param name="objString">目标字符串</param>
        /// <param name="startIndex">起始隐藏位置</param>
        /// <param name="endIndex">结束隐藏位置</param>
        /// <param name="perch">替换符位数，默认3位</param>
        /// <param name="perchString">替换符号，默认*</param>
        /// <returns></returns>
        public static string EncodeString(string objString, int startIndex = 0, int endIndex = 0, int perch = 3, string perchString = "*")
        {
            if (endIndex > 0 && endIndex > startIndex && endIndex <= objString.Length)
            {
                var perchtmp = "";
                for (int i = 0; i < perch; i++)
                {
                    perchtmp += perchString;
                }
                return objString.Substring(0, startIndex) + perchtmp + objString.Substring(endIndex);
            }
            else
            {
                return objString;
            }
        }
        public static string FomartJoinType(TogetherJoinType t)
        {
            switch (t)
            {
                case TogetherJoinType.FollowerJoin:
                    return "跟单";
                case TogetherJoinType.Guarantees:
                    return "保底";
                case TogetherJoinType.Join:
                    return "参与";
                case TogetherJoinType.Subscription:
                    return "认购";
                case TogetherJoinType.SystemGuarantees:
                    return "系统保底";
                default:
                    break;
            }
            return string.Empty;
        }
    }
}
