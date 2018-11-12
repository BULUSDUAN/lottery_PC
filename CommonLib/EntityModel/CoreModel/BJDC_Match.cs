using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
    public class BJDC_Issuse
    {
        public virtual int Id { get; set; }
        public virtual string IssuseNumber { get; set; }
        public virtual DateTime MinLocalStopTime { get; set; }
        public virtual DateTime MinMatchStartTime { get; set; }
    }

    public class BJDC_Match
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        public virtual int Mid { get; set; }
        /// <summary>
        /// 赛事编号
        /// </summary>
        public virtual int MatchId { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public virtual int MatchOrderId { get; set; }
        /// <summary>
        /// 联赛名字
        /// </summary>
        public virtual string MatchName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public virtual DateTime MatchStartTime { get; set; }
        /// <summary>
        /// 本地结束时间
        /// </summary>
        public virtual DateTime LocalStopTime { get; set; }
        /// <summary>
        /// 赛事状态
        /// </summary>
        public virtual BJDCMatchState MatchState { get; set; }
        /// <summary>
        /// 联赛背景色
        /// </summary>
        public virtual string MatchColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public virtual int HomeTeamId { get; set; }
        /// <summary>
        /// 主队排名 有可能是字符串
        /// </summary>
        public virtual string HomeTeamSort { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public virtual string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public virtual int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public virtual string GuestTeamName { get; set; }
        /// <summary>
        /// 客队排名 有可能是字符串
        /// </summary>
        public virtual string GuestTeamSort { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public virtual int LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public virtual decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public virtual decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal LoseOdds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 限制玩法列表
        /// </summary>
        public virtual string PrivilegesType { get; set; }
    }

    public class BJDC_MatchResult
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public virtual int MatchOrderId { get; set; }
        /// <summary>
        /// 主队半场比分
        /// </summary>
        public virtual string HomeHalf_Result { get; set; }
        /// <summary>
        /// 主队全场比分
        /// </summary>
        public virtual string HomeFull_Result { get; set; }
        /// <summary>
        /// 客队半场比分
        /// </summary>
        public virtual string GuestHalf_Result { get; set; }
        /// <summary>
        /// 客队全场比分
        /// </summary>
        public virtual string GuestFull_Result { get; set; }
        /// <summary>
        /// 胜平负彩果
        /// </summary>
        public virtual string SPF_Result { get; set; }
        /// <summary>
        /// 胜平负开奖sp
        /// </summary>
        public virtual decimal SPF_SP { get; set; }
        /// <summary>
        /// 总进球彩果
        /// </summary>
        public virtual string ZJQ_Result { get; set; }
        /// <summary>
        /// 总进球开奖sp
        /// </summary>
        public virtual decimal ZJQ_SP { get; set; }
        /// <summary>
        /// 上下单双彩果
        /// </summary>
        public virtual string SXDS_Result { get; set; }
        /// <summary>
        /// 上下单双开奖sp
        /// </summary>
        public virtual decimal SXDS_SP { get; set; }
        /// <summary>
        /// 比分彩果
        /// </summary>
        public virtual string BF_Result { get; set; }
        /// <summary>
        /// 比分开奖sp
        /// </summary>
        public virtual decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场彩果
        /// </summary>
        public virtual string BQC_Result { get; set; }
        /// <summary>
        /// 半全场开奖sp
        /// </summary>
        public virtual decimal BQC_SP { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual string MatchState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return this.MatchOrderId.ToString();
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return HomeFull_Result + ":" + GuestFull_Result;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
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
            }
            return string.Empty;
        }
    }

    public class BJDC_MatchResult_Prize
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public virtual string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public virtual string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public virtual int MatchOrderId { get; set; }
        /// <summary>
        /// 主队半场比分
        /// </summary>
        public virtual string HomeHalf_Result { get; set; }
        /// <summary>
        /// 主队全场比分
        /// </summary>
        public virtual string HomeFull_Result { get; set; }
        /// <summary>
        /// 客队半场比分
        /// </summary>
        public virtual string GuestHalf_Result { get; set; }
        /// <summary>
        /// 客队全场比分
        /// </summary>
        public virtual string GuestFull_Result { get; set; }
        /// <summary>
        /// 胜平负彩果
        /// </summary>
        public virtual string SPF_Result { get; set; }
        /// <summary>
        /// 胜平负开奖sp
        /// </summary>
        public virtual decimal SPF_SP { get; set; }
        /// <summary>
        /// 总进球彩果
        /// </summary>
        public virtual string ZJQ_Result { get; set; }
        /// <summary>
        /// 总进球开奖sp
        /// </summary>
        public virtual decimal ZJQ_SP { get; set; }
        /// <summary>
        /// 上下单双彩果
        /// </summary>
        public virtual string SXDS_Result { get; set; }
        /// <summary>
        /// 上下单双开奖sp
        /// </summary>
        public virtual decimal SXDS_SP { get; set; }
        /// <summary>
        /// 比分彩果
        /// </summary>
        public virtual string BF_Result { get; set; }
        /// <summary>
        /// 比分开奖sp
        /// </summary>
        public virtual decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场彩果
        /// </summary>
        public virtual string BQC_Result { get; set; }
        /// <summary>
        /// 半全场开奖sp
        /// </summary>
        public virtual decimal BQC_SP { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public virtual string MatchState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return this.MatchOrderId.ToString();
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return HomeFull_Result + ":" + GuestFull_Result;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
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
            }
            return string.Empty;
        }
    }
}
