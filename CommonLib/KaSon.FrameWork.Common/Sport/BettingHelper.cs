using EntityModel;
using EntityModel.CoreModel.BetingEntities;
using EntityModel.Domain.Entities;
using EntityModel.ExceptionExtend;
using EntityModel.LotteryJsonInfo;
using KaSon.FrameWork.Common.GlobalConfigJson;
using KaSon.FrameWork.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.Sport
{
   public class  BettingHelper
    {
        //验证 不支持的玩法
        public static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<Cache_JCZQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 0：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        public static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<Cache_JCLQ_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }
        public static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<Cache_BJDC_MatchInfo> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new Exception(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckPrivilegesType_JCLQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<C_JCLQ_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "RFSF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "SFC":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "DXF":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckPrivilegesType_JCZQ(string gameCode, string gameType, string playType, Sports_AnteCodeInfoCollection codeList, List<C_JCZQ_Match> matchList)
        {
            //PrivilegesType
            //用英文输入法的:【逗号】如’,’分开。
            //竞彩足球：1:胜平负单关 2:比分单关 3:进球数单关 4:半全场单关 5:胜平负过关 6:比分过关 7:进球数过关 8:半全场过关9：不让球胜平负单关 10：不让球胜平负过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "SPF":
                        privileType = playType == "1_1" ? "1" : "5";
                        break;
                    case "BRQSPF":
                        privileType = playType == "1_1" ? "9" : "0";
                        break;
                    case "BF":
                        privileType = playType == "1_1" ? "2" : "6";
                        break;
                    case "ZJQ":
                        privileType = playType == "1_1" ? "3" : "7";
                        break;
                    case "BQC":
                        privileType = playType == "1_1" ? "4" : "8";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.MatchId == code.MatchId);
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.MatchIdName, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckPrivilegesType_BJDC(string gameCode, string gameType, string playType, string issuseNumber, Sports_AnteCodeInfoCollection codeList, List<C_BJDC_Match> matchList)
        {
            //PrivilegesType
            //竞彩篮球：1:胜负单关 2:让分胜负单关 3:胜分差单关 4:大小分单关 5:胜负过关 6:让分胜负过关 7:胜分差过关 8:大小分过关
            foreach (var code in codeList)
            {
                var privileType = string.Empty;
                var tempGameType = gameType != "HH" ? gameType : code.GameType;
                switch (tempGameType.ToUpper())
                {
                    case "BF":
                        privileType = "1";
                        break;
                    case "BQC":
                        privileType = "2";
                        break;
                    case "SPF":
                        privileType = "3";
                        break;
                    case "SXDS":
                        privileType = "4";
                        break;
                    case "ZJQ":
                        privileType = "5";
                        break;
                    case "SF":
                        privileType = "6";
                        break;
                    default:
                        break;
                }
                var temp = matchList.FirstOrDefault(p => p.Id == (issuseNumber + "|" + code.MatchId));
                if (temp == null || string.IsNullOrEmpty(temp.PrivilegesType)) continue;
                var privileArray = temp.PrivilegesType.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (!string.IsNullOrEmpty(privileType) && privileArray.Contains(privileType))
                    throw new LogicException(string.Format("{0} {1}玩法 暂不支持{2}投注", temp.Id, FormatGameType(gameCode, gameType), playType == "1_1" ? "单关" : "过关"));
            }
        }

        public static void CheckSportAnteCode(string gameCode, string gameType, string[] anteCode)
        {
            var allowCodeList = new string[] { };
            switch (gameCode)
            {
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负
                        case "BRQSPF": //不让球胜平负
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,50,51,52,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,05,15,25,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                    }
                    break;
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":     // 胜负
                        case "RFSF":     // 让分胜负
                        case "DXF":     // 大小分
                            allowCodeList = "3,0".Split(',');
                            break;
                        case "SFC":      // 胜分差
                            allowCodeList = "01,02,03,04,05,06,11,12,13,14,15,16".Split(',');
                            break;
                    }
                    break;
                case "BJDC":
                    switch (gameType)
                    {
                        case "SPF":     // 胜平负 
                            allowCodeList = "3,1,0".Split(',');
                            break;
                        case "ZJQ":     // 进球数
                            allowCodeList = "0,1,2,3,4,5,6,7".Split(',');
                            break;
                        case "SXDS":    // 上下单双。上单 3；上双 2；下单 1；下双 0；
                            allowCodeList = "SD,SS,XD,XS".Split(',');
                            break;
                        case "BF":      // 比分
                            allowCodeList = "10,20,30,40,21,31,41,32,42,X0,00,11,22,33,XX,01,02,03,04,12,13,14,23,24,0X".Split(',');
                            break;
                        case "BQC":     // 半全场
                            allowCodeList = "33,31,30,13,11,10,03,01,00".Split(',');
                            break;
                        case "SF":
                            allowCodeList = "3,0".Split(',');
                            break;
                    }
                    break;
                default:
                    break;
            }
            foreach (var item in anteCode)
            {
                if (!allowCodeList.Contains(item))
                    throw new Exception(string.Format("投注号码{0}为非法字符", item));
            }
        }

        // 检查订单基本信息
        public static void CheckSchemeOrder(Sports_BetingInfo info)
        {
            if (info.AnteCodeList.Count == 0)
                throw new ArgumentException("未选择任何比赛或者投注号码");
            if (info.Amount <= 0)
                throw new ArgumentException("订单倍数错误");
            if (info.TotalMoney <= 0M)
                throw new ArgumentException("订单金额错误");
            if (info.GameType != null && info.GameType.ToUpper() != "HH")
            {
                if (info.AnteCodeList != null)
                {
                    foreach (var item in info.AnteCodeList)
                    {
                        if (item.GameType != null)
                        {
                            if (item.GameType.ToUpper() != info.GameType.ToUpper())
                                throw new Exception("彩种玩法有误，应该是:" + FormatGameType(info.GameCode, info.GameType) + ",但实际是:" + FormatGameType(info.GameCode, item.GameType));
                        }
                    }
                }
            }
        }
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


        public static Dictionary<string, string> _stopTime = new Dictionary<string, string>();
        /// <summary>
        /// 查询指定彩是否可以出票
        /// </summary>
        /// <param name="AppSettings">配置文件</param>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public static bool CanRequestBet(string gameCode)
        {
            try
            {
                //
                var key = string.Format("{0}_{1}", gameCode.ToUpper(), "StopTicketing");
                string value = GbConfigHelper.GlobalConfig[key].ToString();// "";  // ConfigurationManager.AppSettings[key];
               // var key = string.Format("{0}_{1}", gameCode.ToUpper(), "StopTicketing");
               // string value = AppSettings; //DBbase.GlobalConfig[key].ToString();
                if (!_stopTime.ContainsKey(key))
                {
                    _stopTime.Add(key, value);
                }
                var stopTime = _stopTime[key];
                if (string.IsNullOrEmpty(stopTime))
                    return true;

                var szArray = new string[] { "SSQ", "DLT", "FC3D", "PL3" };
                if (szArray.Contains(gameCode))
                {
                    var szTimeArray = stopTime.Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                    var szStartTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + szTimeArray[0]);
                    var szEndTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + szTimeArray[1]);

                    if (DateTime.Now > szStartTime && DateTime.Now < szEndTime)
                    {
                        if (gameCode == "FC3D" || gameCode == "PL3")
                        {
                            return false;
                        }
                        var szDayIndex = (int)DateTime.Now.DayOfWeek;
                        switch (szDayIndex)
                        {
                            case 0:
                                return gameCode != "SSQ";
                            case 1:
                                return gameCode != "DLT";
                            case 2:
                                return gameCode != "SSQ";
                            case 3:
                                return gameCode != "DLT";
                            case 4:
                                return gameCode != "SSQ";
                            case 5:
                                break;
                            case 6:
                                return gameCode != "DLT";
                            default:
                                break;
                        }
                    }
                    return true;
                }

                var array = stopTime.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                if (array.Length != 7)
                    return true;
                var dayIndex = (int)DateTime.Now.DayOfWeek;
                var timeArray = array[dayIndex].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                var gameCodeArrary = new string[] { "JCZQ", "JCLQ", "OZB", "SJB" };
                var ozbTime = new string[] { "160611", "160612", "160613", "160614"
                                        , "160615", "160616", "160617", "160618", "160619", "160620" , "160621", "160622"
                                        , "160623", "160625", "160626", "160627", "160628", "160701", "160702", "160703",
                                        "160704", "160707", "160708", "160711"};
                var sjbTime = new string[] { "180614", "180615", "180616", "180617", "180618", "180619", "180620", "180621",
                                            "180621","180622","180623","180624","180625","180626","180627","180628","180629",
                                            "180630","180701","180702","180703","180704","180705","180706","180707","180708",
                                            "180709","180710","180711","180712","180713","180714","180715"};
                var date = DateTime.Now.ToString("yyyyMMdd").Substring(2);
                var isOzb = false;
                var isSJB = false;
                if (sjbTime.Contains(date))
                    isSJB = true;
                if (ozbTime.Contains(date))
                    isOzb = true;
                if (gameCodeArrary.Contains(gameCode) && isOzb)
                {

                    var ssTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "03:00");
                    var eTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "09:00");
                    if (ssTime > eTime)
                        eTime = eTime.AddDays(1);

                    if (DateTime.Now > ssTime && DateTime.Now < eTime)
                    {
                        return false;
                    }
                    return true;
                }
                if (gameCodeArrary.Contains(gameCode) && isSJB)
                {

                    var ssTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "03:00");
                    var eTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + "09:00");
                    if (ssTime > eTime)
                        eTime = eTime.AddDays(1);

                    if (DateTime.Now > ssTime && DateTime.Now < eTime)
                    {
                        return false;
                    }
                    return true;
                }
                var startTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + timeArray[0]);
                var endTime = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd ") + timeArray[1]);

                if (startTime > endTime)
                    endTime = endTime.AddDays(1);

                if (DateTime.Now > startTime && DateTime.Now < endTime)
                {
                    return false;
                }
                return true;
            }
            catch (Exception)
            {
                return true;
            }
        }

        public static string GetSportsBettingSchemeId(string gameCode)
        {
            string prefix = gameCode;
            return prefix + UsefullHelper.UUID();
        }
        /// <summary>
        /// 检查串关方式
        /// </summary>
        public static void CheckPlayType(string gameCode, List<string> gameTypeList, int m)
        {
            if (gameCode.ToUpper() != "JCZQ" && gameCode.ToUpper() != "JCLQ")
                return;
            //竞彩足球 BF、BQC 最高串关4，ZJQ最高串关6
            //竞彩篮球 SFC 最高串关4
            if (gameTypeList.Contains("BF") && m > 4)
                throw new LogicException("比分过关方式最大为4串");
            if (gameTypeList.Contains("BQC") && m > 4)
                throw new LogicException("比分过关方式最大为4串");
            if (gameTypeList.Contains("ZJQ") && m > 6)
                throw new LogicException("总进球过关方式最大为6串");
            if (gameTypeList.Contains("SFC") && m > 4)
                throw new LogicException("胜分差过关方式最大为4串");
        }


        public static void CheckGameCodeAndType(string gameCode, string gameType)
        {
            if (gameCode.ToUpper() == "BJDC")
            {
                if (new string[] { "ZJQ", "SXDS", "BQC", "BF" }.Contains(gameType.ToUpper()))
                    throw new Exception("该玩法暂停销售");
            }
        }
        public static void CheckUserRealName(string idCardNumber)
        {
            if (string.IsNullOrEmpty(idCardNumber))
                throw new Exception("用户身份证信息不完整，不能购买彩票");
            if (idCardNumber.Length < 18)
            {
                if (idCardNumber.Length != 15)
                    throw new Exception("用户身份证号格式不正确，不能购买彩票");
            }
            else if (idCardNumber.Length > 15)
            {
                if (idCardNumber.Length != 18)
                    throw new Exception("用户身份证号格式不正确，不能购买彩票");
            }

            var birth = string.Empty;
            int year = 0;
            int month = 0;
            int day = 0;
            if (idCardNumber.Length == 18)
                birth = idCardNumber.Substring(6, 8);
            if (idCardNumber.Length == 15)
                birth = string.Format("19{0}", idCardNumber.Substring(6, 6));
            if (birth.Length != 8)
                throw new Exception("用户身份证号格式不正确，不能购买彩票");

            year = int.Parse(birth.Substring(0, 4));
            month = int.Parse(birth.Substring(4, 2));
            day = int.Parse(birth.Substring(6, 2));

            var diffYear = DateTime.Now.Year - year;
            if (diffYear > 18)
                return;
            if (diffYear < 18)
                throw new Exception("用户未满18岁，不能购买彩票");
            if (diffYear == 18)
            {
                if (DateTime.Now.Month < month)
                    throw new Exception("用户未满18岁，不能购买彩票");
                else if (DateTime.Now.Month == month)
                {
                    if (DateTime.Now.Day < day)
                        throw new Exception("用户未满18岁，不能购买彩票");
                }
            }
        }
        static string ResUrl = string.Empty;
        public BettingHelper()
        {
            //http://res.iqucai.com/matchdata/jczq/match_list.json?_=1425952243703
            ResUrl = ConfigHelper.ConfigInfo["ResSiteUrl"].ToString() ?? "http://10.0.3.6/";
            //ConfigurationManager.AppSettings["ResSiteUrl"] 
        }

        public List<T> GetMatchInfoList<T>(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (result == null || string.IsNullOrEmpty(result))
                return new List<T>();
            return JsonHelper.Deserialize<List<T>>(result);
        }

        private static string ReadFileString(string fullUrl)
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
        /// 传统足球详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issuseId"></param>
        /// <returns></returns>
        public static List<Web_CTZQ_BonusPoolInfo> GetPoolInfo_CTZQ(string type, string issuseId)
        {
            var poolInfo = GetCTZQBonusPool(IssuseFile(type, issuseId));
            return poolInfo;
        }

        /// <summary>
        /// 传统足球奖期详情
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static List<Web_CTZQ_BonusPoolInfo> GetCTZQBonusPool(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (string.IsNullOrEmpty(result))
                return new List<Web_CTZQ_BonusPoolInfo>();
            return JsonHelper.Deserialize<List<Web_CTZQ_BonusPoolInfo>>(result);
        }

        #region 文件路径
        /// <summary>
        /// 奖期数据文件
        /// </summary>
        private static string IssuseFile(string type, string issuseId)
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
        #endregion

        #region 数字彩详情
        /// <summary>
        /// 数字彩详情
        /// </summary>
        /// <param name="type"></param>
        /// <param name="issuseId"></param>
        /// <returns></returns>
        public static Web_SZC_BonusPoolInfo GetPoolInfo(string type, string issuseId)
        {
            var poolInfo = GetSZCBonusPool(IssuseFile(type, issuseId));
            return poolInfo;
        }

        public static Web_SZC_BonusPoolInfo GetSZCBonusPool(string filePath)
        {
            var result = ReadFileString(ResUrl + filePath);
            if (string.IsNullOrEmpty(result))
                return new Web_SZC_BonusPoolInfo();
            return JsonHelper.Deserialize<Web_SZC_BonusPoolInfo>(result);
        }
        #endregion

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
    }
}
