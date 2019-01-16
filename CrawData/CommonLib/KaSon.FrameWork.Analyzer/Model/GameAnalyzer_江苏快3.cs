using EntityModel.ExceptionExtend;
using EntityModel.Interface;
using KaSon.FrameWork.Common.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace  KaSon.FrameWork.Analyzer.Model
{
    internal class GameAnalyzer_江苏快3 : IOrderAnalyzable, IWinNumberAnalyzable
    {
        public GameAnalyzer_江苏快3(string gameCode)
        {
            GameCode = gameCode;
            Spliter = ',';
            TotalNumber = 3;
        }
        public string GameCode { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public string[] WinNumbers { get; private set; }

        /// <summary>
        /// 检查一个单独号码的格式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckOneAntecodeNumber(string antecodeNumber, out string errMsg, string type = null)
        {
            if (antecodeNumber.Length != 1)
            {
                errMsg = "号码必须用一位数字表示";
                return false;
            }
            UInt16 number;
            if (UInt16.TryParse(antecodeNumber, out number))
            {
                if (number < 1 || number > 6)
                {
                    errMsg = "号码必须是 1 - 6 之间的数字";
                    return false;
                }
            }
            else
            {
                errMsg = "号码必须是 1 - 6 之间的数字";
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
            if (arr.Count > 6)
            {
                errMsg = "最多6个号码 - " + antecodeNumber;
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
            throw new Exception("江苏快3玩法不存在占位符");
        }

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
            var lastNumber = 0;
            foreach (var num in WinNumbers)
            {
                if (int.Parse(num) < lastNumber)
                    throw new AntecodeFormatException(GameCode, GameCode, winNumber, "中奖号码格式错误");
                // 检查每一个号码
                if (!CheckOneAntecodeNumber(num, out errMsg)) { return false; }
                lastNumber = int.Parse(num);
            }
            errMsg = "";
            return true;
        }
    }
}
