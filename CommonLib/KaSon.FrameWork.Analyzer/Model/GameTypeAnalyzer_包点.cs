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
    /// 玩法分析 - 包点
    /// </summary>
    internal class GameTypeAnalyzer_包点 : IAntecodeAnalyzable
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
            int num;
            return CheckAntecode(antecode, out num, out errMsg);
        }
        private bool CheckAntecode(string antecode, out int num, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsNotEmptyString(antecode, "必须传入非空的投注号码");

            if (!int.TryParse(antecode, out num))
            {
                errMsg = "投注号码必须是整数";
                return false;
            }
            if (num < MinValue * BallNumber || num > MaxValue * BallNumber)
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
            int sum;
            string msg;
            if (!CheckAntecode(antecode, out sum, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, antecode, "投注号码格式错误 - " + msg);
            }
            int[] nums = new int[BallNumber];
            int i = 0;
            List<int[]> list = new List<int[]>();
            AnalyzeNumber(sum, 0, 0, nums, MinValue, MaxValue, (item) =>
            {
                int[] tmp = new int[item.Length];
                item.CopyTo(tmp, 0);
                if (!IsContain(list, tmp))
                {
                    i++;
                    list.Add(tmp);
                }
            });
            return i;
        }
        private bool IsContain(List<int[]> list, int[] target)
        {
            foreach (int[] src in list)
            {
                List<int> srcList = new List<int>(src);
                List<int> targetList = new List<int>(target);
                if (IsSame(srcList, targetList))
                {
                    return true;
                }
            }
            return false;
        }
        private bool IsSame(List<int> src, List<int> target)
        {
            for (int i = src.Count - 1; i >= 0; i--)
            {
                int index = target.IndexOf(src[i]);
                if (index >= 0)
                {
                    target.RemoveAt(index);
                }
                else
                {
                    return false;
                }
            }
            return true;
        }
        private void AnalyzeNumber(int target, int sum, int index, int[] nums, int minValue, int maxValue, Action<int[]> action)
        {
            for (int value = minValue; value <= maxValue; value++)
            {
                int temp = sum + value;
                nums[index] = value;
                if (index == nums.Length - 1)
                {
                    if (temp == target)
                    {
                        action(nums);
                        break;
                    }
                }
                else if (temp > target) { break; }
                else { AnalyzeNumber(target, temp, index + 1, nums, minValue, maxValue, action); }
            }
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
            var wins = checker.WinNumbers.Skip(TotalNumber - BallNumber).ToArray();
            var sum = int.Parse(antecode);
            int target = 0;
            foreach (var num in wins)
            {
                target += int.Parse(num);
            }
            var bonusLevelList = new List<int>();
            if (target == sum)
            {
                bonusLevelList.Add(GetBonusGrade(wins));
            }
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
