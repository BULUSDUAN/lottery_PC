using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;
using KaSon.FrameWork.Helper.分析器工厂;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 二星组选复式
    /// </summary>
    internal class GameTypeAnalyzer_二星组选复式 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_二星组选复式()
        {
            Spliter = ',';
            AllNumber = 10;
            TotalNumber = 5;
            BallNumber = 2;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        private int AllNumber { get; set; }
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
            if (AnteCodeNumbers.Length < BallNumber || AnteCodeNumbers.Length > AllNumber)
            {
                errMsg = string.Format("投注号码必须是由\"{0}\"连接的 {1} - {2}个数字组成", Spliter, BallNumber, AllNumber);
                return false;
            }
            var orderAnalyzer = AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            // 检查每一个号码
            if (!orderAnalyzer.CheckComboAntecodeNumber(antecode, Spliter, out errMsg)) { return false; }

            var groupCount = AnteCodeNumbers.GroupBy(a => a).Count();
            if (groupCount != AnteCodeNumbers.Length)
            {
                errMsg = string.Format("投注号码有重复数字出现：{0}", antecode);
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
            int i = 0;
            var p = new Combination();
            p.Calculate(AnteCodeNumbers, BallNumber, (item) =>
            {
                i++;
            });
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
            var wins = checker.WinNumbers.Skip(TotalNumber - BallNumber).ToArray();
            var isWin = true;
            // 分组，以去除号码中的重复项
            var group = wins.GroupBy(c => c);
            if (group.Count() != wins.Length)
            {
                isWin = false;
            }
            else
            {
                foreach (var code in wins)
                {
                    if (!AnteCodeNumbers.Contains(code))
                    {
                        isWin = false;
                        break;
                    }
                }
            }
            var bonusLevelList = new List<int>();
            if (isWin) { bonusLevelList.Add(0); }
            return bonusLevelList;
        }
    }
}
