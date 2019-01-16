using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    internal class GameAnalyzer_大盘数字彩 : IOrderAnalyzable, IWinNumberAnalyzable
    {
        public GameAnalyzer_大盘数字彩(string gameCode)
        {
            GameCode = gameCode;
            Spliter_Level1 = '|';
            Spliter_Level2 = ',';
            TotalNumber_First = 6;
            TotalNumber_Last = 1;

            MinNumber_First = 1;
            MaxNumber_First = 33;
            MinNumber_Last = 1;
            MaxNumber_Last = 16;
        }
        public string GameCode { get; set; }
        public char Spliter_Level1 { get; set; }
        public char Spliter_Level2 { get; set; }
        public int TotalNumber_First { get; set; }
        public int TotalNumber_Last { get; set; }
        public string[] WinNumbers { get; private set; }
        public string[] WinNumbers_First { get; private set; }
        public string[] WinNumbers_Last { get; private set; }
        public int MinNumber_First { get; set; }
        public int MaxNumber_First { get; set; }
        public int MinNumber_Last { get; set; }
        public int MaxNumber_Last { get; set; }

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

            WinNumbers = winNumber.Split(Spliter_Level1);
            if (WinNumbers.Length != 2)
            {
                errMsg = string.Format("中奖号码必须是被\"{0}\"切分成前后两区的字符串", Spliter_Level1);
                return false;
            }
            WinNumbers_First = WinNumbers[0].Split(Spliter_Level2);
            if (WinNumbers_First.Length != TotalNumber_First)
            {
                errMsg = string.Format("中奖号码前区必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter_Level2, TotalNumber_First);
                return false;
            }
            WinNumbers_Last = WinNumbers[1].Split(Spliter_Level2);
            if (WinNumbers_Last.Length != TotalNumber_Last)
            {
                errMsg = string.Format("中奖号码后区必须是被\"{0}\"切分成 {1}个部分的字符串", Spliter_Level2, TotalNumber_Last);
                return false;
            }
            foreach (var num in WinNumbers_First)
            {
                // 检查每一个号码
                if (!CheckOneAntecodeNumber_First(num, out errMsg)) { return false; }
            }
            errMsg = "";
            return true;
        }
        public bool CheckOneAntecodeNumber_First(string antecodeNumber, out string errMsg)
        {
            if (antecodeNumber.Length != 2)
            {
                errMsg = "前区号码必须用两位数字表示";
                return false;
            }
            UInt16 number;
            if (UInt16.TryParse(antecodeNumber, out number))
            {
                if (number < MinNumber_First || number > MaxNumber_First)
                {
                    errMsg = string.Format("前区号码必须是 {0,2:D2} - {1,2:D2} 之间的数字", MinNumber_First, MaxNumber_First);
                    return false;
                }
            }
            else
            {
                errMsg = string.Format("前区号码必须是 {0,2:D2} - {1,2:D2} 之间的数字", MinNumber_First, MaxNumber_First);
                return false;
            }
            errMsg = "";
            return true;
        }
        public bool CheckOneAntecodeNumber_Last(string antecodeNumber, out string errMsg)
        {
            if (antecodeNumber.Length != 2)
            {
                errMsg = "后区号码必须用两位数字表示";
                return false;
            }
            UInt16 number;
            if (UInt16.TryParse(antecodeNumber, out number))
            {
                if (number < MinNumber_Last || number > MaxNumber_Last)
                {
                    errMsg = string.Format("后区号码必须是 {0,2:D2} - {1,2:D2} 之间的数字", MinNumber_Last, MaxNumber_Last);
                    return false;
                }
            }
            else
            {
                errMsg = string.Format("后区号码必须是 {0,2:D2} - {1,2:D2} 之间的数字", MinNumber_Last, MaxNumber_Last);
                return false;
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
        public bool CheckOneAntecodeNumber(string antecodeNumber, out string errMsg, string type)
        {
            switch (type)
            {
                case "F":
                    return CheckOneAntecodeNumber_First(antecodeNumber, out errMsg);
                case "L":
                    return CheckOneAntecodeNumber_Last(antecodeNumber, out errMsg);
                default:
                    throw new Exception("不支持的号码类型 - " + type);
            }
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
            throw new Exception("双色球不存在检查组合号码的格式");
        }
        /// <summary>
        /// 检查占位符的格式
        /// </summary>
        /// <param name="antecodeNumber">占位符</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckSpaceNumber(string antecodeNumber, out string errMsg)
        {
            throw new Exception("双色球不存在占位符");
        }
    }
}
