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
        public static int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
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
            return "";
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
    }
}
