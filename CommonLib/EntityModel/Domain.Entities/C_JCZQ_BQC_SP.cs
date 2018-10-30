using EntityModel.Enum;
using EntityModel.Interface;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    /// <summary>
    /// 竞彩足球 半全场SP
    /// </summary>
  //  [EntityMappingTable("C_JCZQ_BQC_SP")]
    [Entity("C_JCZQ_BQC_SP", Type = EntityType.Table)]
    public class C_JCZQ_BQC_SP : IBallBaseInfo
    {
        public ObjectId _id { get; set; }
        /// <summary>
        /// 比赛Id : 120813001
        /// </summary>
        [Field("MatchId")]
        public string MatchId { get; set; }
        /// <summary>
        /// 比赛日期 : 120813
        /// </summary>
        [Field("MatchData")]
        public string MatchData { get; set; }
        /// <summary>
        /// 比赛编号 ：001
        /// </summary>
        [Field("MatchNumber")]
        public string MatchNumber { get; set; }
        [Field("Id", IsPrimaryKey = true)]
        public long mId { get; set; }
        /// <summary>
        /// 胜-胜
        /// </summary>
        [Field("SH_SH_Odds")]
        public decimal SH_SH_Odds { get; set; }
        /// <summary>
        /// 胜-平
        /// </summary>
        [Field("SH_P_Odds")]
        public decimal SH_P_Odds { get; set; }
        /// <summary>
        /// 胜-负
        /// </summary>
        [Field("SH_F_Odds")]
        public decimal SH_F_Odds { get; set; }
        /// <summary>
        /// 平-胜
        /// </summary>
        [Field("P_SH_Odds")]
        public decimal P_SH_Odds { get; set; }
        /// <summary>
        /// 平-平
        /// </summary>
        [Field("P_P_Odds")]
        public decimal P_P_Odds { get; set; }
        /// <summary>
        /// 平-负
        /// </summary>
        [Field("P_F_Odds")]
        public decimal P_F_Odds { get; set; }
        /// <summary>
        /// 负-胜
        /// </summary>
        [Field("F_SH_Odds")]
        public decimal F_SH_Odds { get; set; }
        /// <summary>
        /// 负-平
        /// </summary>
        [Field("F_P_Odds")]
        public decimal F_P_Odds { get; set; }
        /// <summary>
        /// 负-负
        /// </summary>
        [Field("F_F_Odds")]
        public decimal F_F_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }
        public string NoSaleState { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_JCZQ_BQC_SP;
            if (t.F_F_Odds != this.F_F_Odds
                || t.F_P_Odds != this.F_P_Odds
                || t.F_SH_Odds != this.F_SH_Odds
                || t.P_F_Odds != this.P_F_Odds
                || t.P_P_Odds != this.P_P_Odds
                || t.P_SH_Odds != this.P_SH_Odds
                || t.SH_F_Odds != this.SH_F_Odds
                || t.SH_P_Odds != this.SH_P_Odds
                || t.SH_SH_Odds != this.SH_SH_Odds
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }
}
