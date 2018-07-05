using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;
using KaSon.FrameWork.Common.Algorithms;
using KaSon.FrameWork.Common.Utilities;

namespace  KaSon.FrameWork.Analyzer.Model
{
    /// <summary>
    /// 玩法分析 - 单式
    /// </summary>
    internal class GameTypeAnalyzer_单式 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_单式()
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
            var orderAnalyzer = KaSon.FrameWork.Analyzer.AnalyzerFactory.AnalyzerFactory.GetOrderAnalyzer(GameCode, GameType);
            var i = 0;
            for (; i < TotalNumber - BallNumber; i++)
            {
                // 检查空位占位符
                if (!orderAnalyzer.CheckSpaceNumber(AnteCodeNumbers[i], out errMsg)) { return false; }
            }
            for (; i < TotalNumber; i++)
            {
                // 检查每一个号码
                if (!orderAnalyzer.CheckComboAntecodeNumber(AnteCodeNumbers[i], null, out errMsg)) { return false; }
                // 分组，以去除号码中的重复项
                var group = AnteCodeNumbers[i].GroupBy(c => c);
                if (group.Count() != AnteCodeNumbers[i].Length)
                {
                    errMsg = "号码中有重复的数字 - " + antecode;
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
            var count = 1;
            for (var i = TotalNumber - BallNumber; i < TotalNumber; i++)
            {
                count *= AnteCodeNumbers[i].Length;
            }
            return count;
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
            // 江西时时彩四星单选特殊处理奖等
            if (GameCode == "JXSSC" && BallNumber == 4)
            {
                return Caculate_JXSSC_4XDX(AnteCodeNumbers, checker.WinNumbers);
            }
            else
            {
                var bonusLevelList = new List<int>();
                for (var i = TotalNumber - BallNumber; i < TotalNumber; i++)
                {
                    if (!AnteCodeNumbers[i].Contains(checker.WinNumbers[i]))
                    {
                        return bonusLevelList;
                    }
                }
                bonusLevelList.Add(0);
                return bonusLevelList;
            }
        }
        private IList<int> Caculate_JXSSC_4XDX(string[] codes, string[] wins)
        {
            var bonusLevelList = new List<int>();
            List<string[]> rlt = new List<string[]>();
            for (var i = 0; i < codes.Length; i++)
            {
                var cList = AnteCodeNumbers[i].ToArray();
                var sList = new string[cList.Length];
                for (var j = 0; j < cList.Length; j++)
                {
                    sList[j] = cList[j].ToString();
                }
                rlt.Add(sList);
            }
            var c = new ArrayCombination();
            c.Calculate(rlt.ToArray(), (item) =>
            {
                var level = CheckIsWin_JXSSC_4XDX(item, wins);
                if (level > 0)
                {
                    bonusLevelList.Add(level);
                }
            });
            return bonusLevelList.OrderBy((i) => i).ToList();
        }
        private int CheckIsWin_JXSSC_4XDX(string[] codes, string[] wins)
        {
            bool[] matchArr = new bool[5];
            for (int i = 0; i < 5; i++)
            {
                string code = codes[i];
                string win = wins[i];
                matchArr[i] = code.Contains(win);
            }
            var bonusLevelList = new List<int>();
            if (matchArr[1] && matchArr[2] && matchArr[3] && matchArr[4])
            {
                return 1; // 四星全中
            }
            else if (matchArr[1] && matchArr[2] && matchArr[3])
            {
                return 2; // 前三
            }
            else if (matchArr[2] && matchArr[3] && matchArr[4])
            {
                return 2; // 后三
            }
            else
            {
                return 0;
            }
        }
    }
}
