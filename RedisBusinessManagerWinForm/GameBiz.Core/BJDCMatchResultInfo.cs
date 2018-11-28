using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace GameBiz.Core
{
    [CommunicationObject]
    public class BJDCMatchResultInfo
    {
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        public  string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        public  string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        public  int MatchOrderId { get; set; }
        /// <summary>
        /// 主队半场比分
        /// </summary>
        public  string HomeHalf_Result { get; set; }
        /// <summary>
        /// 主队全场比分
        /// </summary>
        public  string HomeFull_Result { get; set; }
        /// <summary>
        /// 客队半场比分
        /// </summary>
        public  string GuestHalf_Result { get; set; }
        /// <summary>
        /// 客队全场比分
        /// </summary>
        public  string GuestFull_Result { get; set; }
        /// <summary>
        /// 胜平负彩果
        /// </summary>
        public  string SPF_Result { get; set; }
        /// <summary>
        /// 胜平负开奖sp
        /// </summary>
        public  decimal SPF_SP { get; set; }
        /// <summary>
        /// 总进球彩果
        /// </summary>
        public  string ZJQ_Result { get; set; }
        /// <summary>
        /// 总进球开奖sp
        /// </summary>
        public  decimal ZJQ_SP { get; set; }
        /// <summary>
        /// 上下单双彩果
        /// </summary>
        public  string SXDS_Result { get; set; }
        /// <summary>
        /// 上下单双开奖sp
        /// </summary>
        public  decimal SXDS_SP { get; set; }
        /// <summary>
        /// 比分彩果
        /// </summary>
        public  string BF_Result { get; set; }
        /// <summary>
        /// 比分开奖sp
        /// </summary>
        public  decimal BF_SP { get; set; }
        /// <summary>
        /// 半全场彩果
        /// </summary>
        public  string BQC_Result { get; set; }
        /// <summary>
        /// 半全场开奖sp
        /// </summary>
        public  decimal BQC_SP { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        public  string MatchState { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public  DateTime CreateTime { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        public virtual decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        public virtual decimal LoseOdds { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        public  decimal WinOdds { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        public  string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        public  string GuestTeamName { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        public int LetBall { get; set; }
        /// <summary>
        /// 联赛背景色
        /// </summary>
        public string MatchColor { get; set; }
        /// <summary>
        /// 联赛名字
        /// </summary>
        public string MatchName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public  DateTime MatchStartTime { get; set; }

        

    }
    [CommunicationObject]
    public class BJDCMatchResultInfo_Collection
    {
        public BJDCMatchResultInfo_Collection()
        {
            ListInfo = new List<BJDCMatchResultInfo>();
        }
        public int TotalCount { get; set; }
        public List<BJDCMatchResultInfo> ListInfo { get; set; }
    }

    [CommunicationObject]
    public class BJDCIssuseInfo
    {
        /// <summary>
        /// 最新期号
        /// </summary>
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 本地截止时间
        /// </summary>
        public string MinLocalStopTime { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        public string MinMatchStartTime { get; set; }
    }
}
