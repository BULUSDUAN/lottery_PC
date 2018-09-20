using System;
using System.Activities.Statements;
using System.Activities.Validation;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Web;
using System.Text;
using System.Configuration;
using Common.Lottery;
using Common.Lottery.OpenDataGetters;
using Common.XmlAnalyzer;
using app.lottery.site.Controllers;
using MatchBiz.Core;

/// <summary>
/// 站点字符串、地址类
/// </summary>
public static class SiteString
{
    #region 站点配置信息

    /// <summary>
    /// 返回当前站点根目录url地址
    /// </summary>
    /// <param name="request">HttpRequestBase对像</param>
    /// <returns>站点根目录url地址</returns>
    public static string root(HttpRequestBase request)
    {
        return Common.Net.UrlHelper.GetSiteRoot(request);
    }

    /// <summary>
    /// 网站版本号
    /// </summary>
    /// <returns></returns>
    public static string version
    {
        get
        {
            return WCFClients.GameClient.QueryCoreConfigByKey("Site.Version").ConfigValue;
        }
    }

    /// <summary>
    /// 网站副版本号
    /// </summary>
    /// <returns></returns>
    public static string vicever
    {
        get
        {
            return SettingConfigAnalyzer.GetConfigValueByKey("SiteVersion", "Vice");
        }
    }

    /// <summary>
    /// 站点资源目录地址，web.config配置项
    /// </summary>
    public static string res
    {
        get
        {
            try
            {
                return ConfigurationManager.AppSettings["ResourceSiteUrl"];
            }
            catch
            {
                return "http://localhost:887/iqucai";
            }
        }
    }

    /// <summary>
    /// 共享资源目录地址，web.config配置项
    /// </summary>
    public static string share
    {
        get
        {
            try
            {
                return ConfigurationManager.AppSettings["ResSiteUrl"] + "/share";
            }
            catch
            {
                return "http://localhost:887/share";
            }
        }
    }

    /// <summary>
    /// 根据图片名称获取本资源目录下的图片地址
    /// </summary>
    /// <param name="imgName">图片名称，可包含子图片目录路径</param>
    /// <returns>图片url地址</returns>
    public static string img(string imgName)
    {
        return res + "/images/" + imgName;
    }

