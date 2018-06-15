using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KaSon.FrameWork.Helper.分析器工厂.接口;
using KaSon.FrameWork.Helper.分析器工厂;

namespace KaSon.FrameWork.Helper.分析器工厂.Model
{
    /// <summary>
    /// 玩法分析 - 过关足球
    /// </summary>
    public class GameTypeAnalyzer_过关足球 : IAntecodeAnalyzable_Sport
    {
        public GameTypeAnalyzer_过关足球()
        {
            CancelMatchResultFlag = "-1";
            IsResultFromAnteCode = false;
        }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 投注号码切分后的数字列表。执行CheckAntecode或AnalyzeAnteCode或CaculateBonus以后有值
        /// </summary>
        public ISportAnteCode[] DanNumbers { get; private set; }
        public ISportAnteCode[] TuoNumbers { get; private set; }

        public bool IsResultFromAnteCode { get; set; }

        public string CancelMatchResultFlag { get; set; }

        // 过关基数 8串1 中的8，只能对常规过关进行设置，如4串1，5串1，6串1等
        public int BaseCount { get; set; }

        public bool CheckAntecode(ISportAnteCode[] antecodeList, out string errMsg)
        {
            // 前置验证 - 彩种、玩法、投注号码
            PreconditionAssert.IsNotEmptyString(GameCode, "检查投注号码格式前，必须设置彩种");
            PreconditionAssert.IsNotEmptyString(GameType, "检查投注号码格式前，必须设置玩法");
            PreconditionAssert.IsTrue((antecodeList != null && antecodeList.Length > 0), "必须传入投注号码");

            //if (antecodeList.Length != antecodeList.GroupBy(a => a.MatchId).Count())
            //{
            //    errMsg = "选择的比赛场次重复";
            //    return false;
            //}
            if (antecodeList.Length != antecodeList.GroupBy(a => new { MatchId = a.MatchId, GameType = a.GameType }).Count())
            {
                errMsg = "选择的比赛场次重复";
                return false;
            }
            if (antecodeList.Length < BaseCount)
            {
                errMsg = "选择的比赛不够，当前串关方式至少需要 " + BaseCount + "场比赛";
                return false;
            }
            TuoNumbers = antecodeList.Where(a => !a.IsDan).ToArray();
            DanNumbers = antecodeList.Where(a => a.IsDan).ToArray();
            if (DanNumbers.Length > BaseCount)
            {
                errMsg = "胆码太多，当前串关方式最多允许 " + BaseCount + "个胆码";
                return false;
            }
            foreach (var code in antecodeList)
            {
                var type = string.IsNullOrEmpty(code.GameType) ? GameType : code.GameType;
                var orderAnalyzer = AnalyzerFactory.GetSportAnteCodeChecker(GameCode, type);
                if (!orderAnalyzer.CheckAntecodeNumber(code, out errMsg)) return false;
            }

            errMsg = "";
            return true;
        }
        public bool CheckWinNumbers(ISportAnteCode[] antecodeList, ISportResult[] winNumberList, out string errMsg)
        {
            PreconditionAssert.IsTrue((winNumberList != null && winNumberList.Length > 0), "必须传入比赛结果");

            //if (winNumberList.Length != winNumberList.GroupBy(a => a.GetMatchId(GameCode)).Count())
            //{
            //    errMsg = "比赛场次重复";
            //    return false;
            //}
            foreach (var code in winNumberList)
            {
                var ante = antecodeList.FirstOrDefault(a => a.MatchId == code.GetMatchId(GameCode));
                if (ante != null)
                {
                    var orderAnalyzer = AnalyzerFactory.GetSportAnteCodeChecker(GameCode, GameType);
                    if (!orderAnalyzer.CheckWinNumber(code, ante.GameType, out errMsg)) return false;
                }
            }

            errMsg = "";
            return true;
        }
        public int AnalyzeAnteCode(ISportAnteCode[] antecodeList)
        {
            string msg;
            if (!CheckAntecode(antecodeList, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, "过关足球", "投注号码格式错误 - " + msg);
            }
            var tmp = antecodeList.Where(a => !a.IsDan).ToArray();
            var danList = antecodeList.Where(a => a.IsDan).ToArray();

            var c = new Combination();
            var ac = new ArrayCombination();
            var totalCount = 0;

            ////new
            ////按比赛编号组成二维数组
            //var totalCodeList = new List<ISportAnteCode[]>();
            //foreach (var g in tmp.GroupBy(p => p.MatchId))
            //{
            //    totalCodeList.Add(tmp.Where(p => p.MatchId == g.Key).ToArray());
            //}

            //if (this.GameType == "HH")
            //{
            //    ac.Calculate(totalCodeList.ToArray(), (match) =>
            //    {
            //        c.Calculate(match, BaseCount - danList.Count(), (tuoArr) =>
            //        {
            //            totalCount++;
            //        });
            //    });
            //}
            //else
            //{

            //    c.Calculate(totalCodeList.ToArray(), BaseCount - danList.Count(), (arr2) =>
            //    {
            //        ac.Calculate(arr2, (tuoArr) =>
            //        {
            //            var count = 1;
            //            foreach (var item in tuoArr)
            //            {
            //                count *= item.AnteCode.Split(',').Length;
            //            }
            //            totalCount += count;
            //        });
            //    });
            //}

            #region old

            c.Calculate<ISportAnteCode>(tmp, BaseCount - danList.Length, (list) =>
            {
                var count = 1;
                foreach (var t in list)
                {
                    count *= t.Length;
                }
                totalCount += count;
            });
            if (totalCount == 0) totalCount = 1;
            foreach (var dan in danList)
            {
                totalCount *= dan.Length;
            }

            #endregion

            return totalCount;
        }
        public SportBonusResult CaculateBonus(ISportAnteCode[] antecodeList, ISportResult[] winNumberList)
        {
            string msg;
            if (!CheckAntecode(antecodeList, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, "过关足球", "投注号码格式错误 - " + msg);
            }
            if (!CheckWinNumbers(antecodeList, winNumberList, out msg))
            {
                throw new AntecodeFormatException(GameCode, GameType, "过关足球", "比赛结果开奖格式错误 - " + msg);
            }
            var result = new SportBonusResult
            {
                IsWin = false,
                BaseCount = BaseCount,
                AnteDanCount = DanNumbers.Length,
                AnteTuoCount = TuoNumbers.Length,
                AnteTotalCount = antecodeList.Length,
                HitDanCount = 0,
                HitTuoCount = 0,
                HitTotalCount = 0,
                BonusCount = 0,
                HitDanMatchIdList = new List<string>(),
                HitTuoMatchIdList = new List<string>(),
                HitTotalMatchIdList = new List<string>(),
                BonusHitMatchIdListCollection = new List<string[]>(),
            };
            var cancelCount = 1;
            var hitList = new List<ISportAnteCode>(antecodeList);
            var cancelList = new Dictionary<string, int>();
            for (int i = hitList.Count - 1; i >= 0; i--)
            {
                var ante = hitList[i];
                var win = winNumberList.FirstOrDefault(a => a.GetMatchId(GameCode).Equals(ante.MatchId));
                if (win == null)
                {
                    throw new Exception("计算派奖的比赛在已开奖的比赛中不存在 - " + ante.MatchId);
                }
                string winResult;
                var gameType = string.IsNullOrEmpty(ante.GameType) ? GameType : ante.GameType;
                if (IsResultFromAnteCode)
                {
                    winResult = ante.GetMatchResult(GameCode, gameType, win.GetFullMatchScore(GameCode));
                    if (string.IsNullOrEmpty(winResult))
                    {
                        winResult = win.GetMatchResult(GameCode, gameType);
                    }
                }
                else
                {
                    winResult = win.GetMatchResult(GameCode, gameType);
                }
                if (winResult != CancelMatchResultFlag && !ante.AnteCode.Split(',').Contains(winResult))
                {
                    hitList.RemoveAt(i);
                }
                else
                {
                    if (winResult == CancelMatchResultFlag && ante.Length > 1)
                    {

                        cancelCount *= ante.Length;
                        cancelList.Add(ante.MatchId, 1);
                    }
                    if (ante.IsDan)
                    {
                        result.HitDanCount++;
                        result.HitDanMatchIdList.Add(ante.MatchId);
                    }
                    else
                    {
                        result.HitTuoCount++;
                        result.HitTuoMatchIdList.Add(ante.MatchId);
                    }
                    result.HitTotalCount++;
                    result.HitTotalMatchIdList.Add(ante.MatchId);
                }
            }
            if (result.HitDanCount < result.AnteDanCount)
            {
                return result;
            }
            if (result.HitTotalCount < result.BaseCount)
            {
                return result;
            }
            result.IsWin = true;
            if (result.BaseCount == result.AnteDanCount)
            {
                result.BonusCount++;
                var tmp = new List<string>();
                tmp.AddRange(result.HitDanMatchIdList);
                result.BonusHitMatchIdListCollection.Add(tmp.OrderBy(m => m).ToArray());
            }
            else
            {
                var c = new Combination();
                c.Calculate(result.HitTuoMatchIdList.ToArray(), result.BaseCount - result.AnteDanCount, (arr) =>
                {
                    var tmp = new List<string>();
                    tmp.AddRange(result.HitDanMatchIdList);
                    tmp.AddRange(arr);
                    var t = GetBonusCount(cancelList, tmp.ToArray());
                    result.BonusCount += t;
                    for (int i = 0; i < t; i++)
                    {
                        result.BonusHitMatchIdListCollection.Add(tmp.OrderBy(m => m).ToArray());
                    }
                });
            }
            return result;
        }
        private int GetBonusCount(Dictionary<string, int> cancelList, string[] anteList)
        {
            var count = 1;
            foreach (var key in cancelList.Keys)
            {
                if (anteList.Contains(key))
                {
                    count *= cancelList[key];
                }
            }
            return count;
        }
    }
}
