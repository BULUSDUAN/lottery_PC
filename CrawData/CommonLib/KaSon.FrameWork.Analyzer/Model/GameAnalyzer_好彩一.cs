using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EntityModel.Interface;
using KaSon.FrameWork.Common.Utilities;

namespace  KaSon.FrameWork.Analyzer.Model
{
    internal class GameAnalyzer_好彩一 : IOrderAnalyzable, IWinNumberAnalyzable
    {
        public GameAnalyzer_好彩一(string gameCode)
        {
            GameCode = gameCode;
            Spliter = ',';
            TotalNumber = 5;
            NeedCheckRepeat = true;
        }
        public string GameCode { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public string[] WinNumbers { get; private set; }
        public bool NeedCheckRepeat { get; set; }

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
            foreach (var num in WinNumbers)
            {
                // 检查每一个号码
                if (!CheckOneAntecodeNumber(num, out errMsg)) { return false; }
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
            UInt16 number;
            if (UInt16.TryParse(antecodeNumber, out number))
            {
                if (number < 1 || number > 36)
                {
                    errMsg = "号码必须是 1 - 36 之间的数字";
                    return false;
                }
            }
            else
            {
                errMsg = "号码必须是 1 - 36 之间的数字";
                return false;
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
            IList<string> arr;
            if (spliter.HasValue)
            {
                arr = antecodeNumber.Split(spliter.Value);
            }
            else
            {
                arr = new List<string>();
                antecodeNumber.ToList().ForEach((c) =>
                {
                    arr.Add(c.ToString());
                });
            }
            if (arr.Count > 10)
            {
                errMsg = "最多10个号码 - " + antecodeNumber;
                return false;
            }
            foreach (var c in arr)
            {
                if (!CheckOneAntecodeNumber(c, out errMsg))
                {
                    return false;
                }
            }
            errMsg = "";
            return true;
        }
        /// <summary>
        /// 检查占位符的格式
        /// </summary>
        /// <param name="antecodeNumber">占位符</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckSpaceNumber(string antecodeNumber, out string errMsg)
        {
            if (antecodeNumber != "-")
            {
                errMsg = "补位的号码必须是由\"-\"代替";
                return false;
            }
            errMsg = "";
            return true;
        }
    }
}
