using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Interface;
using EntityModel.ExceptionExtend;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 玩法分析 - 直选好运
    /// </summary>
    internal class GameTypeAnalyzer_直选好运 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_直选好运()
        {
            Spliter = ',';
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public bool IsLast { get; set; }
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
            if (IsLast)    // 直选好运特  任意选择1个数字竞猜开奖号码的最后一位(后一),投注号码与开奖号码最后一位相同即中奖
            {
                if (AnteCodeNumbers.Contains(checker.WinNumbers.Last()))
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
            else
            {
                throw new ArgumentOutOfRangeException("直选好运不支持的玩法 - 选" + BallNumber);
            }
            return bonusLevelList.OrderBy((i) => i).ToList();
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