    /// <summary>
    /// 在线客服链接
    /// </summary>
    public static string kefuUrl
    {
        get
        {
            try
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("Site.Service.KeFuUrl").ConfigValue;
            }
            catch
            {
                return "";
            }
        }
    }

    /// <summary>
    /// 网站客服电话
    /// </summary>
    public static string tel
    {
        get
        {
            try
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("Site.Service.Phone").ConfigValue;
            }
            catch
            {
                return "400-009-6569";
            }
        }
    }

    /// <summary>
    /// ICP备案号
    /// </summary>
    public static string icp
    {
        get
        {
            try
            {
                return WCFClients.GameClient.QueryCoreConfigByKey("Site.Service.ICP").ConfigValue;
            }
            catch
            {
                return "粤ICP备17016697号-1";
            }
        }
    }
    public static string icp2
    {
        get
        {
            try
            {
                return SettingConfigAnalyzer.GetConfigValueByKey("SiteService", "icp2");
            }
            catch
            {
                return "渝ICP备12003473号";
            }
        }
    }


    /// <summary>
    /// 获取第三方登录官网URL
    /// </summary>
    /// <param name="loginFrom">登录来源方式</param>
    /// <returns></returns>
    public static string getHZUrl(string loginFrom)
    {
        switch (loginFrom.ToLower())
        {
            case "alipay":
                return "http://www.alipay.com";
            case "qq":
                return "http://aq.qq.com";
            default:
                return "";
        }
    }

    /// <summary>
    /// 获取三方域名网站LOGO样式名称
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string getHZSiteLogo(HttpRequestBase request)
    {
        try
        {
            var host = request.Url.Host;
            return AgentHostMappingConfigAnalyzer.GetSiteLogoByHostName(host);
        }
        catch
        {
            return "";
        }
    }

    /// <summary>
    /// 获取网站名字
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string getHZSiteName(HttpRequestBase request)
    {
        try
        {
            //var host = request.Url.Host;
            //var name = AgentHostMappingConfigAnalyzer.GetSiteNameByHostName(host);
            //return string.IsNullOrEmpty(name) ? "玩彩网" : name;
            return ConfigurationManager.AppSettings["host"] != null ? ConfigurationManager.AppSettings["host"].ToString() : "玩彩网";
        }
        catch
        {
            return "玩彩网";
        }
    }

    public static string getHZSiteName()
    {
        return "玩彩网";
    }

    /// <summary>
    /// 获取网站的网址
    /// </summary>
    /// <param name="request"></param>
    /// <returns></returns>
    public static string getHZSiteUrl(HttpRequestBase request)
    {
        try
        {
            return "http://www.wancai.com";
        }
        catch
        {
            return "http://www.wancai.com";
        }
    }

    public static string getHZSiteUrl()
    {
        return "http://www.wancai.com";
    }

    private static Dictionary<string, string> headFootParamDic = new Dictionary<string, string>();
    /// <summary>
    /// 获取头部和底部后缀
    /// </summary>
    public static string getHeadFootParam(HttpRequestBase request)
    {
        try
        {
            //return "_3145";
            if (headFootParamDic.Count == 0 || !headFootParamDic.ContainsKey(request.Url.Host))
            {
                //取配置
                var nodes = SettingConfigAnalyzer.GetConfigXElementByKey("WebHeadFoot").Elements("Web");
                var de = nodes.FirstOrDefault(p => p.Attribute("Domain").Value == "Default");
                var currentParam = de.Attribute("Params").Value;
                var currentNode = nodes.FirstOrDefault(p => p.Attribute("Domain").Value == request.Url.Host);
                if (currentNode != null)
                    currentParam = currentNode.Attribute("Params").Value;

                headFootParamDic.Add(request.Url.Host, currentParam);
            }
            //if (headFootParamDic.ContainsKey(request.Url.Host))
            return headFootParamDic[request.Url.Host];
            //return string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }

    public static string getApk_dowurl
    {
        //UpdateUrl = string.Format("{0}/"+“GSAPP”+"_{1}.apk", downLoadUrl, version),

        get
        {
            try
            {
                return WCFClients.GameClient.QueryAppConfigByAgentId("").ConfigDownloadUrl;
            }
            catch
            {
                return "";
            }
        }
    }

    public static string getApk_version
    {
        get
        {
            try
            {
                return WCFClients.GameClient.QueryAppConfigByAgentId("").ConfigVersion;
            }
            catch (Exception)
            {

                return "v1.0.0";
            }
        }
    }

    public static string getGSApk_version()
    {
        try
        {
            //return WCFClients.GameClient.QueryCoreConfigByKey("GSAPP.Version").ConfigValue;
            return WCFClients.GameClient.QueryAppConfigByAgentId("").ConfigVersion;

        }
        catch (Exception)
        {

            return "v1.0";
        }
    }
    public static string getIOS_version()
    {
        try
        {
            var version = WCFClients.GameClient.QueryAppConfigByAgentId("100001").ConfigVersion;
            return version;

        }
        catch (Exception)
        {

            return "V1.10";
        }
    }

    public static string getApk_Url()
    {
        try
        {
            var ConfigInfo = WCFClients.GameClient.QueryAppConfigByAgentId("");
            return string.Format("{0}/" + "{1}" + "_{2}.apk", ConfigInfo.ConfigDownloadUrl, ConfigInfo.AgentName, ConfigInfo.ConfigVersion);
        }
        catch (Exception)
        {
            return "";
        }
    }

    public static string getIOS_Url()
    {
        try
        {
            var url = WCFClients.GameClient.QueryCoreConfigByKey("IOSDownloadUrl").ConfigValue;
            return url;
        }
        catch (Exception)
        {
            return "";
        }
    }

    /// <summary>
    /// 获取支付宝图片路径（qucai/iqucai）
    /// </summary>
    /// <returns></returns>
    public static string getAlipayWay
    {
        get
        {
            string defalutValue = ConfigurationManager.AppSettings["alipayWay"];
            try
            {
                var key = WCFClients.GameClient.QueryCoreConfigByKey("QCW_Alipay");
                if (string.IsNullOrEmpty(key.ConfigValue))
                {
                    return defalutValue;
                }
                else
                {
                    return key.ConfigValue;
                }
            }
            catch (Exception)
            {
                return defalutValue;
            }
        }
    }

    #endregion

    #region 彩种相关信息
    /// <summary>
    /// 彩种分类字符串数组
    /// </summary>
    public static string[] gameList(string type)
    {
        switch (type.ToLower())
        {
            case "gpc":
                return new string[] { "cqssc", "jxssc", "sd11x5", "gd11x5", "jx11x5" };
            case "jjc":
                return new string[] { "jczq", "jclq", "bjdc", "ctzq" };
            case "szc":
                return new string[] { "ssq", "dlt", "fc3d", "pl3" };
            case "dpc":
                return new string[] { "ssq", "dlt" };
            case "day":
                return new string[] { "fc3d", "pl3" };
            default:
                return new string[] { };
        }
    }

    /// <summary>
    /// 获取彩种开奖日期
    /// </summary>
    /// <param name="gameCode"></param>
    /// <returns></returns>
    public static string lotteryDay(string gameCode)
    {
        var ret = "不定期";
        switch (gameCode.ToLower())
        {
            case "cqssc":
            case "jsks":
            case "jxssc":
            case "gdklsf":
                ret = "10分钟一期";
                break;
            case "sd11x5":
            case "gd11x5":
            case "jx11x5":
                ret = "12分钟一期";
                break;
            case "sdqyh":
                ret = "15分钟一期";
                break;
            case "fc3d":
            case "pl3":
                ret = "每日开奖";
                break;
            case "ssq":
                ret = "二 四 日";
                break;
            case "dlt":
                ret = "一 三 六";
                break;
        }
        return ret;
    }

    /// <summary>
    /// 战绩计算金额
    /// </summary>
    /// <returns></returns>
    public static int record_money
    {
        get
        {
            try
            {
                return int.Parse(WCFClients.GameClient.QueryCoreConfigByKey("Site.Beedings.RecordMoney").ConfigValue);
            }
            catch (Exception)
            {
                return 1000;
            }
        }
    }

    /// <summary>
    /// 网站保底百分比配置信息
    /// </summary>
    /// <returns></returns>
    public static int sitesafety(string gameCode)
    {
        try
        {
            var res = WCFClients.GameClient.QueryCoreConfigByKey("Site.Together.SystemGuarantees").ConfigValue;
            return int.Parse(res);
        }
        catch (Exception)
        {
            return 10;
        }
    }

    /// <summary>
    /// 彩种奖池信息
    /// </summary>
    /// <param name="service">HttpServerUtilityBase对象</param>
    /// <param name="game">彩种</param>
    /// <param name="issuse">期号</param>
    /// <returns>奖池信息</returns>
    public static Common.Business.OpenDataInfo GameOpenData(HttpServerUtilityBase service, string game, string issuse)
    {
        //return Json_SZC.GetOpenData(service, game, issuse);
        return null;
    }
    #endregion

    #region 字符串加密
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

    /// <summary>
    /// 加密手机号，只显示前三位和后四位
    /// </summary>
    /// <param name="mobile">手机号码</param>
    /// <returns>加密后的手机号码</returns>
    public static string EncodeMobile(string mobile)
    {
        if (string.IsNullOrEmpty(mobile)) return string.Empty;
        return EncodeString(mobile, 3, mobile.Length - 3, 5);
    }

    /// <summary>
    /// 加密身份证号码，只显示前五位和后四位
    /// </summary>
    /// <param name="mobile">身份证号码</param>
    /// <returns>加密后的身份证号码</returns>
    public static string EncodeIdCardNumber(string idCard)
    {
        if (string.IsNullOrEmpty(idCard)) return string.Empty;
        return EncodeString(idCard, 5, idCard.Length - 4);
    }

    /// <summary>
    /// 加密QQ号码，只显示前面1位和后面两位，中间隐藏
    /// </summary>
    public static string EncodeQQNumber(string qq)
    {
        var length = qq.Length;
        if (length <= 3) return qq;
        var showStr = qq.Substring(0, 1);
        for (int i = 0; i < length - 3; i++)
        {
            showStr += "*";
        }
        showStr += qq.Substring(length - 2);
        return showStr;
    }

    /// <summary>
    /// 加密银行卡号，隐藏最后6位
    /// </summary>
    /// <param name="mobile">银行卡号</param>
    /// <returns>加密后的银行卡号</returns>
    public static string EncodeBankCardNumber(string bankCard)
    {
        if (string.IsNullOrEmpty(bankCard)) return string.Empty;
        return EncodeString(bankCard, bankCard.Length - 6, bankCard.Length - 1);
    }

    /// <summary>
    /// 加密邮箱地址，只显示邮箱来源及一部分自定义地址
    /// </summary>
    /// <param name="mobile">邮箱地址</param>
    /// <returns>加密后的邮箱地址</returns>
    public static string EncodeEmail(string email)
    {
        var emailArray = email.Split('@');
        if (emailArray.Length > 1)
        {
            return EncodeString(emailArray[0], 3, emailArray[0].Length) + "@" + emailArray[1];
        }
        else
        {
            return email;
        }
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
    public static string ActivityHideUserName(string userName, int hideCount = 2)
    {
        if (string.IsNullOrEmpty(userName))
        {
            return "";
        }
        var perchtmp = "***";
        return userName.Substring(0, 1) + perchtmp;
    }
    /// <summary>
    /// 隐藏用户名-全站通用
    /// </summary>
    /// <param name="userName">用户名</param>
    /// <param name="showCount">显示长度</param>
    /// <returns>已隐藏部分用户名</returns>
    public static string AllHideUser(string userName, int showCuont = 2)
    {
        try
        {
            return EncodeString(userName, showCuont, userName.Length);
        }
        catch
        {
            return userName;
        }

    }
    #endregion

    #region 竞彩编码
    /// <summary>
    /// 北单投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_BJDC(string type, bool isCode = false)
    {
        switch (type)
        {
            case "spf":
                return isCode ? "3,1,0" : "胜,平,负";
            case "zjq":
                return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
            case "sxds":
                return isCode ? "SD,SS,XD,XS" : "上单,上双,下单,下双";
            case "bf":
                return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,胜其他,平其他,负其他";
            case "bqc":
                return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
            default:
                return isCode ? "3,1,0" : "胜,平,负";
        }
    }

    /// <summary>
    /// 竞彩足球投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_JCZQ(string type, bool isCode = false)
    {
        switch (type)
        {
            case "spf":
                //return isCode ? "3,1,0" : "胜,平,负";
                return isCode ? "3,1,0" : "胜,平,负,让球胜,让球平,让球负";
            case "bqcspf":
                return isCode ? "3,1,0" : "胜,平,负";
            case "zjq":
                return isCode ? "0,1,2,3,4,5,6,7" : "0,1,2,3,4,5,6,7+";
            case "bf":
                return isCode ? "00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X" : "0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他";
            case "bqc":
                return isCode ? "33,31,30,13,11,10,03,01,00" : "胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负";
            case "hh":
                return isCode ? "3,1,0,00,01,02,03,10,11,12,13,20,21,22,23,30,31,32,33,40,41,42,04,14,24,50,51,52,05,15,25,X0,XX,0X,0,1,2,3,4,5,6,7,33,31,30,13,11,10,03,01,00,3,1,0" : "让球胜,让球平,让球负,0:0,0:1,0:2,0:3,1:0,1:1,1:2,1:3,2:0,2:1,2:2,2:3,3:0,3:1,3:2,3:3,4:0,4:1,4:2,0:4,1:4,2:4,5:0,5:1,5:2,0:5,1:5,2:5,胜其他,平其他,负其他,0,1,2,3,4,5,6,7+,胜胜,胜平,胜负,平胜,平平,平负,负胜,负平,负负,胜,平,负";
            default:
                return isCode ? "3,1,0" : "胜,平,负";
        }
    }

    /// <summary>
    /// 竞彩篮球投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_JCLQ(string type, bool isCode = false)
    {
        switch (type)
        {
            case "sf":
            case "rfsf":
                return isCode ? "3,0" : "主胜,客胜";
            case "sfc":
                return isCode ? "01,02,03,04,05,06,11,12,13,14,15,16" : "胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上";
            case "dxf":
                return isCode ? "3,0" : "大,小";
            case "hh":
                return isCode ? "3,0,3,0,01,02,03,04,05,06,11,12,13,14,15,16,3,0" : "主胜,客胜,主胜,客胜,胜1-5分,胜6-10分,胜11-15分,胜16-20分,胜21-25分,胜26分以上,负1-5分,负6-10分,负11-15分,负16-20分,负21-25分,负26分以上,大,小";
            default:
                return isCode ? "3,0" : "主胜,客胜";
        }
    }

    /// <summary>
    /// 传统足球投注编码及显示码
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="isCode">是否投注编码</param>
    /// <returns>返回投注编码或显示码</returns>
    public static string ANTECODES_CTZQ(string type, bool isCode = false)
    {
        switch (type)
        {
            case "t6bqc":
            case "tr9":
            case "t14c":
                return isCode ? "3,1,0" : "胜,平,负";
            case "t4cjq":
                return isCode ? "0,1,2,3" : "0,1,2,3+";
            default:
                return isCode ? "3,1,0" : "胜,平,负";
        }
    }

    /// <summary>
    /// 根据投注编码显示编码名称
    /// </summary>
    /// <param name="type">玩法类型</param>
    /// <param name="value">投注编码</param>
    /// <returns>显示编码</returns>
    public static string ANTECODES_NAME(string type, string value)
    {
        try
        {
            return SportAnalyzer.GetMatchResultDisplayName(type, value);
        }
        catch
        {
            return value;
        }
    }
    public static string GameType_NAME(string code, string type)
    {
        switch (code.ToLower())
        {
            case "bjdc":
                switch (type.ToLower())
                {
                    case "spf":
                        return "胜平负";
                    case "zjq":
                        return "总进球";
                    case "sxds":
                        return "上下单双";
                    case "bf":
                        return "比分";
                    case "bqc":
                        return "半全场";
                    default:
                        return "";
                }
                break;
            case "jczq":
                switch (type.ToLower())
                {
                    case "spf":
                        return "让球胜平负";
                    case "brqspf":
                        return "胜平负";
                    case "zjq":
                        return "总进球";
                    case "bqc":
                        return "半全场";
                    case "bf":
                        return "比分";
                    default:
                        return "";

                }
                break;
            default: return null;
        }

    }


    /// <summary>
    /// 单式上传截取内容长度（每个号码长度）
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int SingleNum(string gameCode, string type)
    {
        switch (gameCode.ToLower())
        {
            case "bjdc":
                switch (type.ToLower())
                {
                    case "spf":
                    case "zjq":
                    case "sxds":
                        return 1;
                    case "bf":
                    case "bqc":
                        return 2;
                    default:
                        return 1;
                }
                break;
            case "jczq":
                switch (type.ToLower())
                {
                    case "spf":
                    case "brqspf":
                    case "zjq":
                        return 1;
                    case "bf":
                    case "bqc":
                        return 2;
                    default:
                        return 1;
                }
                break;
            default:
                return 1;

        }

    }
    /// <summary>
    /// 将buffer内容转换成统一的格式
    /// </summary>
    /// <param name="codearr"></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static List<string> CheckSingleSchemeAnteCode(string[] codearr, string type, string gameCode)
    {
        List<string> strList = new List<string>();
        var numberLength = SingleNum(gameCode, type);
        for (int i = 0; i < codearr.Count(); i++)
        {
            if (codearr[i].Contains(","))
            {
                strList.Add(codearr[i]);
            }
            else
            {
                List<string> tmps = new List<string>();
                for (int j = 0; j < codearr[i].Length; j += numberLength)
                {
                    tmps.Add(codearr[i].Substring(j, numberLength));
                }
                strList.Add(string.Join(",", tmps));
            }
        }
        return strList;
    }
    /// <summary>
    /// 验证每场比赛是否包含场次
    /// </summary>
    /// <returns></returns>
    public static string CheckContainsMatch(string codestr)
    {
        if (codestr.Contains("→"))
        {
            return codestr.Split('→')[1];
        }
        else
        {
            return codestr;
        }
    }
    /// <summary>
    /// 得到对应的投注号码sp
    /// </summary>
    /// <param name="code"></param>
    /// <param name="sp"></param>
    /// <returns></returns>
    public static decimal CheckSp(string code, string sp)
    {
        var spstr = 0M;
        if (!string.IsNullOrEmpty(sp))
        {
            var splist = sp.Split(',');
            foreach (var item in splist)
            {
                var spcode = item.Split('|')[0];
                if (spcode == code) spstr = decimal.Parse(item.Split('|')[1]);
            }
        }
        return spstr;
    }

    public static string ChangeAnteCode(string code)
    {
        var str = code;
        switch (code)
        {
            case "3":
                str = "XS";
                break;
            case "2":
                str = "XD";
                break;
            case "1":
                str = "SS";
                break;
            case "0":
                str = "SD";
                break;
            default:
                return code;

        }
        return str;
    }
    public static string CqsscCode(string code)
    {
        var str = code;
        switch (code)
        {
            case "2":
                str = "大";
                break;
            case "1":
                str = "小";
                break;
            case "5":
                str = "单";
                break;
            case "4":
                str = "双";
                break;
            default:
                return code;

        }
        return str;
    }

    public static string SDKLPK3(string gameType, string anteCode)
    {
        var sdklpk3 = new List<string>();
        if (gameType.ToUpper() == "TH")
        {
            var codes = anteCode.Split(',');
            foreach (var code in codes)
            {
                //同花
                switch (code)
                {
                    case "TH1":
                        sdklpk3.Add("同花黑桃");
                        break;
                    case "TH2":
                        sdklpk3.Add("同花红桃");
                        break;
                    case "TH3":
                        sdklpk3.Add("同花梅花");
                        break;
                    case "TH4":
                        sdklpk3.Add("同花方块");
                        break;
                    case "THX":
                        sdklpk3.Add("同花包选");
                        break;
                    default:
                        break;
                }
            }
            return string.Join(",", sdklpk3.ToArray());
        }
        else if (gameType.ToUpper() == "SZ")
        {
            var codes = anteCode.Split(',');
            foreach (var code in codes)
            {
                switch (code)
                {
                    case "SZ1":
                        sdklpk3.Add("A,2,3");
                        break;
                    case "SZ2":
                        sdklpk3.Add("2,3,4");
                        break;
                    case "SZ3":
                        sdklpk3.Add("3,4,5");
                        break;
                    case "SZ4":
                        sdklpk3.Add("4,5,6");
                        break;
                    case "SZ5":
                        sdklpk3.Add("5,6,7");
                        break;
                    case "SZ6":
                        sdklpk3.Add("6,7,8");
                        break;
                    case "SZ7":
                        sdklpk3.Add("7,8,9");
                        break;
                    case "SZ8":
                        sdklpk3.Add("8,9,10");
                        break;
                    case "SZ9":
                        sdklpk3.Add("9,10,J");
                        break;
                    case "SZ10":
                        sdklpk3.Add("10,J,Q");
                        break;
                    case "SZ11":
                        sdklpk3.Add("J,Q,K");
                        break;
                    case "SZ12":
                        sdklpk3.Add("Q,K,A");
                        break;
                    case "SZX":
                        sdklpk3.Add("顺子包选");
                        break;
                    default:
                        break;
                }

            }
            return string.Join(" ", sdklpk3.ToArray());
        }
        else if (gameType.ToUpper() == "THS")
        {
            var codes = anteCode.Split(',');
            foreach (var code in codes)
            {
                switch (code)
                {
                    case "THS1":
                        sdklpk3.Add("同花顺黑桃");
                        break;
                    case "THS2":
                        sdklpk3.Add("同花顺红桃");
                        break;
                    case "THS3":
                        sdklpk3.Add("同花顺梅花");
                        break;
                    case "THS4":
                        sdklpk3.Add("同花顺方块");
                        break;
                    case "THSX":
                        sdklpk3.Add("同花顺包选");
                        break;
                    default:
                        break;
                }
            }
            return string.Join(",", sdklpk3.ToArray());
        }
        else if (gameType.ToUpper() == "DZ")
        {
            var codes = anteCode.Split(',');
            foreach (var code in codes)
            {
                switch (code)
                {
                    case "DZ1":
                        sdklpk3.Add("A,A");
                        break;
                    case "DZ2":
                        sdklpk3.Add("2,2");
                        break;
                    case "DZ3":
                        sdklpk3.Add("3,3");
                        break;
                    case "DZ4":
                        sdklpk3.Add("4,4");
                        break;
                    case "DZ5":
                        sdklpk3.Add("5,5");
                        break;
                    case "DZ6":
                        sdklpk3.Add("6,6");
                        break;
                    case "DZ7":
                        sdklpk3.Add("7,7");
                        break;
                    case "DZ8":
                        sdklpk3.Add("8,8");
                        break;
                    case "DZ9":
                        sdklpk3.Add("9,9");
                        break;
                    case "DZ10":
                        sdklpk3.Add("10,10");
                        break;
                    case "DZ11":
                        sdklpk3.Add("J,J");
                        break;
                    case "DZ12":
                        sdklpk3.Add("Q,Q");
                        break;
                    case "DZ13":
                        sdklpk3.Add("K,K");
                        break;
                    case "DZX":
                        sdklpk3.Add("对子包选");
                        break;
                    default:
                        break;
                }
            }
            return string.Join(" ", sdklpk3.ToArray());
        }
        else if (gameType.ToUpper() == "BZ")
        {
            var codes = anteCode.Split(',');
            foreach (var code in codes)
            {
                switch (code)
                {
                    case "BZ1":
                        sdklpk3.Add("A,A,A");
                        break;
                    case "BZ2":
                        sdklpk3.Add("2,2,2");
                        break;
                    case "BZ3":
                        sdklpk3.Add("3,3,3");
                        break;
                    case "BZ4":
                        sdklpk3.Add("4,4,4");
                        break;
                    case "BZ5":
                        sdklpk3.Add("5,5,5");
                        break;
                    case "BZ6":
                        sdklpk3.Add("6,6,6");
                        break;
                    case "BZ7":
                        sdklpk3.Add("7,7,7");
                        break;
                    case "BZ8":
                        sdklpk3.Add("8,8,8");
                        break;
                    case "BZ9":
                        sdklpk3.Add("9,9,9");
                        break;
                    case "BZ10":
                        sdklpk3.Add("10,10,10");
                        break;
                    case "BZ11":
                        sdklpk3.Add("J,J,J");
                        break;
                    case "BZ12":
                        sdklpk3.Add("Q,Q,Q");
                        break;
                    case "BZ13":
                        sdklpk3.Add("K,K,K");
                        break;
                    case "BZX":
                        sdklpk3.Add("豹子包选");
                        break;
                    default:
                        break;
                }
            }
            return string.Join(" ", sdklpk3.ToArray());
        }
        else
        {
            return anteCode;
        }
    }

    public static string SDKLPK3WinColor(string n)
    {
        var s_c = n.Substring(0, 1);
        var s = "";
        switch (s_c)
        {
            case "1":
                s = "♠";
                break;
            case "2":
                s = "♥";
                break;
            case "3":
                s = "♣";
                break;
            case "4":
                s = "♦";
                break;
        }

        if (s_c == "2" || s_c == "4")
        {
            return string.Format("<em style=\"font-size:16px;color:red;\">{0}</em>", s);
        }
        else
        {
            return string.Format("<em style=\"font-size:16px;color:black;\">{0}</em>", s);
        }
    }

    public static string SDKLPK3WinNumber(string n)
    {
        var s_c = n.Substring(0, 1);
        var s = "";
        switch (s_c)
        {
            case "1":
                s = "♠";
                break;
            case "2":
                s = "♥";
                break;
            case "3":
                s = "♣";
                break;
            case "4":
                s = "♦";
                break;
        }
        var n_c = int.Parse(n.Substring(1));
        var ns = "";
        switch (n_c)
        {
            case 1:
                ns = "A";
                break;
            case 11:
                ns = "J";
                break;
            case 12:
                ns = "Q";
                break;
            case 13:
                ns = "K";
                break;
            default:
                ns = n_c.ToString();
                break;
        }

        if (s_c == "2" || s_c == "4")
        {
            return string.Format("<span style=\"color:red;margin-right:4px;\"><em style=\"font-size:16px;\">{0}</em>{1}</span>", s, ns);
        }
        else
        {
            return string.Format("<span style=\"color:black;margin-right:4px;\"><em style=\"font-size:16px;\">{0}</em>{1}</span>", s, ns);
        }
    }

    /// <summary>
    /// 虚拟用户
    /// </summary>
    /// <returns></returns>
    public static List<string> UserIdList()
    {
        var list = new List<string>
        {
            "100622","100623","100624","100625","100626","100629","100631","100637","100639",
            "100641","100643","100645","100648","100650","100653","100658","100660","100663","100668","100672","100674","100675","100676","100677",
            "100678","100679","100680","100681","100682","100683","100627","100630","100628","100632","100633","100634","100635","100636","100638",
            "100640","100642","100646","100647","100649","100651","100652","100654","100655","100656","100657","100659","100661","100662","100664",
            "100665","100667","100669","100670","100671","100673","100644"
        };
        return list;
    }
    public static string HrefGameName(string gamecode)
    {
        switch (gamecode.ToLower())
        {
            case "jczq":
                return "jingcai";
            case "jclq":
                return "jingcaibasket";
            case "bjdc":
                return "danchang";
            case "ctzq":
                return "toto";
            case "fc3d":
            case "pl3":
                return "fcp3";
            default: return gamecode;
        }
    }
    #endregion

    #region 网站是否开售
    public static int GetWebIsOpen()
    {
        return int.Parse(ConfigurationManager.AppSettings["IsOpen"]);
    }
    #endregion

    #region 时间转换

    /// <summary>
    /// DateTime时间格式转换为Unix时间戳格式
    /// </summary>
    /// <param name="time"> DateTime时间格式</param>
    /// <returns>Unix时间戳格式</returns>
    public static int ConvertDateTimeInt(System.DateTime time)
    {
        System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
        return (int)(time - startTime).TotalSeconds;
    }

    /// <summary>
    /// 时间戳转为C#格式时间
    /// </summary>
    /// <param name="timeStamp">Unix时间戳格式</param>
    /// <returns>C#格式时间</returns>
    public static DateTime GetTime(string timeStamp)
    {
        DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(DateTime.Now);
        long lTime = long.Parse(timeStamp + "0000000");
        TimeSpan toNow = new TimeSpan(lTime);
        return dtStart.Add(toNow);
    }
    #endregion

    public static string GetAliPayAccount()
    {

        try
        {

            return WCFClients.GameClient.QueryCoreConfigByKey("AlipayFillMoneyAccount").ConfigValue;
        }
        catch (Exception)
        {
            return "pay@iqucai.com";
        }
    }

    public static string GetTeamHref(int teamid)
    {
        if (teamid <= 0)
        {
            return "href=javascript:void(0)";
        }
        return "href=http://info.wancai.com/teams/" + teamid + " target=_blank";
    }

    public static string GetLeagueHref(int leaugeid)
    {
        if (leaugeid <= 0)
        {
            return "href=javascript:void(0)";
        }
        return "href=http://info.wancai.com/zuqiu-" + leaugeid + "/ target=_blank";
    }
    public static string JlGetLeagueHref(int leaugeid)
    {
        if (leaugeid <= 0)
        {
            return "href=javascript:void(0)";
        }
        return "href=http://info.wancai.com/lanqiu/" + leaugeid + "/ target=_blank";
    }


    public static string ConvertGameCode(string gameCode)
    {
        if (string.IsNullOrEmpty(gameCode)) return string.Empty;
        switch (gameCode.ToUpper())
        {
            case "CQSSC":
                return "重庆时时彩";
            case "JX11X5":
                return "江西11选5";
            case "SSQ":
                return "双色球";
            case "DLT":
                return "大乐透";
            case "FC3D":
                return "福彩3D";
            case "PL3":
                return "排列3";
            case "CTZQ":
                return "传统足球";
            case "BJDC":
                return "北京单场";
            case "JCZQ":
                return "竞彩足球";
            case "JCLQ":
                return "竞彩篮球";
            default:
                break;
        }
        return string.Empty;
    }
}