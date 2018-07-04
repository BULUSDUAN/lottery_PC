using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Interface;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 玩法分析 - 时时彩任选
    /// </summary>
    internal class GameTypeAnalyzer_时时彩任选 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_时时彩任选()
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
            if (AnteCodeNumbers.Length != TotalNumber)
            {
                errMsg = string.Format("投注号码必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter, TotalNumber);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            int c = 0;
            foreach (string code in AnteCodeNumbers)
            {
                if (code == "-")
                {
                    c++;
                }
                else
                {
                    if (!orderAnalyzer.CheckOneAntecodeNumber(code, out errMsg))
                    {
                        throw new AntecodeFormatException(GameCode, GameType, antecode, errMsg);
                    }
                }
            }
            if (c + BallNumber != TotalNumber)
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "号码格式错误 - " + antecode);
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
            bool isWin = true;
            for (int i = 0; i < TotalNumber; i++)
            {
                string code = AnteCodeNumbers[i];
                if (code == "-")
                {
                    continue;
                }
                string win = checker.WinNumbers[i];
                if (win != code)
                {
                    isWin = false;
                    break;
                }
            }
            if (isWin)
            {
                bonusLevelList.Add(0);
            }
            return bonusLevelList;
        }
    }
}
