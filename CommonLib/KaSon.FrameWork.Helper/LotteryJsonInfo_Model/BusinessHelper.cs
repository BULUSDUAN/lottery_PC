using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Text;
using System.Globalization;
using System.Xml;
using System.IO;

namespace KaSon.FrameWork.Helper
{
    public class BusinessHelper
    {
        string ResUrl = string.Empty;

        /// <summary>
        /// 奖期数据文件
        /// </summary>
        public static string IssuseFile(string type, string issuseId)
        {
            if (type.StartsWith("CTZQ"))
            {
                var strs = type.Split('_');
                var gameCode = strs[0];
                //var gameType = strs[1];
                return string.Format("/matchdata/{0}/{1}/{2}_BonusLevel.json?_={3}", gameCode, issuseId, type, DateTime.Now.ToString("yyyyMMddHHmmss"));
            }
            return string.Format("/matchdata/{0}/{0}_{1}.json?_={2}", type, issuseId, DateTime.Now.ToString("yyyyMMddHHmmss"));
        }
        public BusinessHelper()
        {
            //http://res.iqucai.com/matchdata/jczq/match_list.json?_=1425952243703
            ResUrl =string.IsNullOrEmpty(ConfigHelper.ConfigInfo["ResSiteUrl"].ToString())?"http://10.0.3.6/":"";
        }

        public static string GetDomain()
        {
            string strUrl = string.IsNullOrEmpty(ConfigHelper.ConfigInfo["ResSiteUrl"].ToString()) ? "http://www.baidu.com":"";
            return strUrl;
        }
        public List<T> GetMatchInfoList<T>(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (result == null || string.IsNullOrEmpty(result))
                return new List<T>();
            return JsonHelper.Deserialize<List<T>>(result);
        }

        public Web_SZC_BonusPoolInfo GetSZCBonusPool(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if ( string.IsNullOrEmpty(result))
                return new Web_SZC_BonusPoolInfo();
            return JsonHelper.Deserialize<Web_SZC_BonusPoolInfo>(result);
        }

        /// <summary>
        /// 传统足球奖期详情
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public List<Web_CTZQ_BonusPoolInfo> GetCTZQBonusPool(string filePath) 
        {
            var result = ReadFileString(ResUrl + filePath);
            if (string.IsNullOrEmpty(result))
                return new List<Web_CTZQ_BonusPoolInfo>();
            return JsonHelper.Deserialize<List<Web_CTZQ_BonusPoolInfo>>(result);
        }

