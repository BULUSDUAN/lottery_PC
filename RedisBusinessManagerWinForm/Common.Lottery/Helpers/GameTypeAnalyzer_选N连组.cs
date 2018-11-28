using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Utilities;
using Common.Algorithms;

namespace Common.Lottery.Helpers
{
    /// <summary>
    /// 玩法分析 - 连N连组
    /// </summary>
    internal class GameTypeAnalyzer_选N连组 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_选N连组()
        {
            Spliter = ',';
            TotalNumber = 5;
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
            AnalyzeAnteCode(antecode);

            string msg;
            var checker = AnalyzerFactory.GetWinNumberAnalyzer(GameCode, GameType);
            if (!checker.CheckWinNumber(winNumber, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, winNumber, "中奖号码格式错误 - " + msg);
            }
            List<string[]> rlt = new List<string[]>();
            for (var i = 0; i < AnteCodeNumbers.Length; i++)
            {
                rlt.Add(AnteCodeNumbers[i].Split(' '));
            }
            var bonusLevelList = new List<int>();
            var c = new Combination();
            c.Calculate(AnteCodeNumbers, BallNumber, (item) =>
            {
                if (item.Distinct().Count() == BallNumber)
                {
                    if (IsWin(item, checker.WinNumbers))
                    {
                        bonusLevelList.Add(0);
                    }
                }
            });
            return bonusLevelList;
        }
        private bool IsWin(string[] anteCodes, string[] winNumbers)
        {
            for (var i = 0; i <= TotalNumber - BallNumber; i++)
            {
                var winItem = winNumbers.Skip(i).Take(BallNumber).ToArray();
                if (IsMatch(anteCodes, winItem))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsMatch(string[] anteCodes, string[] winNumbers)
        {
            foreach (var item in anteCodes)
            {
                if (!winNumbers.Contains(item))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
