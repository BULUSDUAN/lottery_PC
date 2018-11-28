using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utilities;

namespace Common.Lottery.Helpers
{
    /// <summary>
    /// 只处理 2THFX、2THDX两种玩法
    /// </summary>
    internal class GameTypeAnalyzer_江苏快3二同号单复 : IAntecodeAnalyzable
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }

        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string[] AnteCodeNumbers { get; private set; }

        public bool CheckAntecode(string antecode, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");

            switch (GameType)
            {
                case "2THFX":
                    foreach (var item in antecode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        if (!new string[] { "11X", "22X", "33X", "44X", "55X", "66X" }.Contains(item))
                        {
                            errMsg = string.Format("投注号码格式不正确 - {0}", antecode);
                            return false;
                        }
                    }
                    break;
                case "2THDX":
                    var array = antecode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    if (array.Length != 2)
                    {
                        errMsg = "投注号码应该是由|分隔的两部分";
                        return false;
                    }
                    var a1 = array[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in a1)
                    {
                        if (!new string[] { "11", "22", "33", "44", "55", "66" }.Contains(item))
                        {
                            errMsg = "由|分隔的第一部分必须为两个相同的数字";
                            return false;
                        }
                        foreach (var ch in item)
                        {
                            if (int.Parse(ch.ToString()) < 1 || int.Parse(ch.ToString()) > 6)
                            {
                                errMsg = string.Format("号码必须为{0} - {1}之间的数字", MinValue, MaxValue);
                                return false;
                            }
                        }
                    }

                    var a2 = array[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var item in a2)
                    {
                        if (int.Parse(item) < 1 || int.Parse(item) > 6)
                        {
                            errMsg = string.Format("号码必须为{0} - {1}之间的数字", MinValue, MaxValue);
                            return false;
                        }
                    }
                    foreach (var item2 in a2)
                    {
                        var ch = item2[0];
                        if (a1.Contains(item2.PadLeft(2, ch)))
                        {
                            errMsg = "由#分隔的两部分不能包括相同的数字";
                            return false;
                        }
                    }

                    break;
            }

            errMsg = "";
            return true;
        }

        public int AnalyzeAnteCode(string antecode)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            switch (GameType)
            {
                case "2THFX":
                    return antecode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Length;
                case "2THDX":
                    var array = antecode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    return array[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Length * array[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries).Length;
            }
            return 1;
        }

        public IList<int> CaculateBonus(string antecode, string winNumber)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            var checker = AnalyzerFactory.GetWinNumberAnalyzer(GameCode, GameType);
            if (!checker.CheckWinNumber(winNumber, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, winNumber, "中奖号码格式错误 - " + msg);
            }
            var bonusLevelList = new List<int>();

            switch (GameType)
            {
                case "2THFX":
                    foreach (var item in antecode.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        var one = item.Substring(0, 1);
                        if (checker.WinNumbers.Count(c => c == one) >= 2)
                        {
                            bonusLevelList.Add(0);
                            break;
                        }
                    }
                    break;
                case "2THDX":
                    var array = antecode.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (var array1 in array[0].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        foreach (var array2 in array[1].Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (checker.WinNumbers.Count(c => c == array1.Substring(0, 1)) == 2
                            && checker.WinNumbers.Count(c => c == array2) == 1)
                            {
                                bonusLevelList.Add(0);
                            }
                        }
                    }
                    break;
            }
            return bonusLevelList;
        }
    }
}
