using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace MatchBiz.Core
{
    /// <summary>
    /// 北京单场胜平负SP值
    /// </summary>
    [CommunicationObject]
    public class BJDC_SPF_SpInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get { return "SPF"; } }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 胜赔率
        /// </summary>
        public decimal Win_Odds { get; set; }
        /// <summary>
        /// 平赔率
        /// </summary>
        public decimal Flat_Odds { get; set; }
        /// <summary>
        /// 负赔率
        /// </summary>
        public decimal Lose_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        public override bool Equals(object obj)
        {
            var t = obj as BJDC_SPF_SpInfo;
            if (t.MatchOrderId != this.MatchOrderId
                || t.IssuseNumber != this.IssuseNumber
                || t.Win_Odds != this.Win_Odds
                || t.Flat_Odds != this.Flat_Odds
                || t.Lose_Odds != this.Lose_Odds
                )
                return false;

            return true;
        }
    }

    [CommunicationObject]
    public class BJDC_SPF_SpInfoCollection : List<BJDC_SPF_SpInfo>
    {
    }

    /// <summary>
    /// 北京单场总进球SP值
    /// </summary>
    [CommunicationObject]
    public class BJDC_ZJQ_SpInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get { return "ZJQ"; } }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 进球数 0
        /// </summary>
        public decimal JinQiu_0_Odds { get; set; }
        /// <summary>
        /// 进球数 1
        /// </summary>
        public decimal JinQiu_1_Odds { get; set; }
        /// <summary>
        /// 进球数 2
        /// </summary>
        public decimal JinQiu_2_Odds { get; set; }
        /// <summary>
        /// 进球数 3
        /// </summary>
        public decimal JinQiu_3_Odds { get; set; }
        /// <summary>
        /// 进球数 4
        /// </summary>
        public decimal JinQiu_4_Odds { get; set; }
        /// <summary>
        /// 进球数 5
        /// </summary>
        public decimal JinQiu_5_Odds { get; set; }
        /// <summary>
        /// 进球数 6
        /// </summary>
        public decimal JinQiu_6_Odds { get; set; }
        /// <summary>
        /// 进球数 7
        /// </summary>
        public decimal JinQiu_7_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        public override bool Equals(object obj)
        {
            var t = obj as BJDC_ZJQ_SpInfo;
            if (t.IssuseNumber != this.IssuseNumber
                || t.MatchOrderId != this.MatchOrderId
                || t.JinQiu_0_Odds != this.JinQiu_0_Odds
                || t.JinQiu_1_Odds != this.JinQiu_1_Odds
                || t.JinQiu_2_Odds != this.JinQiu_2_Odds
                || t.JinQiu_3_Odds != this.JinQiu_3_Odds
                || t.JinQiu_4_Odds != this.JinQiu_4_Odds
                || t.JinQiu_5_Odds != this.JinQiu_5_Odds
                || t.JinQiu_6_Odds != this.JinQiu_6_Odds
                || t.JinQiu_7_Odds != this.JinQiu_7_Odds
                )
                return false;

            return true;
        }
    }

    [CommunicationObject]
    public class BJDC_ZJQ_SpInfoCollection : List<BJDC_ZJQ_SpInfo>
    {
    }

    /// <summary>
    /// 北京单场上下单双SP值
    /// </summary>
    [CommunicationObject]
    public class BJDC_SXDS_SpInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get { return "SXDS"; } }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
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
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        public override bool Equals(object obj)
        {
            var t = obj as BJDC_SXDS_SpInfo;
            if (t.IssuseNumber != this.IssuseNumber
                || t.MatchOrderId != this.MatchOrderId
                || t.SH_D_Odds != this.SH_D_Odds
                || t.SH_S_Odds != this.SH_S_Odds
                || t.X_D_Odds != this.X_D_Odds
                || t.X_S_Odds != this.X_S_Odds
                )
                return false;

            return true;
        }
    }

    [CommunicationObject]
    public class BJDC_SXDS_SpInfoCollection : List<BJDC_SXDS_SpInfo>
    {
    }

    /// <summary>
    /// 北京单场比分SP值
    /// </summary>
    [CommunicationObject]
    public class BJDC_BF_SpInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get { return "BF"; } }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 胜-其它
        /// </summary>
        public decimal S_QT { get; set; }
        /// <summary>
        /// 胜-10
        /// </summary>
        public decimal S_10 { get; set; }
        /// <summary>
        /// 胜-20
        /// </summary>
        public decimal S_20 { get; set; }
        /// <summary>
        /// 胜-21
        /// </summary>
        public decimal S_21 { get; set; }
        /// <summary>
        /// 胜-30
        /// </summary>
        public decimal S_30 { get; set; }
        /// <summary>
        /// 胜-31
        /// </summary>
        public decimal S_31 { get; set; }
        /// <summary>
        /// 胜-32
        /// </summary>
        public decimal S_32 { get; set; }
        /// <summary>
        /// 胜-40
        /// </summary>
        public decimal S_40 { get; set; }
        /// <summary>
        /// 胜-41
        /// </summary>
        public decimal S_41 { get; set; }
        /// <summary>
        /// 胜-42
        /// </summary>
        public decimal S_42 { get; set; }
        /// <summary>
        /// 平-其它
        /// </summary>
        public decimal P_QT { get; set; }
        /// <summary>
        /// 平-00
        /// </summary>
        public decimal P_00 { get; set; }
        /// <summary>
        /// 平-11
        /// </summary>
        public decimal P_11 { get; set; }
        /// <summary>
        /// 平-22
        /// </summary>
        public decimal P_22 { get; set; }
        /// <summary>
        /// 平-33
        /// </summary>
        public decimal P_33 { get; set; }
        /// <summary>
        /// 负-其它
        /// </summary>
        public decimal F_QT { get; set; }
        /// <summary>
        /// 负-01
        /// </summary>
        public decimal F_01 { get; set; }
        /// <summary>
        /// 负-02
        /// </summary>
        public decimal F_02 { get; set; }
        /// <summary>
        /// 负-12
        /// </summary>
        public decimal F_12 { get; set; }
        /// <summary>
        /// 负-03
        /// </summary>
        public decimal F_03 { get; set; }
        /// <summary>
        /// 负-13
        /// </summary>
        public decimal F_13 { get; set; }
        /// <summary>
        /// 负-23
        /// </summary>
        public decimal F_23 { get; set; }
        /// <summary>
        /// 负-04
        /// </summary>
        public decimal F_04 { get; set; }
        /// <summary>
        /// 负-14
        /// </summary>
        public decimal F_14 { get; set; }
        /// <summary>
        /// 负-24
        /// </summary>
        public decimal F_24 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        public override bool Equals(object obj)
        {
            var t = obj as BJDC_BF_SpInfo;
            if (t.IssuseNumber != this.IssuseNumber
                || t.MatchOrderId != this.MatchOrderId
                || t.S_QT != this.S_QT
                || t.S_10 != this.S_10
                || t.S_20 != this.S_20
                || t.S_21 != this.S_21
                || t.S_30 != this.S_30
                || t.S_31 != this.S_31
                || t.S_32 != this.S_32
                || t.S_40 != this.S_40
                || t.S_41 != this.S_41
                || t.S_42 != this.S_42
                || t.P_QT != this.P_QT
                || t.P_00 != this.P_00
                || t.P_11 != this.P_11
                || t.P_22 != this.P_22
                || t.P_33 != this.P_33
                || t.F_QT != this.F_QT
                || t.F_01 != this.F_01
                || t.F_02 != this.F_02
                || t.F_12 != this.F_12
                || t.F_03 != this.F_03
                || t.F_13 != this.F_13
                || t.F_23 != this.F_23
                || t.F_04 != this.F_04
                || t.F_14 != this.F_14
                || t.F_24 != this.F_24
                )
                return false;

            return true;
        }
    }

    [CommunicationObject]
    public class BJDC_BF_SpInfoCollection : List<BJDC_BF_SpInfo>
    {
    }

    /// <summary>
    /// 北京单场半全场SP值
    /// </summary>
    [CommunicationObject]
    public class BJDC_BQC_SpInfo
    {
        /// <summary>
        /// 玩法编码
        /// </summary>
        public string GameType { get { return "BQC"; } }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 胜-胜
        /// </summary>
        public decimal SH_SH_Odds { get; set; }
        /// <summary>
        /// 胜-平
        /// </summary>
        public decimal SH_P_Odds { get; set; }
        /// <summary>
        /// 胜-负
        /// </summary>
        public decimal SH_F_Odds { get; set; }
        /// <summary>
        /// 平-胜
        /// </summary>
        public decimal P_SH_Odds { get; set; }
        /// <summary>
        /// 平-平
        /// </summary>
        public decimal P_P_Odds { get; set; }
        /// <summary>
        /// 平-负
        /// </summary>
        public decimal P_F_Odds { get; set; }
        /// <summary>
        /// 负-胜
        /// </summary>
        public decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 负-平
        /// </summary>
        public decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负-负
        /// </summary>
        public decimal F_F_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
        public override bool Equals(object obj)
        {
            var t = obj as BJDC_BQC_SpInfo;
            if (t.IssuseNumber != this.IssuseNumber
                || t.MatchOrderId != this.MatchOrderId
                || t.SH_SH_Odds != this.SH_SH_Odds
                || t.SH_P_Odds != this.SH_P_Odds
                || t.SH_F_Odds != this.SH_F_Odds
                || t.P_SH_Odds != this.P_SH_Odds
                || t.P_P_Odds != this.P_P_Odds
                || t.P_F_Odds != this.P_F_Odds
                || t.F_SH_Odds != this.F_SH_Odds
                || t.F_P_Odds != this.F_P_Odds
                || t.F_F_Odds != this.F_F_Odds
                )
                return false;

            return true;
        }

    }

    [CommunicationObject]
    public class BJDC_BQC_SpInfoCollection : List<BJDC_BQC_SpInfo>
    {
    }

    /// <summary>
    /// 北京单场期号信息
    /// </summary>
    public class BJDC_IssuseInfo
    {
        public string IssuseNumber { get; set; }
        public string MinMatchStartTime { get; set; }
        public string MinLocalStopTime { get; set; }
    }

    /// <summary>
    /// 北京单场队伍信息
    /// </summary>
    public class BJDC_MatchInfo
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        public int Mid { get; set; }
        public int FXId { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 联赛Id
        /// </summary>
        public int MatchId { get; set; }
        /// <summary>
        /// 联赛名字
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public string MatchStartTime { get; set; }
        /// <summary>
        /// 本地结束时间
        /// </summary>
        public string LocalStopTime { get; set; }
        /// <summary>
        /// 赛事状态
        /// </summary>
        public BJDCMatchState MatchState { get; set; }
        /// <summary>
        /// 联赛背景色
        /// </summary>
        public string MatchColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队排名 有可能是字符串
        /// </summary>
        public string HomeTeamSort { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 客队排名 有可能是字符串
        /// </summary>
        public string GuestTeamSort { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }


        public string Hi { get; set; }
        public string Gi { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as BJDC_MatchInfo;
            if (t.Id != this.Id
                || t.FlatOdds != this.FlatOdds
                || t.HomeTeamSort != this.HomeTeamSort
                || t.GuestTeamSort != this.GuestTeamSort
                || t.MatchStartTime != this.MatchStartTime
                || t.LocalStopTime != this.LocalStopTime
                || t.LetBall != this.LetBall
                || t.LoseOdds != this.LoseOdds
                || t.WinOdds != this.WinOdds)
                return false;

            return true;
        }
    }

    /// <summary>
    /// 北京单场比赛结果
    /// </summary>
    public class BJDC_MatchResultInfo
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 主队半场比分
        /// </summary>
        public string HomeHalf_Result { get; set; }
        /// <summary>
        /// 主队全场比分
        /// </summary>
        public string HomeFull_Result { get; set; }
        /// <summary>
        /// 客队半场比分
        /// </summary>
        public string GuestHalf_Result { get; set; }
        /// <summary>
        /// 客队全场比分
        /// </summary>
        public string GuestFull_Result { get; set; }
        /// <summary>
        /// 胜平负彩果
        /// </summary>
        public string SPF_Result { get; set; }
        /// <summary>
        /// 胜平负开奖sp
        /// </summary>
        public decimal SPF_SP { get; set; }
        /// <summary>
        /// 总进球彩果
        /// </summary>
        public string ZJQ_Result { get; set; }
        /// <summary>
        /// 总进球开奖sp
        /// </summary>
        public decimal ZJQ_SP { get; set; }
        /// <summary>
        /// 上下单双彩果
        /// </summary>
        public string SXDS_Result { get; set; }
        /// <summary>
        /// 上下单双开奖sp
        /// </summary>
        public decimal SXDS_SP { get; set; }
        /// <summary>
        /// 比分彩果
        /// </summary>
        public string BF_Result { get; set; }
        /// <summary>
        /// 比分开奖sp
        /// </summary>
        public decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场彩果
        /// </summary>
        public string BQC_Result { get; set; }
        /// <summary>
        /// 半全场开奖sp
        /// </summary>
        public decimal BQC_SP { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public string MatchState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as BJDC_MatchResultInfo;
            if (t.Id != this.Id
                || t.HomeHalf_Result != this.HomeHalf_Result
                || t.HomeFull_Result != this.HomeFull_Result
                || t.GuestHalf_Result != this.GuestHalf_Result
                || t.GuestFull_Result != this.GuestFull_Result
                || t.SPF_Result != this.SPF_Result
                || t.SPF_SP != this.SPF_SP
                || t.ZJQ_Result != this.ZJQ_Result
                || t.ZJQ_SP != this.ZJQ_SP
                || t.SXDS_Result != this.SXDS_Result
                || t.SXDS_SP != this.SXDS_SP
                || t.BF_Result != this.BF_Result
                || t.BF_SP != this.BF_SP
                || t.BQC_Result != this.BQC_Result
                || t.BQC_SP != this.BQC_SP
                )
                return false;

            return true;
        }
    }



    public class BJDC_SPF_SP_Trend
    {
        /// <summary>
        /// 赔率编号
        /// </summary>
        public string OddsMid { get; set; }

        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 初盘为 0 其它为1
        /// </summary>
        public int TP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public string CreateTime { get; set; }
    }


    /// <summary>
    /// 平均欧赔
    /// </summary>
    public class BJDC_SPF_OZ_SPInfo : BJDC_SPF_SpInfo
    {
        public string OddsMid { get; set; }
        public string Flag { get; set; }
    }
    /// <summary>
    /// 威廉希尔
    /// </summary>
    public class BJDC_SPF_WLXE_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 澳门
    /// </summary>
    public class BJDC_SPF_AM_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 立博
    /// </summary>
    public class BJDC_SPF_LB_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Bet365
    /// </summary>
    public class BJDC_SPF_Bet365_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// SNAI
    /// </summary>
    public class BJDC_SPF_SNAI_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 易德胜
    /// </summary>
    public class BJDC_SPF_YDS_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 韦德
    /// </summary>
    public class BJDC_SPF_WD_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Bwin
    /// </summary>
    public class BJDC_SPF_Bwin_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Coral
    /// </summary>
    public class BJDC_SPF_Coral_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// Oddset
    /// </summary>
    public class BJDC_SPF_Oddset_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }
    /// <summary>
    /// 投注比例
    /// </summary>
    public class BJDC_SPF_TZBL_SPInfo : BJDC_SPF_OZ_SPInfo
    {
    }


    /// <summary>
    /// 队伍对阵历史
    /// </summary>
    public class BJDC_Team_History
    {
        //<r ln="亚洲预" hteam="澳大利亚" ateam="阿曼" mtime="1364286600" hscore="2" ascore="2" bc="0:1" bet="1.75" binfo="输" htid="632" atid="933" cl="393465"></r>
        public string Ln { get; set; }
        public string HTeam { get; set; }
        public string ATeam { get; set; }
        public string MTime { get; set; }
        public int HScore { get; set; }
        public int AScore { get; set; }
        public string Bc { get; set; }
        public string Bet { get; set; }
        public string BInfo { get; set; }
        public string HTId { get; set; }
        public string ATId { get; set; }
        public string Cl { get; set; }
    }

}
