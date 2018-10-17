using EntityModel.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace EntityModel.CoreModel
{
    public class MatchInfo : ISportResult
    {
        // 彩种
        public string GameCode { get; set; }
        // 期号
        public string IssuseNumber { get; set; }
        // 期号
        public int MatchIndex { get; set; }
        // 比赛唯一编号
        public string MatchId { get; set; }
        // 比赛编号名称。周三002
        public string MatchIdName { get; set; }
        // 比赛序号
        public string MatchNumber { get; set; }
        // 比赛日期
        public string MatchDate { get; set; }
        // 投注截至时间
        public DateTime BettingStopTime { get; set; }
        // 创建时间
        public DateTime CreateTime { get; set; }
        // 更新时间
        public DateTime UpdateTime { get; set; }
        // 开奖时间
        public DateTime? OpenTime { get; set; }

        #region 联赛
        // 联赛编号
        public string LeagueId { get; set; }
        // 联赛名称
        public string LeagueName { get; set; }
        #endregion

        #region 球队
        // 主队编号
        public string HomeTeamId { get; set; }
        // 主队名称
        public string HomeTeamName { get; set; }
        // 主队排名情况
        public string HomeTeamRankName { get; set; }
        // 客队编号
        public string GuestTeamId { get; set; }
        // 客队名称
        public string GuestTeamName { get; set; }
        // 客队排名情况
        public string GuestTeamRankName { get; set; }
        // 让球数
        public int LetBall { get; set; }
        #endregion

        #region 比赛
        // 比赛开始时间
        public DateTime MatchStartTime { get; set; }
        // 比赛状态
        public string MatchState { get; set; }
        // 主队半场得分
        public string HomeTeamHalfScore { get; set; }
        // 主队得分
        public int HomeTeamScore { get; set; }
        // 客队半场得分
        public string GuestTeamHalfScore { get; set; }
        // 客队得分
        public int GuestTeamScore { get; set; }
        #endregion

        #region 结果

        // 胜平负结果:3,1,0
        public string SPF_Result { get; set; }
        // 不让球胜平负结果:3,1,0
        public string BRQSPF_Result { get; set; }
        // 进球数结果
        public string ZJQ_Result { get; set; }
        // 单双比分结果
        public string SXDS_Result { get; set; }
        // 比分结果
        public string BF_Result { get; set; }
        // 半全场结果
        public string BQC_Result { get; set; }

        // 胜负结果:3,1,0
        public string SF_Result { get; set; }
        // 让分胜负结果
        public string RFSF_Result { get; set; }
        // 比分结果
        public string SFC_Result { get; set; }
        // 半全场结果
        public string DXF_Result { get; set; }

        #endregion

        #region 北京单场SP

        /// <summary>
        /// 胜平负开奖sp
        /// </summary>
        public decimal SPF_SP { get; set; }
        /// <summary>
        /// 总进球开奖sp
        /// </summary>
        public decimal ZJQ_SP { get; set; }
        /// <summary>
        /// 上下单双开奖sp
        /// </summary>
        public decimal SXDS_SP { get; set; }
        /// <summary>
        /// 比分开奖sp
        /// </summary>
        public decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场开奖sp
        /// </summary>
        public decimal BQC_SP { get; set; }

        #endregion

        public string GetMatchId(string gameCode)
        {
            switch (gameCode)
            {
                case "BJDC":
                    return MatchIndex.ToString();
                case "JCZQ":
                case "JCLQ":
                    return MatchId;
                default:
                    throw new ArgumentException("获取比赛编号，不支持的彩种 - " + gameCode);
            }
        }
        public string GetFullMatchScore(string gameCode)
        {
            return HomeTeamScore + ":" + GuestTeamScore;
        }
        public string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            gameCode = gameCode.ToUpper();
            gameType = gameType.ToUpper();
            switch (gameCode)
            {
                case "BJDC":
                    switch (gameType)
                    {
                        case "SF":
                            return SF_Result;
                        case "SPF":
                            return SPF_Result;
                        case "ZJQ":
                            return ZJQ_Result;
                        case "SXDS":
                            return SXDS_Result;
                        case "BF":
                            return BF_Result;
                        case "BQC":
                            return BQC_Result;
                        default:
                            throw new ArgumentException("获取北单比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                    }
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":
                            return SPF_Result;
                        case "BRQSPF":
                            return BRQSPF_Result;
                        case "ZJQ":
                            return ZJQ_Result;
                        case "BF":
                            return BF_Result;
                        case "BQC":
                            return BQC_Result;
                        default:
                            throw new ArgumentException("获取竞彩足球比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                    }
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":
                            return SF_Result;
                        case "RFSF":
                            if (offset != -1 && RFSF_Result != "-1")
                            {
                                var host1 = HomeTeamScore;
                                var guest1 = GuestTeamScore;
                                if (host1 + offset > guest1)
                                {
                                    return "3";
                                }
                                else if (host1 + offset < guest1)
                                {
                                    return "0";
                                }
                            }
                            return RFSF_Result;
                        case "SFC":
                            return SFC_Result;
                        case "DXF":
                            if (offset != -1 && DXF_Result != "-1")
                            {
                                var host2 = HomeTeamScore;
                                var guest2 = GuestTeamScore;
                                if (host2 + guest2 > offset)
                                {
                                    return "3";
                                }
                                else if (host2 + guest2 < offset)
                                {
                                    return "0";
                                }
                            }
                            return DXF_Result;
                        default:
                            throw new ArgumentException("获取竞彩篮球比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                    }
                default:
                    throw new ArgumentException("获取比赛结果，不支持的彩种 - " + gameCode);
            }
        }
    }

    public class MatchInfoCollection : List<MatchInfo>
    {
    }

    public class MatchResult
    {
        // 彩种
        public string GameCode { get; set; }
        // 期号
        public string IssuseNumber { get; set; }
        // 期号
        public int MatchIndex { get; set; }
        // 比赛唯一编号
        public string MatchId { get; set; }
        // 比赛编号名称。周三002
        public string MatchIdName { get; set; }
        // 比赛序号
        public string MatchNumber { get; set; }
        // 比赛日期
        public string MatchDate { get; set; }

        #region 联赛
        // 联赛编号
        public string LeagueId { get; set; }
        // 联赛名称
        public string LeagueName { get; set; }
        #endregion

        #region 球队
        // 主队编号
        public string HomeTeamId { get; set; }
        // 主队名称
        public string HomeTeamName { get; set; }
        // 主队排名情况
        public string HomeTeamRankName { get; set; }
        // 客队编号
        public string GuestTeamId { get; set; }
        // 客队名称
        public string GuestTeamName { get; set; }
        // 客队排名情况
        public string GuestTeamRankName { get; set; }
        // 让球数
        public int LetBall { get; set; }
        #endregion

        #region 比赛
        // 比赛开始时间
        public DateTime MatchStartTime { get; set; }
        // 比赛状态
        public string MatchState { get; set; }
        // 主队半场得分
        public string HomeTeamHalfScore { get; set; }
        // 主队得分
        public string HomeTeamScore { get; set; }
        // 客队半场得分
        public string GuestTeamHalfScore { get; set; }
        // 客队得分
        public string GuestTeamScore { get; set; }
        #endregion

        #region 结果
        // 胜平负结果:3,1,0
        public string SPF_Result { get; set; }
        // 不让球胜平负结果:3,1,0
        public string BRQSPF_Result { get; set; }
        // 进球数结果
        public string ZJQ_Result { get; set; }
        // 单双比分结果
        public string SXDS_Result { get; set; }
        // 比分结果
        public string BF_Result { get; set; }
        // 半全场结果
        public string BQC_Result { get; set; }

        // 胜负结果:3,1,0
        public string SF_Result { get; set; }
        // 让分胜负结果
        public string RFSF_Result { get; set; }
        // 比分结果
        public string SFC_Result { get; set; }
        // 半全场结果
        public string DXF_Result { get; set; }
        #endregion
    }

    //public class JCZQ_AnteCode
    //{
    //    public string GameType { get; set; }
    //    public string MatchId { get; set; }
    //    public string AnteCode { get; set; }
    //    public string Odds { get; set; }
    //    public int Length { get; set; }
    //}


    public class MatchInfo_SFGG : ISportResult
    {
        // 彩种
        public string GameCode { get; set; }
        // 期号
        public string IssuseNumber { get; set; }
        // 期号
        public int MatchIndex { get; set; }
        // 比赛唯一编号
        public string MatchId { get; set; }
        // 比赛编号名称。周三002
        public string MatchIdName { get; set; }
        // 比赛序号
        public string MatchNumber { get; set; }
        // 比赛日期
        public string MatchDate { get; set; }
        // 投注截至时间
        public DateTime BettingStopTime { get; set; }
        // 创建时间
        public DateTime CreateTime { get; set; }
        // 更新时间
        public DateTime UpdateTime { get; set; }
        // 开奖时间
        public DateTime? OpenTime { get; set; }

        #region 联赛
        // 联赛编号
        public string LeagueId { get; set; }
        // 联赛名称
        public string LeagueName { get; set; }
        #endregion

        #region 球队
        // 主队编号
        public string HomeTeamId { get; set; }
        // 主队名称
        public string HomeTeamName { get; set; }
        // 主队排名情况
        public string HomeTeamRankName { get; set; }
        // 客队编号
        public string GuestTeamId { get; set; }
        // 客队名称
        public string GuestTeamName { get; set; }
        // 客队排名情况
        public string GuestTeamRankName { get; set; }
        // 让球数
        public decimal LetBall { get; set; }
        #endregion

        #region 比赛
        // 比赛开始时间
        public DateTime MatchStartTime { get; set; }
        // 比赛状态
        public string MatchState { get; set; }
        // 主队半场得分
        public string HomeTeamHalfScore { get; set; }
        // 主队得分
        public string HomeTeamScore { get; set; }
        // 客队半场得分
        public string GuestTeamHalfScore { get; set; }
        // 客队得分
        public string GuestTeamScore { get; set; }
        #endregion

        #region 结果
        // 胜平负结果:3,1,0
        public string SPF_Result { get; set; }
        // 不让球胜平负结果:3,1,0
        public string BRQSPF_Result { get; set; }
        // 进球数结果
        public string ZJQ_Result { get; set; }
        // 单双比分结果
        public string SXDS_Result { get; set; }
        // 比分结果
        public string BF_Result { get; set; }
        // 半全场结果
        public string BQC_Result { get; set; }

        // 胜负结果:3,1,0
        public string SF_Result { get; set; }
        // 让分胜负结果
        public string RFSF_Result { get; set; }
        // 比分结果
        public string SFC_Result { get; set; }
        // 半全场结果
        public string DXF_Result { get; set; }
        #endregion

        public string GetMatchId(string gameCode)
        {
            switch (gameCode)
            {
                case "BJDC":
                    return MatchIndex.ToString();
                case "JCZQ":
                case "JCLQ":
                    return MatchId;
                default:
                    throw new ArgumentException("获取比赛编号，不支持的彩种 - " + gameCode);
            }
        }
        public string GetFullMatchScore(string gameCode)
        {
            return HomeTeamScore + ":" + GuestTeamScore;
        }
        public string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            gameCode = gameCode.ToUpper();
            gameType = gameType.ToUpper();
            switch (gameCode)
            {
                case "BJDC":
                    switch (gameType)
                    {
                        case "SF":
                            return SF_Result;
                        case "SPF":
                            return SPF_Result;
                        case "ZJQ":
                            return ZJQ_Result;
                        case "SXDS":
                            return SXDS_Result;
                        case "BF":
                            return BF_Result;
                        case "BQC":
                            return BQC_Result;
                        default:
                            throw new ArgumentException("获取北单比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                    }
                case "JCZQ":
                    switch (gameType)
                    {
                        case "SPF":
                            return SPF_Result;
                        case "BRQSPF":
                            return BRQSPF_Result;
                        case "ZJQ":
                            return ZJQ_Result;
                        case "BF":
                            return BF_Result;
                        case "BQC":
                            return BQC_Result;
                        default:
                            throw new ArgumentException("获取竞彩足球比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                    }
                case "JCLQ":
                    switch (gameType)
                    {
                        case "SF":
                            return SF_Result;
                        case "RFSF":
                            if (offset != -1)
                            {
                                var host1 = decimal.Parse(HomeTeamScore);
                                var guest1 = decimal.Parse(GuestTeamScore);
                                if (host1 + offset > guest1)
                                {
                                    return "3";
                                }
                                else if (host1 + offset < guest1)
                                {
                                    return "0";
                                }
                            }
                            return RFSF_Result;
                        case "SFC":
                            return SFC_Result;
                        case "DXF":
                            if (offset != -1)
                            {
                                var host2 = decimal.Parse(HomeTeamScore);
                                var guest2 = decimal.Parse(GuestTeamScore);
                                if (host2 + guest2 > offset)
                                {
                                    return "3";
                                }
                                else if (host2 + guest2 < offset)
                                {
                                    return "0";
                                }
                            }
                            return DXF_Result;
                        default:
                            throw new ArgumentException("获取竞彩篮球比赛结果，不支持的玩法 - " + gameCode + "-" + gameType);
                    }
                default:
                    throw new ArgumentException("获取比赛结果，不支持的彩种 - " + gameCode);
            }
        }
    }

    public class MatchInfo_SFGGCollection : List<MatchInfo_SFGG>
    {
    }
}
