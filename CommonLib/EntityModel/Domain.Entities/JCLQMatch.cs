using EntityModel.Interface;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
    [Entity("C_JCLQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_Match_HH : IBallBaseInfo
    {
        [BsonId]
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        [Field("AverageWin")]
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        [Field("AverageLose")]
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public string RFSF { get; set; }
        public string SF { get; set; }
        public string DXF { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_Match_HH;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.AverageWin != this.AverageWin
                || t.AverageLose != this.AverageLose
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeTeamId != this.HomeTeamId
                || t.GuestTeamId != this.GuestTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamName != this.GuestTeamName
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }

    [Entity("C_JCLQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_Match_HHDG : IBallBaseInfo
    {
        [BsonId]
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        [Field("AverageWin")]
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        [Field("AverageLose")]
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public string RFSF { get; set; }
        public string SF { get; set; }
        public string DXF { get; set; }
        public string SFC { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_Match_HHDG;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.AverageWin != this.AverageWin
                || t.AverageLose != this.AverageLose
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeTeamId != this.HomeTeamId
                || t.GuestTeamId != this.GuestTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamName != this.GuestTeamName
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球队伍
    /// </summary>
    [Entity("C_JCLQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_Match_SF : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        [Field("AverageWin")]
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        [Field("AverageLose")]
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public string SF { get; set; }
        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_Match_SF;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.AverageWin != this.AverageWin
                || t.AverageLose != this.AverageLose
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeTeamId != this.HomeTeamId
                || t.GuestTeamId != this.GuestTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamName != this.GuestTeamName
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球队伍
    /// </summary>
    [Entity("C_JCLQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_Match_RFSF : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        [Field("AverageWin")]
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        [Field("AverageLose")]
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public string RFSF { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_Match_RFSF;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.AverageWin != this.AverageWin
                || t.AverageLose != this.AverageLose
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeTeamId != this.HomeTeamId
                || t.GuestTeamId != this.GuestTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamName != this.GuestTeamName
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球队伍
    /// </summary>
    [Entity("C_JCLQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_Match_SFC : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        [Field("AverageWin")]
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        [Field("AverageLose")]
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public string SFC { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_Match_SFC;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.AverageWin != this.AverageWin
                || t.AverageLose != this.AverageLose
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeTeamId != this.HomeTeamId
                || t.GuestTeamId != this.GuestTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamName != this.GuestTeamName
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球队伍
    /// </summary>
    [Entity("C_JCLQ_Match", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_Match_DXF : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 比赛编号名称
        /// </summary>
        [Field("MatchIdName")]
        public string MatchIdName { get; set; }
        /// <summary>
        /// 数据中心Id
        /// </summary>
        [Field("Mid")]
        public int Mid { get; set; }
        //[Field("FXId")]
        public int FXId { get; set; }
        /// <summary>
        /// 联赛Id : 140
        /// </summary>
        [Field("LeagueId")]
        public int LeagueId { get; set; }
        public string State { get; set; }
        /// <summary>
        /// 联赛名称
        /// </summary>
        [Field("LeagueName")]
        public string LeagueName { get; set; }
        /// <summary>
        /// 联赛颜色
        /// </summary>
        [Field("LeagueColor")]
        public string LeagueColor { get; set; }
        /// <summary>
        /// 主队编号
        /// </summary>
        [Field("HomeTeamId")]
        public int HomeTeamId { get; set; }
        /// <summary>
        /// 主队名称
        /// </summary>
        [Field("HomeTeamName")]
        public string HomeTeamName { get; set; }
        /// <summary>
        /// 客队编号
        /// </summary>
        [Field("GuestTeamId")]
        public int GuestTeamId { get; set; }
        /// <summary>
        /// 客队名称
        /// </summary>
        [Field("GuestTeamName")]
        public string GuestTeamName { get; set; }
        /// <summary>
        /// 状态
        /// </summary>
        [Field("MatchState")]
        public int MatchState { get; set; }
        /// <summary>
        /// 平均欧指 - 胜
        /// </summary>
        [Field("AverageWin")]
        public decimal AverageWin { get; set; }
        /// <summary>
        /// 平均欧指 - 负
        /// </summary>
        [Field("AverageLose")]
        public decimal AverageLose { get; set; }
        /// <summary>
        /// 比赛开始时间
        /// </summary>
        [Field("StartDateTime")]
        public string StartDateTime { get; set; }
        /// <summary>
        /// 单式停止投注时间
        /// </summary>
        [Field("DSStopBettingTime")]
        public string DSStopBettingTime { get; set; }
        /// <summary>
        /// 复式停止投注时间
        /// </summary>
        [Field("FSStopBettingTime")]
        public string FSStopBettingTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public string DXF { get; set; }

        /// <summary>
        /// 限玩法列表
        /// </summary>
        public string PrivilegesType { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_Match_DXF;
            if (t.StartDateTime != this.StartDateTime
                || t.DSStopBettingTime != this.DSStopBettingTime
                || t.FSStopBettingTime != this.FSStopBettingTime
                || t.AverageWin != this.AverageWin
                || t.AverageLose != this.AverageLose
                || t.LeagueColor != this.LeagueColor
                || t.Mid != this.Mid
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeTeamId != this.HomeTeamId
                || t.GuestTeamId != this.GuestTeamId
                || t.HomeTeamName != this.HomeTeamName
                || t.GuestTeamName != this.GuestTeamName
                || t.PrivilegesType != this.PrivilegesType
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球 胜负 SP
    /// </summary>
    [Entity("C_JCLQ_SF_SP", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_SF_SP : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        [Field("WinSP")]
        public decimal WinSP { get; set; }
        [Field("LoseSP")]
        public decimal LoseSP { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_SF_SP;
            if (t.WinSP != this.WinSP
                || t.LoseSP != this.LoseSP
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球 让球胜负 SP
    /// </summary>
    [Entity("C_JCLQ_RFSF_SP", Type = EntityType.Table)]
    public class JCLQ_RFSF_SP : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        [Field("WinSP")]
        public decimal WinSP { get; set; }
        [Field("LoseSP")]
        public decimal LoseSP { get; set; }
        /// <summary>
        /// 让分
        /// </summary>
        [Field("RF")]
        public decimal RF { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_RFSF_SP;
            if (t.WinSP != this.WinSP
                || t.LoseSP != this.LoseSP
                || t.RF != this.RF
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球 胜分差 SP
    /// </summary>
    [Entity("C_JCLQ_SFC_SP", Type = EntityType.Table)]
    public class JCLQ_SFC_SP : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }

        [Field("HomeWin1_5")]
        public decimal HomeWin1_5 { get; set; }
        [Field("HomeWin6_10")]
        public decimal HomeWin6_10 { get; set; }
        [Field("HomeWin11_15")]
        public decimal HomeWin11_15 { get; set; }
        [Field("HomeWin16_20")]
        public decimal HomeWin16_20 { get; set; }
        [Field("HomeWin21_25")]
        public decimal HomeWin21_25 { get; set; }
        [Field("HomeWin26")]
        public decimal HomeWin26 { get; set; }

        [Field("GuestWin1_5")]
        public decimal GuestWin1_5 { get; set; }
        [Field("GuestWin6_10")]
        public decimal GuestWin6_10 { get; set; }
        [Field("GuestWin11_15")]
        public decimal GuestWin11_15 { get; set; }
        [Field("GuestWin16_20")]
        public decimal GuestWin16_20 { get; set; }
        [Field("GuestWin21_25")]
        public decimal GuestWin21_25 { get; set; }
        [Field("GuestWin26")]
        public decimal GuestWin26 { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_SFC_SP;
            if (t.HomeWin1_5 != this.HomeWin1_5
                || t.HomeWin6_10 != this.HomeWin6_10
                || t.HomeWin11_15 != this.HomeWin11_15
                || t.HomeWin16_20 != this.HomeWin16_20
                || t.HomeWin21_25 != this.HomeWin21_25
                || t.HomeWin26 != this.HomeWin26
                || t.GuestWin1_5 != this.GuestWin1_5
                || t.GuestWin6_10 != this.GuestWin6_10
                || t.GuestWin11_15 != this.GuestWin11_15
                || t.GuestWin16_20 != this.GuestWin16_20
                || t.GuestWin21_25 != this.GuestWin21_25
                || t.GuestWin26 != this.GuestWin26
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球 大小分 SP
    /// </summary>
    [Entity("C_JCLQ_DXF_SP", Type = EntityType.Table)]
    public class JCLQ_DXF_SP : IBallBaseInfo
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }

        /// <summary>
        /// 预设总分
        /// </summary>
        [Field("YSZF")]
        public decimal YSZF { get; set; }
        /// <summary>
        /// 大分
        /// </summary>
        [Field("DF")]
        public decimal DF { get; set; }
        /// <summary>
        /// 小分
        /// </summary>
        [Field("XF")]
        public decimal XF { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_DXF_SP;
            if (t.YSZF != this.YSZF
                || t.DF != this.DF
                || t.XF != this.XF
                || t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                )
                return false;
            return true;
        }
    }

    /// <summary>
    /// 竞彩篮球 比赛结果
    /// </summary>
    [Entity("C_JCLQ_MatchResult", Type = EntityType.Table)]
    [BsonIgnoreExtraElements]
    public class JCLQ_MatchResult : IBallBaseInfo
    {
        [BsonId]
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
        [Field("Id",    IsPrimaryKey =true, IsIdenty =true)]
        public long mId { get; set; }
        /// <summary>
        /// 主队得分
        /// </summary>
        [Field("HomeScore")]
        public int HomeScore { get; set; }
        /// <summary>
        /// 客队得分
        /// </summary>
        [Field("GuestScore")]
        public int GuestScore { get; set; }
        [Field("MatchState")]
        public string MatchState { get; set; }
        /// <summary>
        /// 胜负结果
        /// </summary>
        [Field("SF_Result")]
        public string SF_Result { get; set; }
        /// <summary>
        /// 胜负 SP
        /// </summary>
        [Field("SF_SP")]
        public decimal SF_SP { get; set; }
        /// <summary>
        /// 让分胜负结果
        /// </summary>
        [Field("RFSF_Result")]
        public string RFSF_Result { get; set; }
        /// <summary>
        /// 让分胜负 SP
        /// </summary>
        [Field("RFSF_SP")]
        public decimal RFSF_SP { get; set; }
        /// <summary>
        /// 胜分差结果
        /// </summary>
        [Field("SFC_Result")]
        public string SFC_Result { get; set; }
        /// <summary>
        /// 胜分差 SP
        /// </summary>
        [Field("SFC_SP")]
        public decimal SFC_SP { get; set; }
        /// <summary>
        /// 大小分结果
        /// </summary>
        [Field("DXF_Result")]
        public string DXF_Result { get; set; }
        /// <summary>
        /// 大小分 SP
        /// </summary>
        [Field("DXF_SP")]
        public decimal DXF_SP { get; set; }
        /// <summary>
        /// 让分胜负走势  如 让分主胜|-7.5;让分主胜|-8.5;
        /// </summary>
        [Field("RFSF_Trend")]
        public string RFSF_Trend { get; set; }
        /// <summary>
        /// 大小分走势   如 小|152.5;小|151.5;小|150.5;
        /// </summary>
        [Field("DXF_Trend")]
        public string DXF_Trend { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        [Field("CreateTime")]
        public string CreateTime { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as JCLQ_MatchResult;
            if (t.MatchId != this.MatchId
                || t.MatchData != this.MatchData
                || t.MatchNumber != this.MatchNumber
                || t.HomeScore != this.HomeScore
                || t.GuestScore != this.GuestScore
                || t.SF_Result != this.SF_Result
                || t.SF_SP != this.SF_SP
                || t.RFSF_Result != this.RFSF_Result
                || t.RFSF_SP != this.RFSF_SP
                || t.SFC_Result != this.SFC_Result
                || t.SFC_SP != this.SFC_SP
                || t.DXF_Result != this.DXF_Result
                || t.DXF_SP != this.DXF_SP
                || t.RFSF_Trend != this.RFSF_Trend
                || t.DXF_Trend != this.DXF_Trend
                || t.MatchState != this.MatchState
                )
                return false;
            return true;
        }

    }
}