        private string ReadFileString(string fullUrl)
        {
            try
            {
                string strResult = PostManager.Get(fullUrl, Encoding.UTF8);
                if (strResult == "404") return string.Empty;

                if (!string.IsNullOrEmpty(strResult))
                {
                    if (strResult.ToLower().StartsWith("var"))
                    {
                        string[] strArray = strResult.Split('=');
                        if (strArray != null && strArray.Length == 2)
                        {
                            if (strArray[1].ToString().Trim().EndsWith(";"))
                            {
                                return strArray[1].ToString().Trim().TrimEnd(';');
                            }
                            return strArray[1].ToString().Trim();
                        }
                    }
                }
            }
            catch (Exception)
            {
            }
            return string.Empty;
        }
        //public string QueryCoreConfigByKey(string key)
        //{
        //    var result = WCFClients.GameClient.QueryCoreConfigByKey(key).ConfigValue;
        //    if (string.IsNullOrEmpty(result))
        //        return string.Empty;
        //    return result;
        //}
        /// <summary>
        /// DateTime时间格式转换为Unix时间戳格式
        /// </summary>
        /// <param name=”time”></param>
        /// <returns></returns>
        public int ConvertDateTimeInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZone.CurrentTimeZone.ToLocalTime(new System.DateTime(1970, 1, 1));
            return (int)(time - startTime).TotalSeconds;
        }
        public DateTime ConvertStrToDateTime(string strTime)
        {
            IFormatProvider ifp = new CultureInfo("zh-CN", true);
            var time = DateTime.ParseExact(strTime, "yyyyMMdd", ifp);
            return time;
        }
        public UpgradeInfo QueryUpgradeInfo()
        {
            try
            {
                XmlDocument xmlDoc = new XmlDocument();
                UpgradeInfo info = new UpgradeInfo();
                string path = Path.Combine(Directory.GetCurrentDirectory(), @"config_Upgrade\Upgrade.xml");
                xmlDoc.Load(path);
                XmlNode node = xmlDoc.SelectSingleNode("UpgradeList/item");
                if (node != null)
                {
                    info.Upgrade = node.Attributes["Upgrade"] == null ? string.Empty : node.Attributes["Upgrade"].Value.ToString();
                    info.UpgradeStopTime = node.Attributes["UpgradeStopTime"] == null ? DateTime.Now : Convert.ToDateTime(node.Attributes["UpgradeStopTime"].Value);
                    info.MaintainPage = node.Attributes["MaintainPage"] == null ? string.Empty : node.Attributes["MaintainPage"].Value.ToString();
                    info.TotalSeconds = 0;
                    if (!string.IsNullOrEmpty(info.Upgrade) && info.Upgrade.ToLower() == "updating")
                    {
                        var timeSpan = info.UpgradeStopTime - DateTime.Now;
                        if (timeSpan.TotalSeconds > 0)
                            info.TotalSeconds = timeSpan.TotalSeconds;
                    }
                }
                return info;
            }
            catch
            {
                return new UpgradeInfo();
            }
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
                case"sjb":
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
                        case "t14c": return "胜负14场";
                        case "tr9": return "任选9";
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
        /// 获取星期
        /// </summary>
        /// <returns></returns>
        public static string Week()
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(DateTime.Now.DayOfWeek)];
            return week;
        }

        public static string GetWeek(DateTime now)
        {
            string[] weekdays = { "周日", "周一", "周二", "周三", "周四", "周五", "周六" };
            string week = weekdays[Convert.ToInt32(now.DayOfWeek)];
            return week;
        }


        /// <summary>
        /// 获取颜色
        /// </summary>
        /// <returns></returns>
        public static string GetLeagueColor()
        {
            string[] colors = { "#385994", "#5b9999", "#67b9cb", "#ddab4a", "#ebeab4", "#5ea673", "#806362", "#a0a065", "#656598", "#562d81", "#484817", "#dd0000", "#577fb5", "#647897", "#396842" };
            Random random = new Random();
            int i = random.Next(0, 14);
            return colors[i];
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
                            return "胜负14场";
                        case "TR9":
                            return "任选9";
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
        /// 传统足球详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issuseId"></param>
        /// <returns></returns>
        public static List<Web_CTZQ_BonusPoolInfo> GetPoolInfo_CTZQ(string type, string issuseId)
        {
            BusinessHelper bizHelper = new BusinessHelper();
            var poolInfo = bizHelper.GetCTZQBonusPool(IssuseFile(type, issuseId));
            return poolInfo;
        }
        /// <summary>
        /// 数字彩详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issuseId"></param>
        /// <returns></returns>
        public static Web_SZC_BonusPoolInfo GetPoolInfo(string type, string issuseId)
        {
            BusinessHelper bizHelper = new BusinessHelper();
            var poolInfo = bizHelper.GetSZCBonusPool(IssuseFile(type, issuseId));
            return poolInfo;
        }
        public static string GetResult(int homeTeamScore, int guestTeamScore)
        {
            string flag = "[{0},{1},{2}-{3}]";
            if (homeTeamScore == guestTeamScore)
            {
                flag = string.Format(flag, "平", "2", homeTeamScore, guestTeamScore);
            }
            else if (homeTeamScore > guestTeamScore)
            {
                flag = string.Format(flag, "胜", "3", homeTeamScore, guestTeamScore);
            }
            else
            {
                flag = string.Format(flag, "负", "1", homeTeamScore, guestTeamScore);
            }
            return flag;
        }
        public static string AnalyticalCurrentSp(string currentSp, string code)
        {
            if (string.IsNullOrEmpty(currentSp) || string.IsNullOrEmpty(code))
                return string.Empty;
            var array = currentSp.Split(',');
            if (array != null && array.Length > 0)
            {
                foreach (var item in array)
                {
                    if (!string.IsNullOrEmpty(item))
                    {
                        var tempArr = item.Split('|');
                        if (tempArr != null && tempArr.Length > 1)
                        {
                            if (code.ToUpper() == tempArr[0].ToUpper())
                                return (tempArr[1]);
                        }
                    }
                }
            }
            return string.Empty;
        }
    }
}