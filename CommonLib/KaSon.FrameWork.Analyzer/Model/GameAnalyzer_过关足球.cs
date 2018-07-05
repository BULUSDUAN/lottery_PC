using EntityModel.Interface;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Common.Utilities;
using EntityModel.ExceptionExtend;

namespace  KaSon.FrameWork.Analyzer.Model
{
    internal class GameAnalyzer_过关足球 : IAnteCodeChecker_Sport
    {
        public GameAnalyzer_过关足球(string gameCode)
        {
            GameCode = gameCode;
            CancelMatchResultFlag = "-1";
            Spliter = ',';
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public char Spliter { get; set; }
        // 有效投注号码
        public string EffectiveAnteCode { get; set; }
        public string EffectiveWinNumber { get; set; }

        public string CancelMatchResultFlag { get; set; }

        private string[] _winNumbers;
        private string[] _anteCodes;
        /// <summary>
        /// 检查中奖号码格式是否正确
        /// </summary>
        /// <param name="winNumber">中奖号码</param>
        /// <param name="gameType">玩法</gameType>
        /// <param name="errMsg">错误消息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckWinNumber(ISportResult winNumber, string gameType, out string errMsg)
        {
            if (!string.IsNullOrEmpty(gameType) && gameType != GameType)
            {
                var orderAnalyzer = KaSon.FrameWork.Analyzer.AnalyzerFactory.AnalyzerFactory.GetSportAnteCodeChecker(GameCode, gameType);
                return orderAnalyzer.CheckWinNumber(winNumber, null, out errMsg);
            }
            else
            {
                // 前置验证 - 彩种、玩法、投注号码
                PreconditionAssert.IsNotEmptyString(GameCode, "检查中奖号码格式前，必须设置彩种");
                PreconditionAssert.IsNotNull(winNumber, "必须传入非空的比赛结果");
                PreconditionAssert.IsNotEmptyString(winNumber.GetMatchId(GameCode), "必须传入比赛编号");

                _winNumbers = EffectiveWinNumber.Split(Spliter);
                var result = winNumber.GetMatchResult(GameCode, GameType);
                if (result != CancelMatchResultFlag && !_winNumbers.Contains(result))
                {
                    errMsg = string.Format("错误的比赛结果格式，允许的格式：{0}，取消结果格式：{1}，实际为：'{2}'.", EffectiveWinNumber, CancelMatchResultFlag, result);
                    return false;
                }
                errMsg = "";
                return true;
            }
        }
        /// <summary>
        /// 检查一个单独号码的格式
        /// </summary>
        /// <param name="antecodeNumber">号码</param>
        /// <param name="errMsg">错误信息</param>
        /// <returns>格式是否正确</returns>
        public bool CheckAntecodeNumber(ISportAnteCode antecode, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查中奖号码格式前，必须设置彩种");
            PreconditionAssert.IsNotNull(antecode, "必须传入非空的比赛结果");
            PreconditionAssert.IsNotEmptyString(antecode.MatchId, "必须传入比赛编号");
            PreconditionAssert.IsNotEmptyString(antecode.AnteCode, "必须传入比赛投注号码");

            var tmpList = antecode.AnteCode.Split(Spliter);
            if (tmpList.Length != tmpList.GroupBy(c => c).Count())
            {
                errMsg = "有重复的投注号码 - " + antecode.AnteCode;
                return false;
            }
            _anteCodes = EffectiveAnteCode.Split(Spliter);
            foreach (var item in tmpList)
            {
                if (!_anteCodes.Contains(item))
                {
                    errMsg = "比赛投注号码必须在\"" + EffectiveAnteCode + "\"范围内，实际为 - " + item;
                    return false;
                }
            }
            errMsg = "";
            return true;
        }

      
    }
}
