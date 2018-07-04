using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Interface;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 玩法分析 - 连N连直
    /// </summary>
    internal class GameTypeAnalyzer_选N连直 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_选N连直()
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
            if (AnteCodeNumbers.Length != BallNumber)
            {
                errMsg = string.Format("投注号码必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter, TotalNumber);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            for (var i = 0; i < BallNumber; i++)
            {
                // 检查每一个号码
                if (!orderAnalyzer.CheckComboAntecodeNumber(AnteCodeNumbers[i], null, out errMsg)) { return false; }
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
            List<string[]> rlt = new List<string[]>();
            for (var i = 0; i < AnteCodeNumbers.Length; i++)
            {
                rlt.Add(AnteCodeNumbers[i].Split(' '));
            }
            int count = 0;
            var c = new ArrayCombination();
            c.Calculate(rlt.ToArray(), (item) =>
            {
                if (item.Distinct().Count() == BallNumber)
                {
                    count++;
                }
            });
            if (count == 0)
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误，不包含可用的号码组合 - " + msg);
            }
            return count;
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
            var c = new ArrayCombination();
            c.Calculate(rlt.ToArray(), (item) =>
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
            var index = winNumbers.ToList().IndexOf(anteCodes[0]);
            if (index < 0 || index > TotalNumber - BallNumber)
            {
                return false;
            }
            for (var i = 1; i < BallNumber && i < winNumbers.Length - 1; i++)
            {
                if (winNumbers[index + i] != anteCodes[i])
                {
                    return false;
                }
            }
            return true;
        }
    }
}
