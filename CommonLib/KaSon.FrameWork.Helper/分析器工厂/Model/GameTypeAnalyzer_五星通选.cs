using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 五星通选
    /// </summary>
    internal class GameTypeAnalyzer_五星通选 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_五星通选()
        {
            Spliter = ',';
            TotalNumber = 5;
            BallNumber = 5;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        private int TotalNumber { get; set; }
        private int BallNumber { get; set; }
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
            for (var i = 0; i < TotalNumber; i++)
            {
                // 检查每一个号码
                if (!orderAnalyzer.CheckOneAntecodeNumber(AnteCodeNumbers[i], out errMsg)) { return false; }
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
            bool[] matchArr = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                matchArr[i] = AnteCodeNumbers[i] == checker.WinNumbers[i];
            }
            if (matchArr[0] && matchArr[1] && matchArr[2] && matchArr[3] && matchArr[4])
            {
                // 全中
                bonusLevelList.Add(1);
            }
            if (matchArr[0] && matchArr[1] && matchArr[2])
            {
                // 前三
                bonusLevelList.Add(2);
            }
            if (matchArr[2] && matchArr[3] && matchArr[4])
            {
                // 后三
                bonusLevelList.Add(2);
            }
            if (matchArr[0] && matchArr[1])
            {
                // 前二
                bonusLevelList.Add(3);
            }
            if (matchArr[3] && matchArr[4])
            {
                // 后二
                bonusLevelList.Add(3);
            }
            return bonusLevelList;
        }
    }
}
