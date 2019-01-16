using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;


namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 只处理3BTH,3BTHDT这两种玩法
    /// </summary>
    internal class GameTypeAnalyzer_江苏快3三不同号 : IAntecodeAnalyzable
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }

        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
        public string[] AnteCodeNumbers { get; private set; }

        public GameTypeAnalyzer_江苏快3三不同号()
        {
        }

        public bool CheckAntecode(string antecode, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");

            antecode = antecode.Replace("|", "#");
            AnteCodeNumbers = antecode.Split(new string[] { ",", "#" }, StringSplitOptions.RemoveEmptyEntries);
            if (AnteCodeNumbers.Length < BallNumber)
            {
                errMsg = string.Format("最少选择{0}位号码", BallNumber);
                return false;
            }
            foreach (var item in AnteCodeNumbers)
            {
                if (int.Parse(item) < MinValue || int.Parse(item) > MaxValue)
                {
                    errMsg = string.Format("号码必须是 {0} - {1} 之间的数字", MinValue, MaxValue);
                    return false;
                }
            }
            if (AnteCodeNumbers.Distinct().Count() != AnteCodeNumbers.Length)
            {
                errMsg = "号码中不能包括重复的数字";
                return false;
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
                case "3BTH":
                    switch (AnteCodeNumbers.Length)
                    {
                        case 3: return 1;
                        case 4: return 4;
                        case 5: return 10;
                        case 6: return 20;
                    }
                    break;
                case "3BTHDT":
                    antecode = antecode.Replace("|", "#");
                    var array = antecode.Split('#');
                    if (array.Length != 2)
                        throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - 胆拖应是以#分隔的两部分");
                    switch (array[0].Split(',').Length)
                    {
                        case 1:
                            switch (array[1].Split(',').Length)
                            {
                                case 2: return 1;
                                case 3: return 3;
                                case 4: return 6;
                                case 5: return 10;
                            }
                            return 1;
                        case 2:
                            return array[1].Split(',').Length;
                    }
                    break;
                default:
                    break;
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
            if (checker.WinNumbers.Distinct().Count() != checker.WinNumbers.Length)
                return bonusLevelList;
            switch (GameType)
            {
                case "3BTH":
                    new Combination().Calculate(AnteCodeNumbers, 3, (a) =>
                    {
                        var t = a.OrderBy(f => f);
                        if (string.Join(",", t.ToArray()) == winNumber)
                        {
                            bonusLevelList.Add(0);
                            return false;
                        }
                        return true;
                    });
                    break;
                case "3BTHDT":
                    antecode = antecode.Replace("|", "#");
                    var array = antecode.Split('#');
                    if (array.Length != 2)
                        throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - 胆拖应是以#分隔的两部分");
                    var a1 = array[0].Split(',');
                    var a2 = array[1].Split(',');
                    if (a1.Length == 2)
                    {
                        var list = new List<string>();
                        foreach (var item in a1)
                            list.Add(item);
                        foreach (var item in a2)
                        {
                            var t = new List<string>();
                            t.AddRange(list);
                            t.Add(item);
                            var t1 = t.OrderBy(f => f);
                            if (string.Join(",", t1.ToArray()) == winNumber)
                            {
                                bonusLevelList.Add(0);
                            }
                        }
                    }
                    else if (a1.Length == 1)
                    {
                        new Combination().Calculate(a2, 2, (s) =>
                        {
                            var list = new List<string>();
                            list.Add(a1[0]);
                            foreach (var item in s)
                            {
                                list.Add(item);
                            }
                            var t = list.OrderBy(f => f);
                            if (string.Join(",", t.ToArray()) == winNumber)
                            {
                                bonusLevelList.Add(0);
                                return false;
                            }
                            return true;
                        });
                    }

                    break;
                default:
                    break;
            }

            return bonusLevelList;
        }
    }
}
