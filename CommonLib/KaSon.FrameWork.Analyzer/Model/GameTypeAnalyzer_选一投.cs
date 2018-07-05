using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Interface;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Utilities;
using KaSon.FrameWork.Common.Algorithms;

namespace  KaSon.FrameWork.Analyzer.Model
{
    /// <summary>
    /// 玩法分析 - 选一投
    /// </summary>
    internal class GameTypeAnalyzer_选一投 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_选一投()
        {
            Spliter = ',';
            BallNumber = 1;
            MaxBallNumber = 0;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MaxBallNumber { get; set; }
        public int MinNumber { get; set; }
        public int MaxNumber { get; set; }
        public bool IsRed { get; set; }
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
            if (MaxBallNumber != 0 && AnteCodeNumbers.Length > MaxBallNumber)
            {
                errMsg = string.Format("投注号码必须最多包含 {0}个数字", MaxBallNumber);
                return false;
            }
            var orderAnalyzer = KaSon.FrameWork.Analyzer.AnalyzerFactory.AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            if (!orderAnalyzer.CheckComboAntecodeNumber(antecode, ',', out errMsg))
            {
                return false;
            }
            foreach (var item in AnteCodeNumbers)
            {
                var num = int.Parse(item);
                if (num < MinNumber || num > MaxNumber)
                {
                    errMsg = string.Format("号码必须是 {0,2:D2} - {1,2:D2} 之间的数字", MinNumber, MaxNumber);
                    return false;
                }
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
            var checker = KaSon.FrameWork.Analyzer.AnalyzerFactory.AnalyzerFactory.GetWinNumberAnalyzer(GameCode, GameType);
            if (!checker.CheckWinNumber(winNumber, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, winNumber, "中奖号码格式错误 - " + msg);
            }
            var bonusLevelList = new List<int>();
            if (IsRed)    // 选一红投
            {
                var win = int.Parse(checker.WinNumbers[0]);
                if (win >= MinNumber && win <= MaxNumber)
                {
                    bonusLevelList.Add(0);
                }
            }
            else    // 选一数投
            {
                if (AnteCodeNumbers.Contains(checker.WinNumbers[0]))
                {
                    bonusLevelList.Add(0);
                }
            }
            return bonusLevelList;
        }
    }
}
