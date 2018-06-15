using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;


namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 群英会任选
    /// </summary>
    internal class GameTypeAnalyzer_群英会任选 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_群英会任选()
        {
            Spliter = ',';
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
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

            AnteCodeNumbers = antecode.Split(Spliter);
            if (AnteCodeNumbers.Length < BallNumber)
            {
                errMsg = string.Format("投注号码必须至少包含 {0}个数字", BallNumber);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            if (!orderAnalyzer.CheckComboAntecodeNumber(antecode, ',', out errMsg))
            {
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
            c.Calculate(AnteCodeNumbers, BallNumber, (num) => i++);
            return i;
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
            if (BallNumber == 3 || BallNumber == 4)
            {
                var p = new Combination();
                p.Calculate(AnteCodeNumbers, BallNumber, (num) =>
                {
                    if (IsWin_Less(checker.WinNumbers, num))
                    {
                        bonusLevelList.Add(1);
                    }
                    else
                    {
                        p.Calculate(num, BallNumber - 1, (item) =>
                        {
                            if (IsWin_Less(checker.WinNumbers, item))
                            {
                                bonusLevelList.Add(2);
                                return false;
                            }
                            return true;
                        });
                    }
                });
            }
            else if (BallNumber == 5)
            {
                var p = new Combination();
                p.Calculate(AnteCodeNumbers, BallNumber, (num) =>
                {
                    if (IsWin_Less(checker.WinNumbers, num))
                    {
                        bonusLevelList.Add(1);
                    }
                    else
                    {
                        var isWin = false;
                        p.Calculate(num, BallNumber - 1, (item) =>
                        {
                            if (IsWin_Less(checker.WinNumbers, item))
                            {
                                isWin = true;
                                bonusLevelList.Add(2);
                                return false;
                            }
                            return true;
                        });
                        if (!isWin)
                        {
                            p.Calculate(num, BallNumber - 2, (item) =>
                            {
                                if (IsWin_Less(checker.WinNumbers, item))
                                {
                                    bonusLevelList.Add(3);
                                    return false;
                                }
                                return true;
                            });
                        }
                    }
                });
            }
            else if (BallNumber <= TotalNumber)     // 任选的数量小于等于总号码数。如：任选四、任选五
            {
                var p = new Combination();
                p.Calculate(AnteCodeNumbers, BallNumber, (num) =>
                {
                    if (IsWin_Less(checker.WinNumbers, num))
                    {
                        bonusLevelList.Add(0);
                    }
                });
            }
            else    // 任选的数量大于等于总号码数。如：任选七
            {
                var winTime = 0;
                foreach (var item in checker.WinNumbers)
                {
                    if (AnteCodeNumbers.Contains(item)) winTime++;
                }
                if (winTime == TotalNumber)
                {
                    var p = new Combination();
                    p.Calculate(AnteCodeNumbers.Take(AnteCodeNumbers.Length - 5).ToArray(), BallNumber - 5, (item) =>
                    {
                        bonusLevelList.Add(0);
                    });
                }
            }
            return bonusLevelList;
        }
        private bool IsWin_Less(string[] winNumber, string[] anteNumber)
        {
            bool isWin = true;
            foreach (var item in anteNumber)
            {
                if (!winNumber.Contains(item))
                {
                    isWin = false;
                    break;
                }
            }
            return isWin;
        }
    }
}
