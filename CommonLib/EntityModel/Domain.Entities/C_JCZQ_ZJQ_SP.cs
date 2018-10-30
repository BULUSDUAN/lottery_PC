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
    /// 竞彩足球赛事信息
    /// </summary>
    [Entity("C_JCZQ_ZJQ_SP", Type = EntityType.Table)]
    public class C_JCZQ_ZJQ_SP : IBallBaseInfo
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
        /// 进球数 0
        /// </summary>
        [Field("JinQiu_0_Odds")]
        public decimal JinQiu_0_Odds { get; set; }
        /// <summary>
        /// 进球数 1
        /// </summary>
        [Field("JinQiu_1_Odds")]
        public decimal JinQiu_1_Odds { get; set; }
        /// <summary>
        /// 进球数 2
        /// </summary>
        [Field("JinQiu_2_Odds")]
        public decimal JinQiu_2_Odds { get; set; }
        /// <summary>
        /// 进球数 3
        /// </summary>
        [Field("JinQiu_3_Odds")]
        public decimal JinQiu_3_Odds { get; set; }
        /// <summary>
        /// 进球数 4
        /// </summary>
        [Field("JinQiu_4_Odds")]
        public decimal JinQiu_4_Odds { get; set; }
        /// <summary>
        /// 进球数 5
        /// </summary>
        [Field("JinQiu_5_Odds")]
        public decimal JinQiu_5_Odds { get; set; }
        /// <summary>
        /// 进球数 6
        /// </summary>
        [Field("JinQiu_6_Odds")]
        public decimal JinQiu_6_Odds { get; set; }
        /// <summary>
        /// 进球数 7
        /// </summary>
        [Field("JinQiu_7_Odds")]
        public decimal JinQiu_7_Odds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }
        public string NoSaleState { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_JCZQ_ZJQ_SP;
            if (t.JinQiu_0_Odds != this.JinQiu_0_Odds
                || t.JinQiu_1_Odds != this.JinQiu_1_Odds
                || t.JinQiu_2_Odds != this.JinQiu_2_Odds
                || t.JinQiu_3_Odds != this.JinQiu_3_Odds
                || t.JinQiu_4_Odds != this.JinQiu_4_Odds
                || t.JinQiu_5_Odds != this.JinQiu_5_Odds
                || t.JinQiu_6_Odds != this.JinQiu_6_Odds
                || t.JinQiu_7_Odds != this.JinQiu_7_Odds
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }


}
