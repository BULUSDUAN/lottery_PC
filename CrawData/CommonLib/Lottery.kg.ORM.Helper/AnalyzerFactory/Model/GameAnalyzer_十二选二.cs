using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace KaSon.FrameWork.ORM.Helper.AnalyzerFactory.Model
{
    internal class GameAnalyzer_十二选二 : IOrderAnalyzable, IWinNumberAnalyzable
    {
        public GameAnalyzer_十二选二(string gameCode)
        {
            GameCode = gameCode;
            Spliter = ',';
            TotalNumber = 2;
        }
        public string GameCode { get; set; }
        public char Spliter { get; set; }
        public int TotalNumber { get; set; }
        public string[] WinNumbers { get; private set; }
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
            if (!CheckComboAntecodeNumber(winNumber, ',', out errMsg)) { return false; }
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
            if (antecodeNumber.Length != 2)
            {
                errMsg = "号码必须用二位数字（不足二位的，前置补 0，如：02）表示";
                return false;
            }
            UInt16 number;
            if (UInt16.TryParse(antecodeNumber, out number))
            {
                if (number < 1 || number > 12)
                {
                    errMsg = "号码必须是 01 - 12 之间的数字";
                    return false;
                }
            }
            else
            {
                errMsg = "号码必须是 01 - 12 之间的数字";
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
            var codes = antecodeNumber.Split(spliter.HasValue ? spliter.Value : ' ');
            if (codes.Length > 12)
            {
                errMsg = "最多12个号码 - " + antecodeNumber;
                return false;
            }
            // 分组，以去除号码中的重复项
            var group = codes.GroupBy(c => c);
            if (group.Count() != codes.Length)
            {
                errMsg = "号码中有重复的数字 - " + antecodeNumber;
                return false;
            }
            foreach (var c in codes)
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
            throw new Exception("十二选二玩法不存在占位符");
        }
    }
}
