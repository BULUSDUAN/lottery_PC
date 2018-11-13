using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{

    /// <summary>
    /// 北京单场胜负过关比赛信息
    /// </summary>
   // [EntityMappingTable("C_BJDC_Match_SFGG")]
    [Entity("C_BJDC_Match_SFGG", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class BJDC_Match_SFGG
    {
        [BsonId]
        public ObjectId _id { get; set; }
        /// <summary>
        /// IssuseNumber|MatchOrderId
        /// </summary>
        [Field("Id", IsPrimaryKey =true)]
        public string Id { get; set; }
        /// <summary>
        /// 期号
        /// </summary>
        [Field("IssuseNumber")]
        public string IssuseNumber { get; set; }
        /// <summary>
        /// 联赛排序编号
        /// </summary>
        [Field("MatchOrderId")]
        public int MatchOrderId { get; set; }
        /// <summary>
        /// 类别：足球、篮球、橄榄球等
        /// </summary>
        [Field("Category")]
        public string Category { get; set; }
        /// <summary>
        /// 联赛名字
        /// </summary>
        [Field("MatchName")]
        public string MatchName { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("MatchStartTime")]
        public string MatchStartTime { get; set; }
        /// <summary>
        /// 本投注截止时间
        /// </summary>
        [Field("BetStopTime")]
        public string BetStopTime { get; set; }
        /// <summary>
        /// 赛事状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 让球数
        /// </summary>
        [Field("LetBall")]
        public decimal LetBall { get; set; }
        /// <summary>
        /// 胜 平均赔率
        /// </summary>
        [Field("WinOdds")]
        public decimal WinOdds { get; set; }
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
    }
}
