
using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.ExceptionExtend;
using KaSon.FrameWork.Common.Utilities;
namespace  KaSon.FrameWork.Analyzer.Model
{
    internal class GameAnalyzer_传统足球 : IOrderAnalyzable, IWinNumberAnalyzable
    {
        public GameAnalyzer_传统足球(string gameCode)
        {
            GameCode = gameCode;
            Spliter = ',';
            Wildcard = "*";
            TotalNumber = 14;
        }
        public string GameCode { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        // 是否允许通配符
        public bool IsEnableWildcard { get; set; }
        // 通配符
        public string Wildcard { get; set; }
        public int BallNumber { get; set; }
        public string[] WinNumbers { get; private set; }
        // 有效投注号码
        public string EffectiveAnteCode { get; set; }
        public string EffectiveWinNumber { get; set; }

        /// <summary>
        /// 检查中奖号码格式是否正确
        /// </summary>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckWinNumber(string winNumber, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查中奖号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(winNumber, "必须传入非空的中奖号码");

            WinNumbers = winNumber.Split(Spliter);
            if (WinNumbers.Length != TotalNumber)
            {
                errMsg = string.Format("中奖号码必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter, TotalNumber);
                return false;
            }
            var count = WinNumbers.Count(a => a.Length != 1 || (!EffectiveWinNumber.Contains(a) && IsEnableWildcard && a != Wildcard));
            if (count > 0)
            {
                errMsg = string.Format("中奖号码必须是由{0}场比赛结果数字\"{1}\"组成的字符串", TotalNumber, string.Join(",", EffectiveWinNumber.ToArray()));
                return false;
            }
            if (BallNumber == 14 || BallNumber == 9)
            {
                if (WinNumbers.Count(a => a.Equals(Wildcard)) > 6)
                {
                    errMsg = string.Format("中奖号码超过6场没有比赛结果，{0}", string.Join(",", WinNumbers));
                    return false;
                }
            }
            errMsg = "";
            return true;
        }
        /// <summary>
        /// 检查一个单独号码的格式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckOneAntecodeNumber(string antecodeNumber, out string errMsg, string type = null)
        {
            if (antecodeNumber.Length != antecodeNumber.GroupBy(c => c).Count())
            {
                errMsg = "有重复的投注号码 - " + antecodeNumber;
                return false;
            }
            foreach (var item in antecodeNumber)
            {
                if (!EffectiveAnteCode.Contains(item))
                {
                    errMsg = "号码必须在\"" + string.Join(",", EffectiveAnteCode.ToArray()) + "\"范围内 - " + antecodeNumber;
                    return false;
                }
            }
            errMsg = "";
            return true;
        }
        /// <summary>
        /// 检查一个组合号码的格式，如：时时彩中的复式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="spliter">分隔符</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckComboAntecodeNumber(string antecodeNumber, char? spliter, out string errMsg)
        {
            throw new Exception("传统足球不存在组合号码");
        }
        /// <summary>
        /// 检查占位符的格式
        /// </summary>
        /// <param name="antecodeNumber">占位符</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckSpaceNumber(string antecodeNumber, out string errMsg)
        {
            throw new Exception("传统足球不存在占位符");
        }
    }
}
