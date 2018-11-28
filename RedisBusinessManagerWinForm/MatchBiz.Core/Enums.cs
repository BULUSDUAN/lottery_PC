using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MatchBiz.Core
{
    /// <summary>
    /// 传统足球玩法类别
    /// </summary>
    public enum CTZQMatchCategory
    {
        /// <summary>
        /// 14场胜负
        /// </summary>
        T14C = 0,
        /// <summary>
        /// 胜负任9
        /// </summary>
        TR9 = 1,
        /// <summary>
        /// 6场半全
        /// </summary>
        T6BQC = 2,
        /// <summary>
        /// 4场进球
        /// </summary>
        T4CJQ = 3,
    }

    /// <summary>
    /// 足球胜平负结果
    /// </summary>
    public enum MatchResult
    {
        /// <summary>
        /// 胜
        /// </summary>
        Win = 3,
        /// <summary>
        /// 平
        /// </summary>
        Flat = 1,
        /// <summary>
        /// 负
        /// </summary>
        Lose = 0,
    }
    /// <summary>
    /// 传统足球赛事状态
    /// </summary>
    public enum CTZQMatchState
    {
        Waiting = 0,
        Running = 10,
        Finished = 20,
    }

    /// <summary>
    /// 北单比赛状态
    /// </summary>
    public enum BJDCMatchState
    {
        /// <summary>
        /// 销售中
        /// </summary>
        Sales = 0,
        /// <summary>
        /// 停止
        /// </summary>
        Stop = 1,
    }

    public enum DBChangeState
    {
        Add = 10,
        Update = 20,
    }
    public enum NoticeCategory
    {
        /// <summary>
        /// 传统足球队伍
        /// </summary>
        CTZQ_Match = 10,
        /// <summary>
        /// 传统足球赔率
        /// </summary>
        CTZQ_Odds = 11,
        /// <summary>
        /// 传统足球奖期
        /// </summary>
        CTZQ_Issuse = 12,
        /// <summary>
        /// 传统足球奖池奖级
        /// </summary>
        CTZQ_MatchPoolLevel = 13,

        /// <summary>
        /// 北单期号
        /// </summary>
        BJDC_Issuse = 22,
        /// <summary>
        /// 北单队伍
        /// </summary>
        BJDC_Match = 20,
        /// <summary>
        /// 北单结果
        /// </summary>
        BJDC_MatchResult = 21,
        /// <summary>
        /// 北单Sp
        /// </summary>
        //BJDC_MatchResult_SP = 22,
        /// <summary>
        /// 北单胜负过关
        /// </summary>
        BJDC_Match_SFGG = 25,
        /// <summary>
        /// 北单结果
        /// </summary>
        BJDC_MatchResult_SFGG = 26,

        /// <summary>
        /// 竞彩足球队伍
        /// </summary>
        JCZQ_Match = 30,
        /// <summary>
        /// 竞彩足球SPF SP
        /// </summary>
        JCZQ_SPF_SP = 31,
        /// <summary>
        /// 竞彩足球BF SP
        /// </summary>
        JCZQ_BF_SP = 32,
        /// <summary>
        /// 竞彩足球ZJQ SP
        /// </summary>
        JCZQ_ZJQ_SP = 33,
        /// <summary>
        /// 竞彩足球BQC SP
        /// </summary>
        JCZQ_BQC_SP = 34,
        /// <summary>
        /// 竞彩足球比赛结果
        /// </summary>
        JCZQ_MatchResult = 35,
        /// <summary>
        /// 不让球胜平负SP
        /// </summary>
        JCZQ_BRQSPF_SP = 36,

        /// <summary>
        /// 竞彩足球SPF SP DS
        /// </summary>
        JCZQ_SPF_SP_DS = 310,
        /// <summary>
        /// 竞彩足球BF SP DS
        /// </summary>
        JCZQ_BF_SP_DS = 320,
        /// <summary>
        /// 竞彩足球ZJQ SP DS
        /// </summary>
        JCZQ_ZJQ_SP_DS = 330,
        /// <summary>
        /// 竞彩足球BQC SP DS
        /// </summary>
        JCZQ_BQC_SP_DS = 340,
        /// <summary>
        /// 不让球胜平负SP
        /// </summary>
        JCZQ_BRQSPF_SP_DS = 360,


        /// <summary>
        /// 竞彩篮球队伍
        /// </summary>
        JCLQ_Match = 40,
        /// <summary>
        /// 竞彩篮球 SF SP
        /// </summary>
        JCLQ_SF_SP = 41,
        /// <summary>
        /// 竞彩篮球 RFSF SP
        /// </summary>
        JCLQ_RFSF_SP = 42,
        /// <summary>
        /// 竞彩篮球 SFC SP
        /// </summary>
        JCLQ_SFC_SP = 43,
        /// <summary>
        /// 竞彩篮球 DXF SP
        /// </summary>
        JCLQ_DXF_SP = 44,

        /// <summary>
        /// 竞彩篮球 SF SP
        /// </summary>
        JCLQ_SF_SP_DS = 410,
        /// <summary>
        /// 竞彩篮球 RFSF SP
        /// </summary>
        JCLQ_RFSF_SP_DS = 420,
        /// <summary>
        /// 竞彩篮球 SFC SP
        /// </summary>
        JCLQ_SFC_SP_DS = 430,
        /// <summary>
        /// 竞彩篮球 DXF SP
        /// </summary>
        JCLQ_DXF_SP_DS = 440,


        /// <summary>
        /// 竞彩篮球比赛结果
        /// </summary>
        JCLQ_MatchResult = 45,
        JCSJB_GJ = 110,
        JCSJB_GYJ = 130,
    }
}
