using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;
using KaSon.FrameWork.Helper.分析器工厂;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 分段单式
    /// </summary>
    internal class GameTypeAnalyzer_分段单式 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_分段单式()
        {
            Spliter_Level1 = '|';
            Spliter_Level2 = ',';

            BallNumber_First = 6;
            BallNumber_Last = 1;

            MaxTotalNumber_First = 6;
            MinTotalNumber_First = 6;
            MaxTotalNumber_Last = 1;
            MinTotalNumber_Last = 1;

            MinNumber_First = 1;
            MaxNumber_First = 33;
            MinNumber_Last = 1;
            MaxNumber_Last = 16;

            MaxBallTotalLenght = 36;
            MinBallTotalLenght = 8;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }

        public char Spliter_Level1 { get; set; }
        public char Spliter_Level2 { get; set; }

        public int BallNumber_First { get; set; }
        public int BallNumber_Last { get; set; }

        public int MaxBallTotalLenght { get; set; }
        public int MinBallTotalLenght { get; set; }

        public int MaxTotalNumber_First { get; set; }
        public int MinTotalNumber_First { get; set; }
        public int MaxTotalNumber_Last { get; set; }
        public int MinTotalNumber_Last { get; set; }

        public int MinNumber_First { get; set; }
        public int MaxNumber_First { get; set; }
        public int MinNumber_Last { get; set; }
        public int MaxNumber_Last { get; set; }
        /// <summary>
        /// 投注号码切分后的数字列表。执行CheckAntecode或AnalyzeAnteCode或CaculateBonus以后有值
        /// </summary>
        public string[] AnteCodeNumbers { get; private set; }

        /// <summary>
        /// 检查投注号码格式是否正确
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckAntecode(string antecode, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");

            AnteCodeNumbers = antecode.Split(Spliter_Level1);
            if (AnteCodeNumbers.Length != 2)
            {
                errMsg = string.Format("投注号码必须是被\"{0}\"切分成 2个部分的字符串", Spliter_Level1);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            var arr_first = AnteCodeNumbers[0].Split(Spliter_Level2);
            var arr_last = AnteCodeNumbers[1].Split(Spliter_Level2);
            if (arr_first.Length < MinTotalNumber_First || arr_first.Length > MaxTotalNumber_First)
            {
                if (MinTotalNumber_First == MaxTotalNumber_First)
                {
                    errMsg = string.Format("前区号码必须是\"{0}\"分隔的 {1}个号码", Spliter_Level2, MinTotalNumber_First);
                }
                else
                {
                    errMsg = string.Format("前区号码必须是\"{0}\"分隔的 {1} - {2}个号码", Spliter_Level2, MinTotalNumber_First, MaxTotalNumber_First);
                }
                return false;
            }
            if (arr_last.Length < MinTotalNumber_Last || arr_last.Length > MaxTotalNumber_Last)
            {
                if (MinTotalNumber_Last == MaxTotalNumber_Last)
                {
                    errMsg = string.Format("后区号码必须是\"{0}\"分隔的 {1}个号码", Spliter_Level2, MinTotalNumber_Last);
                }
                else
                {
                    errMsg = string.Format("后区号码必须是\"{0}\"分隔的 {1} - {2}个号码", Spliter_Level2, MinTotalNumber_Last, MaxTotalNumber_Last);
                }
                return false;
            }
            if (arr_first.Length + arr_last.Length < MinBallTotalLenght || arr_first.Length + arr_last.Length > MaxBallTotalLenght)
            {
                if (MinBallTotalLenght == MaxBallTotalLenght)
                {
                    errMsg = string.Format("号码总长度必须是 {0}个号码", MinBallTotalLenght);
                }
                else
                {
                    errMsg = string.Format("号码总长度必须是 {0} - {1}个号码", MinBallTotalLenght, MaxBallTotalLenght);
                }
                return false;
            }
            foreach (var item in arr_first)
            {
                // 检查空位占位符
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg, "F")) { return false; }
            }
            foreach (var item in arr_last)
            {
                // 检查空位占位符
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg, "L")) { return false; }
            }
            // 分组，以去除号码中的重复项
            var group_first = arr_first.GroupBy(c => c);
            if (group_first.Count() != arr_first.Length)
            {
                errMsg = "前区号码中有重复的数字 - " + antecode;
                return false;
            }
            // 分组，以去除号码中的重复项
            var group_last = arr_last.GroupBy(c => c);
            if (group_last.Count() != arr_last.Length)
            {
                errMsg = "后区号码中有重复的数字 - " + antecode;
                return false;
            }
            errMsg = "";
            return true;
        }
        /// <summary>
        /// 分析一个投注号码，计算此号码所包含的注数。
        /// 投注号码格式错误时抛出异常AntecodeFormatException。
        /// </summary>
        /// <param name="antecode">投注号码</param>
        /// <returns>号码所包含的注数</returns>
        public int AnalyzeAnteCode(string antecode)
        {
            string msg;
            if (!CheckAntecode(antecode, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            var c = new Combination();
            var i = 0;
            var j = 0;
            var arr_first = AnteCodeNumbers[0].Split(Spliter_Level2);
            var arr_last = AnteCodeNumbers[1].Split(Spliter_Level2);
            c.Calculate(arr_first, BallNumber_First, (num) => i++);
            c.Calculate(arr_last, BallNumber_Last, (num) => j++);
            return i * j;
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
            var red_antecode = AnteCodeNumbers[0].Split(Spliter_Level2);
            var blue_antecode = AnteCodeNumbers[1].Split(Spliter_Level2);
            var red_winNumber = checker.WinNumbers[0].Split(Spliter_Level2);
            var blue_winNumber = checker.WinNumbers[1].Split(Spliter_Level2);

            var list = new List<int>();
            var p = new Combination();
            p.Calculate(red_antecode, BallNumber_First, (item_red) =>
            {
                p.Calculate(blue_antecode, BallNumber_Last, (item_blue) =>
                {
                    var redCount = GetSameCodeCount(item_red, red_winNumber);
                    var blueCount = GetSameCodeCount(item_blue, blue_winNumber);

                    switch (GameCode)
                    {
                        case "SSQ":
                            if (redCount == 6 && blueCount == 1)
                            {
                                list.Add(1);
                            }
                            else if (redCount == 6 && blueCount == 0)
                            {
                                list.Add(2);
                            }
                            else if (redCount == 5 && blueCount == 1)
                            {
                                list.Add(3);
                            }
                            else if ((redCount == 5 && blueCount == 0) || (redCount == 4 && blueCount == 1))
                            {
                                list.Add(4);
                            }
                            else if ((redCount == 4 && blueCount == 0) || (redCount == 3 && blueCount == 1))
                            {
                                list.Add(5);
                            }
                            else if (blueCount == 1)
                            {
                                list.Add(6);
                            }
                            break;
                        case "DLT":
                            if (redCount == 5 && blueCount == 2)
                            {
                                list.Add(1);
                            }
                            else if (redCount == 5 && blueCount == 1)
                            {
                                list.Add(2);
                            }
                            else if ((redCount == 5 && blueCount == 0) || (redCount == 4 && blueCount == 2))
                            {
                                list.Add(3);
                            }
                            //else if (redCount == 4 && blueCount == 2)
                            //{
                            //    list.Add(4);
                            //}
                            else if ((redCount == 4 && blueCount == 1) || (redCount == 3 && blueCount == 2))
                            {
                                list.Add(4);
                            }
                            //else if ((redCount == 4 && blueCount == 0) || (redCount == 3 && blueCount == 2))
                            //{
                            //    list.Add(6);
                            //}
                            else if ((redCount == 4 && blueCount == 0) || (redCount == 3 && blueCount == 1) || (redCount == 2 && blueCount == 2))
                            {
                                list.Add(5);
                            }
                            else if ((redCount == 3 && blueCount == 0) || (redCount == 1 && blueCount == 2) || (redCount == 2 && blueCount == 1) || (redCount == 0 && blueCount == 2))
                            {
                                list.Add(6);
                            }
                            break;
                        default:
                            throw new ArgumentOutOfRangeException("不支持的彩种 - " + GameCode);
                    }
                });
            });
            return list;
        }
        private int GetSameCodeCount(string[] antecodes, string[] winNumbers)
        {
            var count = 0;
            foreach (var c in antecodes)
            {
                if (winNumbers.Contains(c))
                {
                    count++;
                }
            }
            return count;
        }
    }
}
