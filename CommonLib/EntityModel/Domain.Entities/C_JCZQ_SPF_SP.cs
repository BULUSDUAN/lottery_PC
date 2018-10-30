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
    /// 竞彩足球 胜平负SP
    /// </summary>
    [Entity("C_JCZQ_SPF_SP", Type = EntityType.Table)]
    public class C_JCZQ_SPF_SP : IBallBaseInfo
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
        /// 胜 平均赔率
        /// </summary>
        [Field("WinOdds")]
        public decimal WinOdds { get; set; }
        /// <summary>
        /// 平 平均赔率
        /// </summary>
        [Field("FlatOdds")]
        public decimal FlatOdds { get; set; }
        /// <summary>
        /// 负 平均赔率
        /// </summary>
        [Field("LoseOdds")]
        public decimal LoseOdds { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }
        public string NoSaleState { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as C_JCZQ_SPF_SP;
            if (t.WinOdds != this.WinOdds
                || t.FlatOdds != this.FlatOdds
                || t.LoseOdds != this.LoseOdds
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }
}
