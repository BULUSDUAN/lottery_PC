using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;
using EntityModel.Interface;
using KaSon.FrameWork.Common.Utilities;

namespace  KaSon.FrameWork.Analyzer.Model
{
    /// <summary>
    /// 江苏快3 和值分析
    /// </summary>
    internal class GameTypeAnalyzer_江苏快3和值 : IAntecodeAnalyzable
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
            if (i <= MinValue * BallNumber || i >= MaxValue * BallNumber)
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
            return 1;
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
            var sum = int.Parse(antecode);
            int target = 0;
            for (int i = 1; i <= BallNumber; i++)
            {
                target += int.Parse(checker.WinNumbers[TotalNumber - i]);
            }
            if (target == sum)
            {
                switch (sum)
                {
                    case 3:
                    case 18:
                        bonusLevelList.Add(1);
                        break;
                    case 4:
                    case 17:
                        bonusLevelList.Add(2);
                        break;
                    case 5:
                    case 16:
                        bonusLevelList.Add(3);
                        break;
                    case 6:
                    case 15:
                        bonusLevelList.Add(4);
                        break;
                    case 7:
                    case 14:
                        bonusLevelList.Add(5);
                        break;
                    case 8:
                    case 13:
                        bonusLevelList.Add(6);
                        break;
                    case 9:
                    case 12:
                        bonusLevelList.Add(7);
                        break;
                    case 10:
                    case 11:
                        bonusLevelList.Add(8);
                        break;
                }
            }
            return bonusLevelList;
        }
    }
}
