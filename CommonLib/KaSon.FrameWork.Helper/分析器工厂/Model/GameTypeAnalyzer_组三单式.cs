using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 组三单式
    /// </summary>
    internal class GameTypeAnalyzer_组三单式 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_组三单式()
        {
            Spliter = ',';
            TotalNumber = 5;
            BallNumber = 3;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
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
            if (AnteCodeNumbers.Length != BallNumber)
            {
                errMsg = string.Format("投注号码必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter, TotalNumber);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            for (var i = 0; i < BallNumber; i++)
            {
                // 检查每一个号码
                if (!orderAnalyzer.CheckOneAntecodeNumber(AnteCodeNumbers[i], out errMsg)) { return false; }
            }
            var groupCount = AnteCodeNumbers.GroupBy(a => a).Count();
            if (groupCount == 1)
            {
                errMsg = string.Format("投注号码不能是三个数字都相同的豹子：", antecode);
                return false;
            }
            else if (groupCount == 3)
            {
                errMsg = string.Format("投注号码不能是三个数字都不相同的组六：", antecode);
                return false;
            }
            else if (groupCount == 2)
            {
                errMsg = "";
                return true;
            }
            else
            {
                errMsg = string.Format("分析投注号码未知错误：", antecode);
                return false;
            }
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
            var wins = checker.WinNumbers.Skip(TotalNumber - BallNumber).ToArray();
            var bonusLevelList = new List<int>();
            var groupWin = wins.GroupBy(a => a);
            if (groupWin.Count() == 2)
            {
                var groupNum = AnteCodeNumbers.GroupBy(a => a);
                if (CheckIsWin(groupWin, groupNum)) { bonusLevelList.Add(0); }
            }
            return bonusLevelList;
        }
        private bool CheckIsWin(IEnumerable<IGrouping<string, string>> groupWin, IEnumerable<IGrouping<string, string>> groupNum)
        {
            string win_More = "", win_Less = "", num_More = "", num_Less = "";
            foreach (var win in groupWin)
            {
                if (win.Count() == 1) { win_Less = win.Key; }
                else if (win.Count() == 2) { win_More = win.Key; }
            }
            foreach (var num in groupNum)
            {
                if (num.Count() == 1) { num_Less = num.Key; }
                else if (num.Count() == 2) { num_More = num.Key; }
            }
            if (win_Less == "" || win_More == "" || num_Less == "" || num_More == "")
            {
                return false;
            }
            return (win_More == num_More && win_Less == num_Less);
        }
    }
}
