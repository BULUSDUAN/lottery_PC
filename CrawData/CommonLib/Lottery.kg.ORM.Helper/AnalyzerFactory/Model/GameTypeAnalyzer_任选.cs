using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;


namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 玩法分析 - 任选
    /// </summary>
    internal class GameTypeAnalyzer_任选 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_任选()
        {
            Spliter = ',';
            MinCount = -1;
            MaxCount = -1;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MinCount { get; set; }
        public int MaxCount { get; set; }
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
            if (MinCount == -1 && MaxCount == -1)
            {
                if (AnteCodeNumbers.Length < BallNumber)
                {
                    errMsg = string.Format("投注号码必须至少包含 {0}个数字", BallNumber);
                    return false;
                }
            }
            else
            {
                if (AnteCodeNumbers.Length < MinCount || AnteCodeNumbers.Length > MaxCount)
                {
                    if (MinCount == MaxCount)
                    {
                        errMsg = string.Format("投注号码必须包含 {0}个数字", MinCount);
                    }
                    else
                    {
                        errMsg = string.Format("投注号码必须至少包含 {0} - {1}个数字", MinCount, MaxCount);
                    }
                    return false;
                }
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
            if (BallNumber == 1)    // 任选一，定位到中奖号码的第一位进行比较
            {
                if (AnteCodeNumbers.Contains(checker.WinNumbers[0]))
                {
                    bonusLevelList.Add(0);
                }
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
