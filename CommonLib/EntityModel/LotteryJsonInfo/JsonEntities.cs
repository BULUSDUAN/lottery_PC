
using EntityModel.Enum;
using EntityModel.Interface;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace EntityModel.LotteryJsonInfo
{
    public class JCLQ_DXF_SPInfo : JCLQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 大分
        /// </summary>
        public decimal DF { get; set; }
        /// <summary>
        /// 小分
        /// </summary>
        public decimal XF { get; set; }
        /// <summary>
        /// 预设总分
        /// </summary>
        public decimal YSZF { get; set; }

        public string RFSF { get; set; }
        public string SFC { get; set; }
        public string DXF { get; set; }
        public string SF { get; set; }
        public string PrivilegesType { get; set; }
    }
    public class JCLQ_SFC_SPInfo : JCLQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 客胜1-5
        /// </summary>
        public decimal GuestWin1_5 { get; set; }
        public decimal GuestWin11_15 { get; set; }
        public decimal GuestWin16_20 { get; set; }
        public decimal GuestWin21_25 { get; set; }
        public decimal GuestWin26 { get; set; }
        public decimal GuestWin6_10 { get; set; }
        /// <summary>
        /// 主生1-5
        /// </summary>
        public decimal HomeWin1_5 { get; set; }
        public decimal HomeWin11_15 { get; set; }
        public decimal HomeWin16_20 { get; set; }
        public decimal HomeWin21_25 { get; set; }
        public decimal HomeWin26 { get; set; }
        public decimal HomeWin6_10 { get; set; }

        public string RFSF { get; set; }
        public string SFC { get; set; }
        public string DXF { get; set; }
        public string SF { get; set; }
        public string PrivilegesType { get; set; }
    }
    public class JCLQ_RFSF_SPInfo : JCLQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 负sp
        /// </summary>
        public decimal LoseSP { get; set; }
        /// <summary>
        /// 让分数
        /// </summary>
        public decimal RF { get; set; }
        /// <summary>
        /// 胜sp
        /// </summary>
        public decimal WinSP { get; set; }

        public string RFSF { get; set; }
        public string SFC { get; set; }
        public string DXF { get; set; }
        public string SF { get; set; }
        public string PrivilegesType { get; set; }
    }
    public class JCLQ_SF_SPInfo : JCLQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 负sp
        /// </summary>
        public decimal LoseSP { get; set; }
        /// <summary>
        /// 胜sp
        /// </summary>
        public decimal WinSP { get; set; }

        public string RFSF { get; set; }
        public string SFC { get; set; }
        public string DXF { get; set; }
        public string SF { get; set; }
        public string PrivilegesType { get; set; }
    }
    public class JCLQ_HH_SPInfo : JCLQBase
    {
        public string RFSF { get; set; }
        public string SFC { get; set; }
        public string DXF { get; set; }
        public string SF { get; set; }
        public string PrivilegesType { get; set; }
    }
    public class JCLQ_MatchResultInfo : JCLQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 大小分比赛结果
        /// </summary>
        public string DXF_Result { get; set; }
        /// <summary>
        /// 大小分sp
        /// </summary>
        public decimal DXF_SP { get; set; }
        public string DXF_Trend { get; set; }
        /// <summary>
        /// 客队得分
        /// </summary>
        public int GuestScore { get; set; }
        /// <summary>
        /// 主队得分
        /// </summary>
        public int HomeScore { get; set; }
        /// <summary>
        /// 比赛状态
        /// </summary>
        public string MatchState { get; set; }
        /// <summary>
        /// 让分胜负比赛结果
        /// </summary>
        public string RFSF_Result { get; set; }
        public decimal RFSF_SP { get; set; }
        public string RFSF_Trend { get; set; }
        /// <summary>
        /// 胜负比赛结果
        /// </summary>
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        /// <summary>
        /// 胜分差比赛结果
        /// </summary>
        public string SFC_Result { get; set; }
        public decimal SFC_SP { get; set; }
        public string PrivilegesType { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class JCLQ_SP_SFC : JCLQBase
    {
        public ObjectId _id { get; set; }
        public string SFC { get; set; }
    }
    public class JCLQ_SP : JCLQBase
    {
        public ObjectId _id { get; set; }
        public string RFSF { get; set; }
        public string SF { get; set; }
        public string SFC { get; set; }
        public string DXF { get; set; }
    }
    [BsonIgnoreExtraElements]
    public class JCLQ_MatchInfo : JCLQBase
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 平均负sp
        /// </summary>
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 平均胜sp
        /// </summary>
        public decimal AverageWin { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 单式投注结束时间
        /// </summary>
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式投注结束时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        public int FXId { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 联赛编号
        /// </summary>
        public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        public int MatchState { get; set; }
        public int Mid { get; set; }
        /// <summary>
        /// 开赛时间
        /// </summary>
        public string StartDateTime { get; set; }
        public string PrivilegesType { get; set; }
        public string State { get; set; }
    }
    public class JCLQBase: IBallBaseInfo
    {
        /// <summary>
        /// 比赛日期
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛期号
        /// </summary>
        public string MatchNumber { get; set; }
        //public string PrivilegesType { get; set; }
        //public JCLQ_SF_SPInfo SF { get; set; }
        //public JCLQ_RFSF_SPInfo RFSF { get; set; }
        //public JCLQ_DXF_SPInfo DXF { get; set; }
        //public JCLQ_SFC_SPInfo SFC { get; set; }

    }
    public class JCZQ_BQC_SPInfo : JCZQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 负负sp
        /// </summary>
        public decimal F_F_Odds { get; set; }
        /// <summary>
        /// 负平
        /// </summary>
        public decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负胜
        /// </summary>
        public decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 平负
        /// </summary>
        public decimal P_F_Odds { get; set; }
        /// <summary>
        /// 平平
        /// </summary>
        public decimal P_P_Odds { get; set; }
        /// <summary>
        /// 平胜
        /// </summary>
        public decimal P_SH_Odds { get; set; }
        /// <summary>
        /// 胜负
        /// </summary>
        public decimal SH_F_Odds { get; set; }
        /// <summary>
        /// 胜平
        /// </summary>
        public decimal SH_P_Odds { get; set; }
        /// <summary>
        /// 胜胜
        /// </summary>
        public decimal SH_SH_Odds { get; set; }
        public string PrivilegesType { get; set; }
        public string NoSaleState { get; set; }
    }
    public class JCZQ_BF_SPInfo : JCZQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 负 0比1
        /// </summary>
        public decimal F_01 { get; set; }
        /// <summary>
        /// 负 0比2
        /// </summary>
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_05 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_15 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        public decimal F_25 { get; set; }
        /// <summary>
        /// 负其它
        /// </summary>
        public decimal F_QT { get; set; }
        /// <summary>
        /// 平 0比0
        /// </summary>
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        /// <summary>
        /// 平 其它
        /// </summary>
        public decimal P_QT { get; set; }
        /// <summary>
        /// 胜1比0
        /// </summary>
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        public decimal S_50 { get; set; }
        public decimal S_51 { get; set; }
        public decimal S_52 { get; set; }
        /// <summary>
        /// 胜其它
        /// </summary>
        public decimal S_QT { get; set; }
        public string PrivilegesType { get; set; }
        public string NoSaleState { get; set; }
    }
    public class JCZQ_ZJQ_SPInfo : JCZQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 进球0sp
        /// </summary>
        public decimal JinQiu_0_Odds { get; set; }
        /// <summary>
        /// 进球1sp
        /// </summary>
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        public decimal JinQiu_7_Odds { get; set; }
        public string PrivilegesType { get; set; }
        public string NoSaleState { get; set; }
    }
    public class JCZQ_SPF_SPInfo : JCZQBase
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 平sp
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负sp
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 胜sp
        /// </summary>
        public decimal WinOdds { get; set; }
        public string PrivilegesType { get; set; }
        public string NoSaleState { get; set; }
    }
    public class JCZQ_MatchResultInfo : JCZQBase
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 比分结果
        /// </summary>
        public string BF_Result { get; set; }
        /// <summary>
        /// 比分sp
        /// </summary>
        public decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场
        /// </summary>
        public string BQC_Result { get; set; }
        public decimal BQC_SP { get; set; }
        /// <summary>
        /// 不让球胜平负
        /// </summary>
        public string BRQSPF_Result { get; set; }
        public decimal BRQSPF_SP { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 客队全场得分
        /// </summary>
        public int FullGuestTeamScore { get; set; }
        /// <summary>
        /// 主队全场得分
        /// </summary>
        public int FullHomeTeamScore { get; set; }
        /// <summary>
        /// 客队半场得分
        /// </summary>
        public int HalfGuestTeamScore { get; set; }
        /// <summary>
        /// 主队半场得分
        /// </summary>
        public int HalfHomeTeamScore { get; set; }
        public string MatchState { get; set; }
        /// <summary>
        /// 胜平负结果
        /// </summary>
        public string SPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        /// <summary>
        /// 总进球结果
        /// </summary>
        public string ZJQ_Result { get; set; }
        public decimal ZJQ_SP { get; set; }
        public string PrivilegesType { get; set; }
    }

    public class JCZQ_MatchInfo : JCZQBase
    {

        public ObjectId _id { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 单式投注结束时间
        /// </summary>
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 平 平均sp
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 复式投注结束时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        public int FXId { get; set; }
        public string Gi { get; set; }
        /// <summary>
        /// 客队id
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        public string Hi { get; set; }
        /// <summary>
        /// 主队id
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 联赛id
        /// </summary>
        public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        /// <summary>
        /// 负 平均sp
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛id名称
        /// </summary>
        public string MatchIdName { get; set; }
        public int Mid { get; set; }
        public string PrivilegesType { get; set; }
        /// <summary>
        /// 客队短名称
        /// </summary>
        public string ShortGuestTeamName { get; set; }
        /// <summary>
        /// 主队短名称
        /// </summary>
        public string ShortHomeTeamName { get; set; }
        /// <summary>
        /// 联赛短名称
        /// </summary>
        public string ShortLeagueName { get; set; }
        /// <summary>
        /// 开塞时间
        /// </summary>
        public string StartDateTime { get; set; }
        /// <summary>
        /// 胜 平均sp
        /// </summary>
        public decimal WinOdds { get; set; }
        public string MatchStopDesc { get; set; }
        public string HomeImgPath { get; set; }
        public string GuestImgPath { get; set; }
        public string HRank { get; set; }
        public string GRank { get; set; }
        public string HLg { get; set; }
        public string GLg { get; set; }
        public long mId { get; set; }
    }

    public class JCZQ_SJBMatchInfo
    {
        public string MatchId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 球队
        /// </summary>
        public string Team { get; set; }
        /// <summary>
        /// 投注状态
        /// </summary>
        public string BetState { get; set; }
        /// <summary>
        /// 世界杯类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 奖金金额
        /// </summary>
        public decimal BonusMoney { get; set; }
        /// <summary>
        /// 支持率
        /// </summary>
        public decimal SupportRate { get; set; }
        /// <summary>
        /// 概率
        /// </summary>
        public decimal Probadbility { get; set; }
    }

    public class JCZQBase: IBallBaseInfo
    {
        /// <summary>
        /// 比赛日期
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛期号
        /// </summary>
        public string MatchNumber { get; set; }
        public string State { get; set; }
        public string BF { get; set; }
        public string BQC { get; set; }
        public string SPF { get; set; }
        public string BRQSPF { get; set; }
        public string ZJQ { get; set; }
        //public string PrivilegesType { get; set; }

    }
    public class BJDC_BQC_SpInfo:IBJDCBallBaseInfo
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 负负sp
        /// </summary>
        public decimal F_F_Odds { get; set; }
        /// <summary>
        /// 负平sp
        /// </summary>
        public decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负胜sp
        /// </summary>
        public decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        public decimal P_F_Odds { get; set; }
        public decimal P_P_Odds { get; set; }
        public decimal P_SH_Odds { get; set; }
        public decimal SH_F_Odds { get; set; }
        public decimal SH_P_Odds { get; set; }
        public decimal SH_SH_Odds { get; set; }
    }
    public class BJDC_BF_SpInfo:IBJDCBallBaseInfo
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 负 0:1
        /// </summary>
        public decimal F_01 { get; set; }
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        /// <summary>
        /// 负其它
        /// </summary>
        public decimal F_QT { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 平 0：0
        /// </summary>
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        /// <summary>
        /// 平其它
        /// </summary>
        public decimal P_QT { get; set; }
        /// <summary>
        /// 胜1:0
        /// </summary>
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        /// <summary>
        /// 胜其它
        /// </summary>
        public decimal S_QT { get; set; }

    }
    public class BJDC_SXDS_SpInfo:IBJDCBallBaseInfo
    {
        public string CreateTime { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 上单
        /// </summary>
        public decimal SH_D_Odds { get; set; }
        /// <summary>
        /// 上双
        /// </summary>
        public decimal SH_S_Odds { get; set; }
        /// <summary>
        /// 下单
        /// </summary>
        public decimal X_D_Odds { get; set; }
        /// <summary>
        /// 下双
        /// </summary>
        public decimal X_S_Odds { get; set; }

    }
    public class BJDC_ZJQ_SpInfo:IBJDCBallBaseInfo
    {
        public string CreateTime { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 进球1 sp
        /// </summary>
        public decimal JinQiu_0_Odds { get; set; }
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        /// <summary>
        /// 进球7或以上sp
        /// </summary>
        public decimal JinQiu_7_Odds { get; set; }

    }
    public class BJDC_SPF_SpInfo: IBJDCBallBaseInfo
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 平sp
        /// </summary>
        public decimal Flat_Odds { get; set; }
        public string GameType { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 负sp
        /// </summary>
        public decimal Lose_Odds { get; set; }
        /// <summary>
        /// 胜sp
        /// </summary>
        public decimal Win_Odds { get; set; }

    }
    public class BJDC_MatchResultInfo
    {
        /// <summary>
        /// 比分结果
        /// </summary>
        public string BF_Result { get; set; }
        /// <summary>
        /// 比分sp
        /// </summary>
        public decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场结果
        /// </summary>
        public string BQC_Result { get; set; }
        public decimal BQC_SP { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 客队全场得分
        /// </summary>
        public string GuestFull_Result { get; set; }
        /// <summary>
        /// 客队半场得分
        /// </summary>
        public string GuestHalf_Result { get; set; }
        /// <summary>
        /// 主队全场得分
        /// </summary>
        public string HomeFull_Result { get; set; }
        /// <summary>
        /// 主队半场得分
        /// </summary>
        public string HomeHalf_Result { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        public string MatchState { get; set; }
        /// <summary>
        /// 胜平负结果
        /// </summary>
        public string SPF_Result { get; set; }
        public decimal SPF_SP { get; set; }
        /// <summary>
        /// 上下单双结果
        /// </summary>
        public string SXDS_Result { get; set; }
        public decimal SXDS_SP { get; set; }
        /// <summary>
        /// 总进球结果
        /// </summary>
        public string ZJQ_Result { get; set; }
        public decimal ZJQ_SP { get; set; }

    }
    public class BJDC_MatchInfo
    {
        public string CreateTime { get; set; }
        /// <summary>
        /// 平 平均sp
        /// </summary>
        public decimal FlatOdds { get; set; }
        public int FXId { get; set; }
        public string Gi { get; set; }
        /// <summary>
        /// 客队id
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 客队排名
        /// </summary>
        public string GuestTeamSort { get; set; }
        public string Hi { get; set; }
        /// <summary>
        /// 主队id
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 主队排名
        /// </summary>
        public string HomeTeamSort { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        public string LocalStopTime { get; set; }
        /// <summary>
        /// 负 平均sp
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛颜色
        /// </summary>
        public string MatchColor { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 比赛排序id
        /// </summary>
        public int MatchOrderId { get; set; }
        public string MatchStartTime { get; set; }
        public BJDCMatchState MatchState { get; set; }
        public int Mid { get; set; }
        /// <summary>
        /// 胜 平均sp
        /// </summary>
        public decimal WinOdds { get; set; }

    }
    public class CTZQ_OddInfo
    {
        public string AverageOdds { get; set; }
        public string FullAverageOdds { get; set; }
        public string HalfAverageOdds { get; set; }
        public string Id { get; set; }
        public decimal KLFlat { get; set; }
        public decimal KLLose { get; set; }
        public decimal KLWin { get; set; }
        public decimal LSFlat { get; set; }
        public decimal LSLose { get; set; }
        public decimal LSWin { get; set; }
        public string UpdateTime { get; set; }
        public string YPSW { get; set; }
    }

    public class CTZQ_MatchInfo
    {
        /// <summary>
        /// 颜色
        /// </summary>
        public string Color { get; set; }
        public int FXId { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 客队半场得分
        /// </summary>
        public int GuestTeamHalfScore { get; set; }
        /// <summary>
        /// 客队id
        /// </summary>
        public string GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 客队排名
        /// </summary>
        public int GuestTeamScore { get; set; }
        public string GuestTeamStanding { get; set; }
        /// <summary>
        /// 主队半场得分
        /// </summary>
        public int HomeTeamHalfScore { get; set; }
        /// <summary>
        /// 主队id
        /// </summary>
        public string HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 主队排名
        /// </summary>
        public int HomeTeamScore { get; set; }
        public string HomeTeamStanding { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// 比赛名称
        /// </summary>
        public string MatchName { get; set; }
        public string MatchResult { get; set; }
        public string MatchStartTime { get; set; }
        public CTZQMatchState MatchState { get; set; }
        public int Mid { get; set; }
        public int OrderNumber { get; set; }
        public string UpdateTime { get; set; }

        //public string GuestTeamStanding { get; set; }
    }
    /// <summary>
    /// 北单队伍信息
    /// </summary>
    [Serializable]
    public class BJDC_MatchInfo_WEB
    {
        //队伍基础信息
        //public string CreateTime { get; set; }
        //public decimal FlatOdds { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        //public string GuestTeamSort { get; set; }
        //public string GuestTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        //public string HomeTeamSort { get; set; }
        //public string HomeTeamId { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        public DateTime LocalStopTime { get; set; }
        //public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛颜色
        /// </summary>
        public string MatchColor { get; set; }
        //public int MatchId { get; set; }
        /// <summary>
        /// 比赛名称
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public int MatchOrderId { get; set; }
        public DateTime MatchStartTime { get; set; }
        public int MatchState { get; set; }
        //public decimal WinOdds { get; set; }
        //public int Mid { get; set; }
        public int FXId { get; set; }

        //队伍结果信息
        //public string BF_Result { get; set; }
        //public decimal BF_SP { get; set; }
        //public string BQC_Result { get; set; }
        //public decimal BQC_SP { get; set; }
        //public string GuestFull_Result { get; set; }
        //public string GuestHalf_Result { get; set; }
        //public string HomeFull_Result { get; set; }
        //public string HomeHalf_Result { get; set; }
        //public string MatchStateName { get; set; }
        //public string SPF_Result { get; set; }
        //public decimal SPF_SP { get; set; }
        //public string SXDS_Result { get; set; }
        //public decimal SXDS_SP { get; set; }
        //public string ZJQ_Result { get; set; }
        //public decimal ZJQ_SP { get; set; }
        //public string LotteryTime { get; set; }

        //胜平负-SP数据
        public decimal SP_Flat_Odds { get; set; }
        public decimal SP_Lose_Odds { get; set; }
        public decimal SP_Win_Odds { get; set; }

        //总进球-SP数据
        public decimal JinQiu_0_Odds { get; set; }
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        public decimal JinQiu_7_Odds { get; set; }

        //上下单双-SP数据
        public decimal SH_D_Odds { get; set; }
        public decimal SH_S_Odds { get; set; }
        public decimal X_D_Odds { get; set; }
        public decimal X_S_Odds { get; set; }

        //比分-SP数据
        public decimal F_01 { get; set; }
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        public decimal F_QT { get; set; }
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        public decimal P_QT { get; set; }
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        public decimal S_QT { get; set; }

        //半全场-SP数据
        public decimal F_F_Odds { get; set; }
        public decimal F_P_Odds { get; set; }
        public decimal F_SH_Odds { get; set; }
        public decimal P_F_Odds { get; set; }
        public decimal P_P_Odds { get; set; }
        public decimal P_SH_Odds { get; set; }
        public decimal SH_F_Odds { get; set; }
        public decimal SH_P_Odds { get; set; }
        public decimal SH_SH_Odds { get; set; }
    }

    /// <summary>
    /// 传统足球队伍信息
    /// </summary>
    /// 
    [Serializable]
    public class CTZQ_MatchInfo_WEB
    {
        //队伍基础信息
        public string Color { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get; set; }
        //public int GuestTeamHalfScore { get; set; }
        //public string GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        //public int GuestTeamScore { get; set; }
        public string GuestTeamStanding { get; set; }
        //public int HomeTeamHalfScore { get; set; }
        //public string HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        //public int HomeTeamScore { get; set; }
        public string HomeTeamStanding { get; set; }
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        //public int MatchId { get; set; }
        public string MatchName { get; set; }
        //public string MatchResult { get; set; }
        public DateTime MatchStartTime { get; set; }
        public CTZQMatchState MatchState { get; set; }
        public int OrderNumber { get; set; }
        //public string UpdateTime { get; set; }
        //public int Mid { get; set; }
        public int FXId { get; set; }

        //队伍平均赔率数据
        public string AverageOdds { get; set; }
        //public string FullAverageOdds { get; set; }
        //public string HalfAverageOdds { get; set; }
        //public decimal KLFlat { get; set; }
        //public decimal KLLose { get; set; }
        //public decimal KLWin { get; set; }
        //public decimal LSFlat { get; set; }
        //public decimal LSLose { get; set; }
        //public decimal LSWin { get; set; }
        //public string YPSW { get; set; }
    }
    public class CTZQ_MatchList_AnteCode
    {
        public string AnteCode { get; set; }
        public int BonusStatus { get; set; }
        public string CurrentSp { get; set; }
        public string FullResult { get; set; }
        public string GameType { get; set; }
        public string GuestTeamId { get; set; }
        public string GuestTeamName { get; set; }
        public string HalfResult { get; set; }
        public string HomeTeamId { get; set; }
        public string HomeTeamName { get; set; }
        public bool IsDan { get; set; }
        public string IssuseNumber { get; set; }
        public string LeagueColor { get; set; }
        public string LeagueName { get; set; }
        public int LetBall { get; set; }
        public string MatchId { get; set; }
        public string MatchIdName { get; set; }
        public string MatchResult { get; set; }
        public decimal MatchResultSp { get; set; }
        public string MatchState { get; set; }
        public DateTime StartTime { get; set; }
        public int Amount { get; set; }
        public int BetCount { get; set; }
        public string Detail_RF { get; set; }
        public string Detail_YSZF { get; set; }
        public int OrderNumber { get; set; }
    }

    /// <summary>
    /// 竞彩足球队伍信息
    /// </summary>
    [Serializable]
    public class JCZQ_MatchInfo_WEB
    {
        //队伍基础信息
        //public string CreateTime { get; set; }
        //public string DSStopBettingTime { get; set; }
        public DateTime MatcheDateTime { get; set; }
        /// <summary>
        /// 比赛日期
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛id
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛期号
        /// </summary>
        public string MatchNumber { get; set; }
        //public decimal FlatOdds { get; set; }
        /// <summary>
        /// 复式投注结束时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        //public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        //public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        //public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        //public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 开比赛时间
        /// </summary>
        public DateTime StartDateTime { get; set; }
        //public decimal WinOdds { get; set; }
        //public int Mid { get; set; }
        public int FXId { get; set; }
        public string MatchStopDesc { get; set; }
        public string PrivilegesType { get; set; }

        //队伍结果信息
        //public string BF_Result { get; set; }
        //public decimal BF_SP { get; set; }
        //public string BQC_Result { get; set; }
        //public decimal BQC_SP { get; set; }
        //public int FullGuestTeamScore { get; set; }
        //public int FullHomeTeamScore { get; set; }
        //public int HalfGuestTeamScore { get; set; }
        //public int HalfHomeTeamScore { get; set; }
        //public string MatchState { get; set; }
        //public string SPF_Result { get; set; }
        //public decimal SPF_SP { get; set; }
        //public string ZJQ_Result { get; set; }
        //public decimal ZJQ_SP { get; set; }

        //让球胜平负-SP数据
        public decimal SP_Flat_Odds { get; set; }
        public decimal SP_Lose_Odds { get; set; }
        public decimal SP_Win_Odds { get; set; }
        public string NoSaleState_SPF { get; set; }

        //胜平负-SP数据
        public decimal SP_Flat_Odds_BRQ { get; set; }
        public decimal SP_Lose_Odds_BRQ { get; set; }
        public decimal SP_Win_Odds_BRQ { get; set; }
        public string NoSaleState_BRQSPF { get; set; }

        //总进球-SP数据
        public decimal JinQiu_0_Odds { get; set; }
        public decimal JinQiu_1_Odds { get; set; }
        public decimal JinQiu_2_Odds { get; set; }
        public decimal JinQiu_3_Odds { get; set; }
        public decimal JinQiu_4_Odds { get; set; }
        public decimal JinQiu_5_Odds { get; set; }
        public decimal JinQiu_6_Odds { get; set; }
        public decimal JinQiu_7_Odds { get; set; }
        public string NoSaleState_ZJQ { get; set; }

        //比分-SP数据
        public decimal F_01 { get; set; }
        public decimal F_02 { get; set; }
        public decimal F_03 { get; set; }
        public decimal F_04 { get; set; }
        public decimal F_05 { get; set; }
        public decimal F_12 { get; set; }
        public decimal F_13 { get; set; }
        public decimal F_14 { get; set; }
        public decimal F_15 { get; set; }
        public decimal F_23 { get; set; }
        public decimal F_24 { get; set; }
        public decimal F_25 { get; set; }
        public decimal F_QT { get; set; }
        public decimal P_00 { get; set; }
        public decimal P_11 { get; set; }
        public decimal P_22 { get; set; }
        public decimal P_33 { get; set; }
        public decimal P_QT { get; set; }
        public decimal S_10 { get; set; }
        public decimal S_20 { get; set; }
        public decimal S_21 { get; set; }
        public decimal S_30 { get; set; }
        public decimal S_31 { get; set; }
        public decimal S_32 { get; set; }
        public decimal S_40 { get; set; }
        public decimal S_41 { get; set; }
        public decimal S_42 { get; set; }
        public decimal S_50 { get; set; }
        public decimal S_51 { get; set; }
        public decimal S_52 { get; set; }
        public decimal S_QT { get; set; }
        public string NoSaleState_BF { get; set; }

        //半全场-SP数据
        public decimal F_F_Odds { get; set; }
        public decimal F_P_Odds { get; set; }
        public decimal F_SH_Odds { get; set; }
        public decimal P_F_Odds { get; set; }
        public decimal P_P_Odds { get; set; }
        public decimal P_SH_Odds { get; set; }
        public decimal SH_F_Odds { get; set; }
        public decimal SH_P_Odds { get; set; }
        public decimal SH_SH_Odds { get; set; }
        public string State { get; set; }
        public string State_HHDG { get; set; }
        public string NoSaleState_BQC { get; set; }

    }

    /// <summary>
    /// 竞彩篮球队伍信息
    /// </summary>
    [Serializable]
    public class JCLQ_MatchInfo_WEB
    {
        //队伍基础信息
        //public decimal AverageLose { get; set; }
        //public decimal AverageWin { get; set; }
        //public string CreateTime { get; set; }
        //public string DSStopBettingTime { get; set; }
        public DateTime MatcheDateTime { get; set; }
        /// <summary>
        /// 复式投注结时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        //public int GuestTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        //public int HomeTeamId { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        //public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        /// <summary>
        /// 比赛日期
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        public string MatchNumber { get; set; }
        public DateTime StartDateTime { get; set; }
        //public int Mid { get; set; }
        public int FXId { get; set; }
        public string PrivilegesType { get; set; }
        public string State_HHDG { get; set; }

        //队伍结果信息
        /// <summary>
        /// 大小分结果
        /// </summary>
        public string DXF_Result { get; set; }
        public decimal DXF_SP { get; set; }
        public string DXF_Trend { get; set; }
        /// <summary>
        /// 客队得分
        /// </summary>
        public int GuestScore { get; set; }
        /// <summary>
        /// 主队得分
        /// </summary>
        public int HomeScore { get; set; }
        public string MatchState { get; set; }
        /// <summary>
        /// 让分胜负结果
        /// </summary>
        public string RFSF_Result { get; set; }
        public decimal RFSF_SP { get; set; }
        public string RFSF_Trend { get; set; }
        /// <summary>
        /// 胜负结果
        /// </summary>
        public string SF_Result { get; set; }
        public decimal SF_SP { get; set; }
        /// <summary>
        /// 胜分差结果
        /// </summary>
        public string SFC_Result { get; set; }
        public decimal SFC_SP { get; set; }

        //胜负-SP数据
        public decimal SF_LoseSP { get; set; }
        public decimal SF_WinSP { get; set; }

        //让分胜负-SP数据
        public decimal RF_LoseSP { get; set; }
        public decimal RF { get; set; }
        public decimal RF_WinSP { get; set; }

        //胜分差-SP数据
        public decimal GuestWin1_5 { get; set; }
        public decimal GuestWin11_15 { get; set; }
        public decimal GuestWin16_20 { get; set; }
        public decimal GuestWin21_25 { get; set; }
        public decimal GuestWin26 { get; set; }
        public decimal GuestWin6_10 { get; set; }
        public decimal HomeWin1_5 { get; set; }
        public decimal HomeWin11_15 { get; set; }
        public decimal HomeWin16_20 { get; set; }
        public decimal HomeWin21_25 { get; set; }
        public decimal HomeWin26 { get; set; }
        public decimal HomeWin6_10 { get; set; }

        //大小分-SP数据
        public decimal DF { get; set; }
        public decimal XF { get; set; }
        public decimal YSZF { get; set; }
    }

    #region 混合单关

    /// <summary>
    /// 竞彩足球混合单关基类
    /// </summary>
    public class JCZQHHDGBase
    {
        /// <summary>
        /// 比赛日期
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛期号
        /// </summary>
        public string MatchNumber { get; set; }
        public string CreateTime { get; set; }
        /// <summary>
        /// 单式结束时间
        /// </summary>
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式结束时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        public int FXId { get; set; }
        /// <summary>
        /// 平 sp
        /// </summary>
        public decimal FlatOdds { get; set; }
        public string Gi { get; set; }
        /// <summary>
        /// 客队id
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        public string Hi { get; set; }
        /// <summary>
        /// 主队id
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        public int Id { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 联赛id
        /// </summary>
        public int LeagueId { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 让球
        /// </summary>
        public int LetBall { get; set; }
        /// <summary>
        /// 负 sp
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        public string MatchIdName { get; set; }
        public string MatchStopDesc { get; set; }
        public int Mid { get; set; }
        public string PrivilegesType { get; set; }
        /// <summary>
        /// 客队短名称
        /// </summary>
        public string ShortGuestTeamName { get; set; }
        /// <summary>
        /// 主队短名称
        /// </summary>
        public string ShortHomeTeamName { get; set; }
        /// <summary>
        /// 联赛短名称
        /// </summary>
        public string ShortLeagueName { get; set; }
        public string StartDateTime { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 胜 sp
        /// </summary>
        public decimal WinOdds { get; set; }
        public string BF { get; set; }
        public string BQC { get; set; }
        public string BRQSPF { get; set; }
        public string SPF { get; set; }
        public string ZJQ { get; set; }
    }
    /// <summary>
    /// 竞彩篮球混合单关基类
    /// </summary>
    public class JCLQHHDGBase
    {
        /// <summary>
        /// 比赛时间
        /// </summary>
        public string MatcheDateTime { get; set; }
        /// <summary>
        /// 复式投注结束时间
        /// </summary>
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 比赛颜色
        /// </summary>
        public string LeagueColor { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        public string LeagueName { get; set; }
        /// <summary>
        /// 比赛编号名称 
        /// </summary>
        public string MatchIdName { get; set; }
        public string StartDateTime { get; set; }
        /// <summary>
        /// 比赛日期
        /// </summary>
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号
        /// </summary>
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛期号
        /// </summary>
        public string MatchNumber { get; set; }
        public int FXId { get; set; }
        public string State { get; set; }
        public string PrivilegesType { get; set; }
        public string DXF { get; set; }
        public string RFSF { get; set; }
        public string SFC { get; set; }
        public string SF { get; set; }
    }

    #endregion

    #region 缓存相关

    /// <summary>
    /// 登录日志
    /// </summary>
    public class LoginLogInfo
    {
        public string LoginName { get; set; }
        public DateTime LoginTime { get; set; }
    }

    /// <summary>
    /// 用户信息刷新日志
    /// </summary>
    public class UserRefreshInfo
    {
        public string LoginName { get; set; }
        public DateTime LoadTime { get; set; }
    }

    #endregion


    #region new
    /// <summary>
    /// 期号和比赛相关时间
    /// </summary>
    public class PhoneTermInfo_JSON
    {
        public string tag2 { get; set; }
        public string stopSaleTime { get; set; }
        public string openPrizeTime { get; set; }
        public string max { get; set; }
        public string tag { get; set; }
        public string termNo { get; set; }
        public string gid { get; set; }
        public string type { get; set; }
        public int deadLine { get; set; }
    }

    /// <summary>
    /// 广告轮播
    /// </summary>
    public class RecordTotal
    {
        public int total { get; set; }
        public List<Record> records { get; set; }
    }

    public class Record
    {
        public string desc { get; set; }
        public int status { get; set; }
        public string name { get; set; }
        public string image { get; set; }
        public int type { get; set; }
        public string url { get; set; }
        public int row { get; set; }
    }

    /// <summary>
    /// 开奖接口
    /// </summary>
    [Serializable]
    public class KaiJiang
    {
        public string result { get; set; }
        public object prizepool { get; set; }
        public string nums { get; set; }
        public string name { get; set; }
        public string termNo { get; set; }
        public string ver { get; set; }
        public string grades { get; set; }
        public string date { get; set; }
        public string type { get; set; }
        public string sale { get; set; }
    }

    /// <summary>
    /// 数字彩奖池：双色球、大乐透
    /// </summary>
    public class Web_SZC_BonusPoolInfo
    {
        /// <summary>
        /// 彩种
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 奖等
        /// </summary>
        public List<Web_GradeList> GradeList { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 来源
        /// </summary>
        public string Source { get; set; }
        public int TotalBonusCount { get; set; }
        public decimal TotalBonusMoney { get; set; }
        public decimal TotalPrizePoolMoney { get; set; }
        public decimal TotalSellMoney { get; set; }
        public string WinNumber { get; set; }
    }

    public class Web_GradeList
    {
        public string Attr { get; set; }
        public int BonusCount { get; set; }
        public decimal BonusMoney { get; set; }
        public int Grade { get; set; }
        public int GradeIndex { get; set; }
        public string GradeName { get; set; }
    }


    /// <summary>
    /// 开奖详情
    /// </summary>
    public class PrizelevelInfo
    {
        public string stopSendPrizeTime { get; set; }
        public List<Prizelevel> prizeLevel { get; set; }
    }

    public class Prizelevel
    {
        public string betnum { get; set; }
        public string prize { get; set; }
        public string name { get; set; }
    }


    /// <summary>
    /// 开奖历史
    /// </summary>
    public class KaiJiangHistory
    {
        public string result { get; set; }
        public string time { get; set; }
        public string prizepool { get; set; }
        public string term { get; set; }
        public string type { get; set; }
        public string sale { get; set; }
    }

    /// <summary>
    /// 传统足球开奖详情页内请求
    /// </summary>
    public class KaiJiangOpenMatch
    {
        public string result { get; set; }
        public int match_point { get; set; }
        public string whole_score { get; set; }
        public string match_name { get; set; }
        public string away_team { get; set; }
        public string match_state { get; set; }
        public string home_team { get; set; }
        public string half_score { get; set; }
        public string bout_index { get; set; }
        public string match_time { get; set; }
    }
    [Serializable]
    public class CtzqIssuesWeb
    {
        public string CreateTime { get; set; }
        public string DSStopBettingTime { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string Id { get; set; }
        public string IssuseNumber { get; set; }
        public string OfficialStopTime { get; set; }
        public string StartTime { get; set; }
        public string StopBettingTime { get; set; }
        public string WinNumber { get; set; }
    }



    public class Web_CTZQ_BonusPoolInfo
    {
        public int BonusBalance { get; set; }
        public int BonusCount { get; set; }
        public int BonusLevel { get; set; }
        public string BonusLevelDisplayName { get; set; }
        public int BonusMoney { get; set; }
        public string CreateTime { get; set; }
        public string GameCode { get; set; }
        public string GameType { get; set; }
        public string Id { get; set; }
        public string IssuseNumber { get; set; }
        public string MatchResult { get; set; }
        public int TotalSaleMoney { get; set; }
    }
    #endregion
}