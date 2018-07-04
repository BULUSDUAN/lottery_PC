using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.Common.Business
{
    /// <summary>
    /// 
    /// </summary>
    public class LotteryHelper
    {
        /// <summary>
        /// 通过彩种编号获取彩种名称
        /// </summary>
        /// <param name="gameCode"></param>
        /// <returns></returns>
        public static string GetDisplayNameFromGameCode(string gameCode)
        {
            switch (gameCode)
            {
                case "CQSSC":
                    return "重庆时时彩";
                case "DLT":
                    return "大乐透";
                case "FC3D":
                    return "福彩3D";
                case "GD11X5":
                    return "广东十一选五";
                case "GDKLSF":
                    return "广东快乐十分";
                case "GXKLSF":
                    return "广西快乐十分";
                case "JX11X5":
                    return "江西十一选五";
                case "JXSSC":
                    return "江西时时彩";
                case "PL3":
                    return "排列三";
                case "QLC":
                    return "七乐彩";
                case "QXC":
                    return "七星彩";
                case "SD11X5":
                    return "山东十一选五";
                case "SDQYH":
                    return "山东群英会";
                case "SSQ":
                    return "双色球";
                default:
                    return "";
            }
        }

        /// <summary>
        /// 投注字符名字
        /// </summary>
        /// <param name="betStr">投注字符</param>
        /// <param name="gameType">类型</param>
        /// <returns></returns>
        public static string GetResultName(string betStr, string gameType)
        {
            string BetName = string.Empty;
            switch (gameType)
            {
                case "SPF": return GetSPFName(betStr);
                case "ZJQ": return GetZJQName(betStr);
                case "BQC": return GetBQCName(betStr);
                case "BF": return GetBFName(betStr);
            }
            return BetName;
        }
        /// <summary>
        /// 胜平负
        /// </summary>
        /// <param name="betStr"></param>
        /// <returns></returns>
        private static string GetSPFName(string betStr)
        {
            switch (betStr)
            {
                case "3": return "胜";
                case "1": return "平";
                case "0": return "负";
                default: return "";
            }
        }

        /// <summary>
        /// 总进球
        /// </summary>
        /// <param name="betStr"></param>
        /// <returns></returns>
        private static string GetZJQName(string betStr)
        {
            switch (betStr)
            {
                case "0": return "0";
                case "1": return "1";
                case "2": return "2";
                case "3": return "3";
                case "4": return "4";
                case "5": return "5";
                case "6": return "6";
                case "7": return "7+";
                default: return "";
            }
        }

        /// <summary>
        /// 半全场
        /// </summary>
        /// <param name="betStr"></param>
        /// <returns></returns>
        private static string GetBQCName(string betStr)
        {
            switch (betStr)
            {
                case "1": return "胜-胜";
                case "2": return "胜-平";
                case "3": return "胜-负";
                case "4": return "平-胜";
                case "5": return "平-平";
                case "6": return "平-负";
                case "7": return "负-胜";
                case "8": return "负-平";
                case "9": return "负-负";
                default: return "";
            }
        }

        /// <summary>
        /// 比分
        /// </summary>
        /// <param name="betStr"></param>
        /// <returns></returns>
        private static string GetBFName(string betStr)
        {
            switch (betStr)
            {
                case "1": return "0:0";
                case "2": return "0:1";
                case "3": return "0:2";
                case "4": return "0:3";
                case "5": return "1:0";
                case "6": return "1:1";
                case "7": return "1:2";
                case "8": return "1:3";
                case "9": return "2:0";
                case "10": return "2:1";
                case "11": return "2:2";
                case "12": return "2:3";
                case "13": return "3:0";
                case "14": return "3:1";
                case "15": return "3:2";
                case "16": return "3:3";
                case "17": return "4:0";
                case "18": return "4:1";
                case "19": return "4:2";
                case "20": return "0:4";
                case "21": return "1:4";
                case "22": return "2:4";
                case "23": return "5:0";
                case "24": return "5:1";
                case "25": return "5:2";
                case "26": return "0:5";
                case "27": return "1:5";
                case "28": return "2:5";
                case "29": return "胜其它";
                case "30": return "平其它";
                case "31": return "负其它";
                default: return "";
            }
        }

        /// <summary>
        /// 让球胜平负比分算结果
        /// </summary>
        /// <param name="score">比分</param>
        /// <returns></returns>
        public static string ScoreSPFResult(string score, string letBall)
        {
            string[] arr = score.Split(',');
            string BQCResult = string.Empty;
            if (arr.Length == 4)
            {
                int HomeHalfScore = int.Parse(arr[0]);
                int AwayHalfScore = int.Parse(arr[1]);
                int HomeScore = int.Parse(arr[2]);
                int AwayScore = int.Parse(arr[3]);
                int Ball = int.Parse(letBall);
                if (Ball == 0)
                {
                    if (HomeScore == AwayScore)
                    {
                        BQCResult = "平";
                    }
                    else if (HomeScore > AwayScore)
                    {
                        BQCResult = "胜";
                    }
                    else
                    {
                        BQCResult = "负";
                    }
                }
                else if (Ball < 0)
                {
                    if ((HomeScore - Ball) == AwayScore)
                    {
                        BQCResult = "平";
                    }
                    else if ((HomeScore - Ball) > AwayScore)
                    {
                        BQCResult = "胜";
                    }
                    else
                    {
                        BQCResult = "负";
                    }
                }
                else
                {
                    if (HomeScore == (AwayScore - Ball))
                    {
                        BQCResult = "平";
                    }
                    else if (HomeScore > (AwayScore - Ball))
                    {
                        BQCResult = "胜";
                    }
                    else
                    {
                        BQCResult = "负";
                    }
                }

            }
            return BQCResult;
        }

        /// <summary>
        /// 总进球比分算结果
        /// </summary>
        /// <param name="score">比分</param>
        /// <returns></returns>
        public static string ScoreZJQResult(string score)
        {
            string[] arr = score.Split(',');
            int TotalNum;
            if (arr.Length == 4)
            {
                TotalNum = (int.Parse(arr[2]) + int.Parse(arr[3]));
                if (TotalNum >= 7)
                    return "7+";
                else
                    return TotalNum.ToString();
            }
            else
                return "";
        }

        /// <summary>
        /// 半全场比分算结果
        /// </summary>
        /// <param name="score">比分</param>
        /// <returns></returns>
        public static string ScoreBQCResult(string score)
        {
            string[] arr = score.Split(',');
            string BQCResult = string.Empty;
            if (arr.Length == 4)
            {
                int HomeHalfScore = int.Parse(arr[0]);
                int AwayHalfScore = int.Parse(arr[1]);
                int HomeScore = int.Parse(arr[2]);
                int AwayScore = int.Parse(arr[3]);
                if (HomeHalfScore == AwayHalfScore)
                {
                    BQCResult = "平";
                }
                else if (HomeHalfScore > AwayHalfScore)
                {
                    BQCResult = "胜";
                }
                else
                {
                    BQCResult = "负";
                }
                if (HomeScore == AwayScore)
                {
                    BQCResult += "平";
                }
                else if (HomeScore > AwayScore)
                {
                    BQCResult += "胜";
                }
                else
                {
                    BQCResult += "负";
                }

            }
            return BQCResult;
        }

        /// <summary>
        /// 比分算结果
        /// </summary>
        /// <param name="score">比分</param>
        /// <returns></returns>
        public static string ScoreBFResult(string score)
        {
            string[] arr = score.Split(',');
            if (arr.Length == 4)
            {
                if (int.Parse(arr[2]) > 5)
                {
                    return "胜其它";
                }
                if ((int.Parse(arr[2]) == 4 || int.Parse(arr[2]) == 5)&&int.Parse(arr[3])==3)
                    return "胜其它";
                if (int.Parse(arr[3]) > 5)
                {
                    return "负其它";
                }
                if ((int.Parse(arr[3]) == 4 || int.Parse(arr[3]) == 5) && int.Parse(arr[2]) == 3)
                    return "负其它";
                if (int.Parse(arr[2]) == int.Parse(arr[3]))
                {
                    if (int.Parse(arr[2]) > 3)
                    {
                        return "平其它";
                    }
                }
                return arr[2] + ":" + arr[3];
            }
            else
                return "";
        }

    }
}
