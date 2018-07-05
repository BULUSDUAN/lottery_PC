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
    /// 玩法分析 - 大小单双
    /// </summary>
    internal class GameTypeAnalyzer_大小单双 : IAntecodeAnalyzable
    {
        public GameTypeAnalyzer_大小单双()
        {
            Spliter = ',';
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        private string _allowChars = "1245";
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
            if (AnteCodeNumbers.Length != 2)
            {
                errMsg = string.Format("投注号码必须是被\"{0}\"切分成 2个部分的字符串", Spliter);
                return false;
            }
            if (string.IsNullOrEmpty(AnteCodeNumbers[0]) || string.IsNullOrEmpty(AnteCodeNumbers[1]))
            {
                errMsg = "大/小/单/双 投注内容不能为空";
                return false;
            }
            if (!_allowChars.Contains(AnteCodeNumbers[0]) || !_allowChars.Contains(AnteCodeNumbers[1]))
            {
                errMsg = "大/小/单/双 必须由对应的 2/1/5/4 四个数字表示 - " + antecode;
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
            // 判断大小单双
            string[] s_10 = GetCodeStatus(checker.WinNumbers[3]);
            string[] s_01 = GetCodeStatus(checker.WinNumbers[4]);
            if (s_10.Contains(AnteCodeNumbers[0]) && s_01.Contains(AnteCodeNumbers[1]))
            {
                bonusLevelList.Add(0);
            }
            return bonusLevelList;
        }
        // 获取大小单双号码状态。如：3，则返回单小 [1,5]
        private string[] GetCodeStatus(string code)
        {
            var s = new string[2];
            var i = int.Parse(code);
            if (i >= 5 && i <= 9)
            {
                // 大
                s[0] = "2";
            }
            else if (i >= 0 && i <= 4)
            {
                // 小
                s[0] = "1";
            }
            else
            {
                throw new AntecodeFormatException(GameCode, GameType, code, "号码格式错误 - " + code);
            }
            if (i % 2 == 0)
            {
                // 双
                s[1] = "4";
            }
            else
            {
                // 单
                s[1] = "5";
            }
            return s;
        }
    }
}
