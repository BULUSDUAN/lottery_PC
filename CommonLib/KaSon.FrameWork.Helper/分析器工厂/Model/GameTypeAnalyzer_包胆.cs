using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;
using KaSon.FrameWork.Helper.分析器工厂;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 包胆
    /// </summary>
    internal class GameTypeAnalyzer_包胆 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_包胆()
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
            if (AnteCodeNumbers.Length >= BallNumber)
            {
                errMsg = string.Format("投注号码最多只能是被\"{0}\"切分成 {1}个部分的字符串", Spliter, (BallNumber - 1));
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
            if (BallNumber == 2)
            {
                return 10;
            }
            else if (BallNumber == 3)
            {
                if (AnteCodeNumbers.Length == 1)
                {
                    return 55;
                }
                else if (AnteCodeNumbers.Length == 2)
                {
                    return 10;
                }
                else
                {
                    throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - 号码超出最大个数限制");
                }
            }
            else
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "暂不支持四星包胆玩法");
            }
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
            var tmp = checker.WinNumbers.Skip(TotalNumber - BallNumber).ToArray();
            var wins = new List<string>(tmp);
            var codes = AnteCodeNumbers.ToList();
            bool isWin = true;
            for (int i = codes.Count - 1; i >= 0; i--)
            {
                var index = wins.IndexOf(codes[i]);
                if (index < 0)
                {
                    isWin = false;
                    break;
                }
                else
                {
                    wins.RemoveAt(index);
                    codes.RemoveAt(i);
                    //i -= codes.RemoveAll((item) => item.Equals(codes[i]));
                }
            }
            var bonusLevelList = new List<int>();
            if (isWin) { bonusLevelList.Add(GetBonusGrade(tmp)); }
            return bonusLevelList;
        }
        private int GetBonusGrade(string[] wins)
        {
            var group = wins.GroupBy(n => n);
            var count = group.Count();
            return count;
        }
    }
}
