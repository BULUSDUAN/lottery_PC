using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Interface;
using EntityModel.ExceptionExtend;
namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    /// <summary>
    /// 玩法分析 - 组选和值
    /// </summary>
    internal class GameTypeAnalyzer_组选和值 : IAntecodeAnalyzable
    {
        public string GameCode { get; set; }
        public string GameType { get; set; }

        public int TotalNumber { get; set; }
        public int BallNumber { get; set; }
        public int MinValue { get; set; }
        public int MaxValue { get; set; }
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

            int i;
            if (!int.TryParse(antecode, out i))
            {
                errMsg = "投注号码必须是整数";
                return false;
            }
            if (i < MinValue * BallNumber || i > MaxValue * BallNumber)
            {
                errMsg = string.Format("投注号码必须是介于{0}与{1}之间的数字", MinValue * BallNumber, MaxValue * BallNumber);
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
            int[] nums = new int[BallNumber];
            var sum = int.Parse(antecode);
            if (sum == 1)
                return 1;
            else if (sum == 2)
                return 2;
            else if (sum == 3)
                return 2;
            else if (sum == 4)
                return 4;
            else if (sum == 5)
                return 5;
            else if (sum == 6)
                return 6;
            else if (sum == 7)
                return 8;
            else if (sum == 8)
                return 10;
            else if (sum == 9)
                return 11;
            else if (sum == 10)
                return 13;
            else if (sum == 11)
                return 14;
            else if (sum == 12)
                return 14;
            else if (sum == 13)
                return 15;
            else if (sum == 14)
                return 15;
            else if (sum == 15)
                return 14;
            else if (sum == 16)
                return 14;
            else if (sum == 17)
                return 13;
            else if (sum == 18)
                return 11;
            else if (sum == 19)
                return 10;
            else if (sum == 20)
                return 8;
            else if (sum == 21)
                return 6;
            else if (sum == 22)
                return 5;
            else if (sum == 23)
                return 4;
            else if (sum == 24)
                return 2;
            else if (sum == 25)
                return 2;
            else if (sum == 26)
                return 1;
            else
                return 0;
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
            var sum = int.Parse(antecode);
            int target = 0;
            for (int i = 1; i <= BallNumber; i++)
            {
                target += int.Parse(checker.WinNumbers[TotalNumber - i]);
            }
            if (target == sum)
            {
                bonusLevelList.Add(0);
            }
            return bonusLevelList;
        }
    }
}
