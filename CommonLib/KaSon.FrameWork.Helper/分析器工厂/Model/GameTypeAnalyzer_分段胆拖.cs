using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;


namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 分段胆拖
    /// </summary>
    internal class GameTypeAnalyzer_分段胆拖 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_分段胆拖()
        {
            Spliter_Level1 = '|';
            Spliter_Level2 = ',';

            BallNumber_First = 6;
            BallNumber_Last = 1;

            MaxLength_First = 21;
            MinLength_First = 7;
            MaxLength_Last = 16;
            MinLength_Last = 1;

            MaxTotalNumber_First_Dan = 5;
            MinTotalNumber_First_Dan = 1;
            MaxTotalNumber_First_Tuo = 20;
            MinTotalNumber_First_Tuo = 2;

            MaxTotalNumber_Last_Dan = 0;
            MinTotalNumber_Last_Dan = 0;
            MaxTotalNumber_Last_Tuo = 16;
            MinTotalNumber_Last_Tuo = 1;

            MinNumber_First_Dan = 1;
            MaxNumber_First_Dan = 33;
            MinNumber_First_Tuo = 1;
            MaxNumber_First_Tuo = 33;

            MinNumber_Last_Dan = 1;
            MaxNumber_Last_Dan = 16;
            MinNumber_Last_Tuo = 1;
            MaxNumber_Last_Tuo = 16;

            NeedLastDan = false;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }

        public char Spliter_Level1 { get; set; }
        public char Spliter_Level2 { get; set; }

        public int BallNumber_First { get; set; }
        public int BallNumber_Last { get; set; }

        public int MaxLength_First { get; set; }
        public int MinLength_First { get; set; }
        public int MaxLength_Last { get; set; }
        public int MinLength_Last { get; set; }

        public int MaxTotalNumber_First_Dan { get; set; }
        public int MinTotalNumber_First_Dan { get; set; }
        public int MaxTotalNumber_First_Tuo { get; set; }
        public int MinTotalNumber_First_Tuo { get; set; }

        public int MaxTotalNumber_Last_Dan { get; set; }
        public int MinTotalNumber_Last_Dan { get; set; }
        public int MaxTotalNumber_Last_Tuo { get; set; }
        public int MinTotalNumber_Last_Tuo { get; set; }

        public int MinNumber_First_Dan { get; set; }
        public int MaxNumber_First_Dan { get; set; }
        public int MinNumber_First_Tuo { get; set; }
        public int MaxNumber_First_Tuo { get; set; }

        public int MinNumber_Last_Dan { get; set; }
        public int MaxNumber_Last_Dan { get; set; }
        public int MinNumber_Last_Tuo { get; set; }
        public int MaxNumber_Last_Tuo { get; set; }

        public bool NeedLastDan { get; set; }
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
            if (NeedLastDan)
            {
                if (AnteCodeNumbers.Length != 4)
                {
                    errMsg = string.Format("投注号码必须是被\"{0}\"切分成 4个部分的字符串", Spliter_Level1);
                    return false;
                }
            }
            else
            {
                if (AnteCodeNumbers.Length != 3)
                {
                    errMsg = string.Format("投注号码必须是被\"{0}\"切分成 3个部分的字符串", Spliter_Level1);
                    return false;
                }
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            var arr_first_dan = AnteCodeNumbers[0].Split(new char[] { Spliter_Level2 }, StringSplitOptions.RemoveEmptyEntries);
            var arr_first_tuo = AnteCodeNumbers[1].Split(new char[] { Spliter_Level2 }, StringSplitOptions.RemoveEmptyEntries);
            string[] arr_last_dan, arr_last_tuo;
            if (NeedLastDan)
            {
                arr_last_dan = AnteCodeNumbers[2].Split(new char[] { Spliter_Level2 }, StringSplitOptions.RemoveEmptyEntries);
                arr_last_tuo = AnteCodeNumbers[3].Split(new char[] { Spliter_Level2 }, StringSplitOptions.RemoveEmptyEntries);
            }
            else
            {
                arr_last_dan = new string[0];
                arr_last_tuo = AnteCodeNumbers[2].Split(Spliter_Level2);
            }

            if (arr_first_dan.Length < MinTotalNumber_First_Dan || arr_first_dan.Length > MaxTotalNumber_First_Dan)
            {
                if (MinTotalNumber_First_Dan == MaxTotalNumber_First_Dan)
                {
                    errMsg = string.Format("前区胆码必须是\"{0}\"分隔的 {1}个号码", Spliter_Level2, MinTotalNumber_First_Dan);
                }
                else
                {
                    errMsg = string.Format("前区胆码必须是\"{0}\"分隔的 {1} - {2}个号码", Spliter_Level2, MinTotalNumber_First_Dan, MaxTotalNumber_First_Dan);
                }
                return false;
            }
            if (arr_first_tuo.Length < MinTotalNumber_First_Tuo || arr_first_tuo.Length > MaxTotalNumber_First_Tuo)
            {
                if (MinTotalNumber_First_Tuo == MaxTotalNumber_First_Tuo)
                {
                    errMsg = string.Format("前区拖码必须是\"{0}\"分隔的 {1}个号码", Spliter_Level2, MinTotalNumber_First_Dan);
                }
                else
                {
                    errMsg = string.Format("前区拖码必须是\"{0}\"分隔的 {1} - {2}个号码", Spliter_Level2, MinTotalNumber_First_Dan, MaxTotalNumber_First_Dan);
                }
                return false;
            }
            if (NeedLastDan)
            {
                if (arr_last_dan.Length < MinTotalNumber_Last_Dan || arr_last_dan.Length > MaxTotalNumber_Last_Dan)
                {
                    if (MinTotalNumber_Last_Dan == MaxTotalNumber_Last_Dan)
                    {
                        errMsg = string.Format("后区拖码必须是\"{0}\"分隔的 {1}个号码", Spliter_Level2, MinTotalNumber_Last_Dan);
                    }
                    else
                    {
                        errMsg = string.Format("后区拖码必须是\"{0}\"分隔的 {1} - {2}个号码", Spliter_Level2, MinTotalNumber_Last_Dan, MaxTotalNumber_Last_Dan);
                    }
                    return false;
                }
            }
            if (arr_last_tuo.Length < MinTotalNumber_Last_Tuo || arr_last_tuo.Length > MaxTotalNumber_Last_Tuo)
            {
                if (MinTotalNumber_Last_Tuo == MaxTotalNumber_Last_Tuo)
                {
                    errMsg = string.Format("后区号码必须是\"{0}\"分隔的 {1}个号码", Spliter_Level2, MinTotalNumber_Last_Tuo);
                }
                else
                {
                    errMsg = string.Format("后区号码必须是\"{0}\"分隔的 {1} - {2}个号码", Spliter_Level2, MinTotalNumber_Last_Tuo, MaxTotalNumber_Last_Tuo);
                }
                return false;
            }

            foreach (var item in arr_first_dan)
            {
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg, "F")) { return false; }
            }
            foreach (var item in arr_first_tuo)
            {
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg, "F")) { return false; }
            }
            foreach (var item in arr_last_dan)
            {
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg, "L")) { return false; }
            }
            foreach (var item in arr_last_tuo)
            {
                if (!orderAnalyzer.CheckOneAntecodeNumber(item, out errMsg, "L")) { return false; }
            }
            // 分组，以去除号码中的重复项
            var arr_first = arr_first_dan.Union(arr_first_tuo);
            if (arr_first.Count() != arr_first_dan.Length + arr_first_tuo.Length)
            {
                errMsg = "前区胆码与拖码有重复的数字 - " + antecode;
                return false;
            }
            if (arr_first.Count() < MinLength_First)
            {
                errMsg = "前区胆码与拖码个数必须大于 - " + (MinLength_First - 1);
                return false;
            }
            if (arr_first.Count() > MaxLength_First)
            {
                errMsg = "前区胆码与拖码个数必须小于 - " + (MaxLength_First + 1);
                return false;
            }
            var group_first = arr_first.GroupBy(c => c);
            if (group_first.Count() != arr_first.Count())
            {
                errMsg = "前区号码中有重复的数字 - " + antecode;
                return false;
            }
            // 分组，以去除号码中的重复项
            var arr_last = arr_last_dan.Union(arr_last_tuo);
            if (arr_last.Count() != arr_last_dan.Length + arr_last_tuo.Length)
            {
                errMsg = "前区胆码与拖码有重复的数字 - " + antecode;
                return false;
            }
            if (arr_last.Count() < MinLength_Last)
            {
                errMsg = "后区胆码与拖码个数必须大于 - " + (MinLength_Last - 1);
                return false;
            }
            if (arr_last.Count() > MaxLength_Last)
            {
                errMsg = "后区胆码与拖码个数必须小于 - " + (MaxLength_Last + 1);
                return false;
            }
            var group_last = arr_last.GroupBy(c => c);
            if (group_last.Count() != arr_last.Count())
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

            var arr_first_dan = AnteCodeNumbers[0].Split(Spliter_Level2);
            var arr_first_tuo = AnteCodeNumbers[1].Split(Spliter_Level2);
            string[] arr_last_dan, arr_last_tuo;
            if (NeedLastDan)
            {
                arr_last_dan = AnteCodeNumbers[2].Split(Spliter_Level2);
                arr_last_tuo = AnteCodeNumbers[3].Split(Spliter_Level2);
            }
            else
            {
                arr_last_dan = new string[0];
                arr_last_tuo = AnteCodeNumbers[2].Split(Spliter_Level2);
            }

            var len_first = BallNumber_First - arr_first_dan.Length;
            var len_last = BallNumber_Last - arr_last_dan.Length;

            c.Calculate(arr_first_tuo, len_first, (num) => i++);
            c.Calculate(arr_last_tuo, len_last, (num) => j++);

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
            var red_dan = AnteCodeNumbers[0].Split(Spliter_Level2);
            var red_tuo = AnteCodeNumbers[1].Split(Spliter_Level2);
            string[] blue_dan, blue_tuo;
            if (NeedLastDan)
            {
                blue_dan = AnteCodeNumbers[2].Split(Spliter_Level2);
                blue_tuo = AnteCodeNumbers[3].Split(Spliter_Level2);
            }
            else
            {
                blue_dan = new string[0];
                blue_tuo = AnteCodeNumbers[2].Split(Spliter_Level2);
            }

            var red_winNumber = checker.WinNumbers[0].Split(Spliter_Level2);
            var blue_winNumber = checker.WinNumbers[1].Split(Spliter_Level2);

            var list = new List<int>();
            var p = new Combination();
            p.Calculate(red_tuo, BallNumber_First - red_dan.Length, (item_red) =>
            {
                p.Calculate(blue_tuo, BallNumber_Last - blue_dan.Length, (item_blue) =>
                {
                    var redList = new List<string>(red_dan);
                    redList.AddRange(item_red);
                    var count_red = GetSameCodeCount(redList.ToArray(), red_winNumber);

                    var blueList = new List<string>(blue_dan);
                    blueList.AddRange(item_blue);
                    var count_blue = GetSameCodeCount(blueList.ToArray(), blue_winNumber);

                    switch (GameCode)
                    {
                        case "SSQ":
                            if (count_red == 6 && count_blue == 1)
                            {
                                list.Add(1);
                            }
                            else if (count_red == 6 && count_blue == 0)
                            {
                                list.Add(2);
                            }
                            else if (count_red == 5 && count_blue == 1)
                            {
                                list.Add(3);
                            }
                            else if ((count_red == 5 && count_blue == 0) || (count_red == 4 && count_blue == 1))
                            {
                                list.Add(4);
                            }
                            else if ((count_red == 4 && count_blue == 0) || (count_red == 3 && count_blue == 1))
                            {
                                list.Add(5);
                            }
                            else if (count_blue == 1)
                            {
                                list.Add(6);
                            }
                            break;
                        case "DLT":
                            if (count_red == 5 && count_blue == 2)
                            {
                                list.Add(1);
                            }
                            else if (count_red == 5 && count_blue == 1)
                            {
                                list.Add(2);
                            }
                            else if ((count_red == 5 && count_blue == 0) || (count_red == 4 && count_blue == 2))
                            {
                                list.Add(3);
                            }
                            //else if (count_red == 4 && count_blue == 2)
                            //{
                            //    list.Add(4);
                            //}
                            else if ((count_red == 4 && count_blue == 1) || (count_red == 3 && count_blue == 2))
                            {
                                list.Add(4);
                            }
                            //else if ((count_red == 4 && count_blue == 0) || (count_red == 3 && count_blue == 2))
                            //{
                            //    list.Add(6);
                            //}
                            else if ((count_red == 4 && count_blue == 0) || (count_red == 3 && count_blue == 1) || (count_red == 2 && count_blue == 2))
                            {
                                list.Add(5);
                            }
                            else if ((count_red == 3 && count_blue == 0) || (count_red == 1 && count_blue == 2) || (count_red == 2 && count_blue == 1) || (count_red == 0 && count_blue == 2))
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
