﻿using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Interface
{
    /// <summary>
    /// 标识能够分析投注号码的接口
    /// </summary>
    public interface IAntecodeAnalyzable_Sport
    {
        /// <summary>
        /// 过关基数
        /// </summary>
        int BaseCount { get; set; }
        /// <summary>
        /// 检查投注号码格式是否正确
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        bool CheckAntecode(ISportAnteCode[] antecodeList, out string errMsg);
        /// <summary>
        /// 分析一个投注号码，计算此号码所包含的注数
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <returns>号码所包含的注数</returns>
        int AnalyzeAnteCode(ISportAnteCode[] antecodeList);
        /// <summary>
        /// 计算投注号码的中奖状态，返回中奖的奖等列表。如果为空，表示未中奖；
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="winNumber">中奖号码</param>
        /// <returns>返回中奖的奖等列表</returns>
        SportBonusResult CaculateBonus(ISportAnteCode[] antecodeList, ISportResult[] winNumberList);
    }
   
    public interface ISportResult
    {
        string GetMatchId(string gameCode);
        string GetMatchResult(string gameCode, string gameType, decimal offset = -1);
        string GetFullMatchScore(string gameCode);
    }
    public static class SportAnalyzer
    {
        public static List<int> AnalyzeChuan(int a, int b)
        {
            var list = new List<int>();
            if (b == 1)
            {
                list.Add(a);
                return list;
            }
            switch (a + "_" + b)
            {
                #region 2串(只有北京单场)
                case "2_3":
                    list.Add(1);
                    list.Add(2);
                    break;
                #endregion

                #region 3串
                case "3_3":
                    list.Add(2);
                    break;
                case "3_4":
                    list.Add(2);
                    list.Add(3);
                    break;
                case "3_7":
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    break;
                #endregion

                #region 4串
                case "4_4":
                    list.Add(3);
                    break;
                case "4_5":
                    list.Add(3);
                    list.Add(4);
                    break;
                case "4_6":
                    list.Add(2);
                    break;
                case "4_11":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    break;
                case "4_15":
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    break;
                #endregion

                #region 5串
                case "5_5":
                    list.Add(4);
                    break;
                case "5_6":
                    list.Add(4);
                    list.Add(5);
                    break;
                case "5_10":
                    list.Add(2);
                    break;
                case "5_16":
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    break;
                case "5_20":
                    list.Add(2);
                    list.Add(3);
                    break;
                case "5_26":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    break;
                case "5_31":
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    break;
                #endregion

                #region 6串
                case "6_6":
                    list.Add(5);
                    break;
                case "6_7":
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_15":
                    list.Add(2);
                    break;
                case "6_20":
                    list.Add(3);
                    break;
                case "6_22":
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_35":
                    list.Add(2);
                    list.Add(3);
                    break;
                case "6_42":
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_50":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    break;
                case "6_57":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                case "6_63":
                    list.Add(1);
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    break;
                #endregion

                #region 7串
                case "7_7":
                    list.Add(6);
                    break;
                case "7_8":
                    list.Add(6);
                    list.Add(7);
                    break;
                case "7_21":
                    list.Add(5);
                    break;
                case "7_35":
                    list.Add(4);
                    break;
                case "7_120":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    list.Add(7);
                    break;
                #endregion

                #region 8串
                case "8_8":
                    list.Add(7);
                    break;
                case "8_9":
                    list.Add(7);
                    list.Add(8);
                    break;
                case "8_28":
                    list.Add(6);
                    break;
                case "8_56":
                    list.Add(5);
                    break;
                case "8_70":
                    list.Add(4);
                    break;
                case "8_247":
                    list.Add(2);
                    list.Add(3);
                    list.Add(4);
                    list.Add(5);
                    list.Add(6);
                    list.Add(7);
                    list.Add(8);
                    break;
                #endregion
            }
            if (list.Count == 0)
            {
                throw new ArgumentException("不支持的串关方式 - " + a + "_" + b);
            }
            return list;
        }

        public static string GetMatchResultDisplayName(string gameType, string matchResult)
        {
            if (string.IsNullOrEmpty(matchResult))
            {
                return "";
            }
            switch (gameType.ToUpper())
            {
                case "SPF":
                case "T14C":
                case "BRQSPF":
                    switch (matchResult)
                    {
                        case "3":
                            return "胜";
                        case "1":
                            return "平";
                        case "0":
                            return "负";
                    }
                    return matchResult;
                case "ZJQ":
                    int t;
                    if (int.TryParse(matchResult, out t))
                    {
                        if (t >= 7)
                        {
                            return "7+球";
                        }
                    }
                    return matchResult + "球";
                case "SXDS":
                    switch (matchResult)
                    {
                        case "SD":
                            return "上单";
                        case "SS":
                            return "上双";
                        case "XD":
                            return "下单";
                        case "XS":
                            return "下双";
                    }
                    return matchResult;
                case "BF":
                    if (matchResult == "X0")
                    {
                        return "胜其他";
                    }
                    else if (matchResult == "XX")
                    {
                        return "平其他";
                    }
                    else if (matchResult == "0X")
                    {
                        return "负其他";
                    }
                    else
                    {
                        var arr = matchResult.ToArray();
                        if (arr.Length == 2)
                        {
                            return arr[0] + ":" + arr[1];
                        }
                    }
                    return matchResult;
                case "BQC":
                    var tmp = matchResult.ToArray();
                    if (tmp.Length == 2)
                    {
                        return GetMatchResultDisplayName("SPF", tmp[0].ToString()) + "-" + GetMatchResultDisplayName("SPF", tmp[1].ToString());
                    }
                    return matchResult;

                case "SF":
                    switch (matchResult)
                    {
                        case "3":
                            return "主胜";
                        case "0":
                            return "客胜";
                    }
                    return matchResult;
                case "RFSF":
                    switch (matchResult)
                    {
                        case "3":
                            return "主胜";
                        case "0":
                            return "客胜";
                    }
                    return matchResult;
                case "SFC":
                    switch (matchResult)
                    {
                        case "01":
                            return "主胜1-5";
                        case "02":
                            return "主胜6-10";
                        case "03":
                            return "主胜11-15";
                        case "04":
                            return "主胜16-20";
                        case "05":
                            return "主胜21-25";
                        case "06":
                            return "主胜26+";
                        case "11":
                            return "客胜1-5";
                        case "12":
                            return "客胜6-10";
                        case "13":
                            return "客胜11-15";
                        case "14":
                            return "客胜16-20";
                        case "15":
                            return "客胜21-25";
                        case "16":
                            return "客胜26+";
                    }
                    return matchResult;
                case "DXF":
                    switch (matchResult)
                    {
                        case "3":
                            return "大分";
                        case "0":
                            return "小分";
                    }
                    return matchResult;
            }
            return matchResult;
        }
    }
    //    public static string GetMatchResultDisplayName(string gameType, string matchResult)
    //    {
    //        if (string.IsNullOrEmpty(matchResult))
    //        {
    //            return "";
    //        }
    //        switch (gameType.ToUpper())
    //        {
    //            case "T14C":
    //            case "SPF":
    //            case "BRQSPF":
    //                switch (matchResult)
    //                {
    //                    case "3":
    //                        return "胜";
    //                    case "1":
    //                        return "平";
    //                    case "0":
    //                        return "负";
    //                }
    //                return matchResult;
    //            case "ZJQ":
    //                int t;
    //                if (int.TryParse(matchResult, out t))
    //                {
    //                    if (t >= 7)
    //                    {
    //                        return "7+球";
    //                    }
    //                }
    //                return matchResult + "球";
    //            case "SXDS":
    //                switch (matchResult)
    //                {
    //                    case "SD":
    //                        return "上单";
    //                    case "SS":
    //                        return "上双";
    //                    case "XD":
    //                        return "下单";
    //                    case "XS":
    //                        return "下双";
    //                }
    //                return matchResult;
    //            case "BF":
    //                if (matchResult == "X0")
    //                {
    //                    return "胜其他";
    //                }
    //                else if (matchResult == "XX")
    //                {
    //                    return "平其他";
    //                }
    //                else if (matchResult == "0X")
    //                {
    //                    return "负其他";
    //                }
    //                else
    //                {
    //                    var arr = matchResult.ToArray();
    //                    if (arr.Length == 2)
    //                    {
    //                        return arr[0] + ":" + arr[1];
    //                    }
    //                }
    //                return matchResult;
    //            case "BQC":
    //                var tmp = matchResult.ToArray();
    //                if (tmp.Length == 2)
    //                {
    //                    return GetMatchResultDisplayName("SPF", tmp[0].ToString()) + "-" + GetMatchResultDisplayName("SPF", tmp[1].ToString());
    //                }
    //                return matchResult;

    //            case "SF":
    //                switch (matchResult)
    //                {
    //                    case "3":
    //                        return "胜";
    //                    case "0":
    //                        return "负";
    //                }
    //                return matchResult;
    //            case "RFSF":
    //                switch (matchResult)
    //                {
    //                    case "3":
    //                        return "胜";
    //                    case "0":
    //                        return "负";
    //                }
    //                return matchResult;
    //            case "SFC":
    //                switch (matchResult)
    //                {
    //                    case "01":
    //                        return "胜1-5";
    //                    case "02":
    //                        return "胜6-10";
    //                    case "03":
    //                        return "胜11-15";
    //                    case "04":
    //                        return "胜16-20";
    //                    case "05":
    //                        return "胜21-25";
    //                    case "06":
    //                        return "胜26+";
    //                    case "11":
    //                        return "负1-5";
    //                    case "12":
    //                        return "负6-10";
    //                    case "13":
    //                        return "负11-15";
    //                    case "14":
    //                        return "负16-20";
    //                    case "15":
    //                        return "负21-25";
    //                    case "16":
    //                        return "负26+";
    //                }
    //                return matchResult;
    //            case "DXF":
    //                switch (matchResult)
    //                {
    //                    case "3":
    //                        return "大分";
    //                    case "0":
    //                        return "小分";
    //                }
    //                return matchResult;
    //        }
    //        return matchResult;
    //    }
    //}
    public class SportBonusResult
    {
        public bool IsWin { get; set; }
        public int BaseCount { get; set; }

        public int AnteDanCount { get; set; }
        public int AnteTuoCount { get; set; }
        public int AnteTotalCount { get; set; }

        public int HitDanCount { get; set; }
        public int HitTuoCount { get; set; }
        public int HitTotalCount { get; set; }

        // 中奖注数
        public List<string> HitDanMatchIdList { get; set; }
        public List<string> HitTuoMatchIdList { get; set; }
        public List<string> HitTotalMatchIdList { get; set; }

        public int BonusCount { get; set; }
        public List<string[]> BonusHitMatchIdListCollection { get; set; }
    }
}
