using EntityModel.Interface;
using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 
    ///</summary>
    [ProtoContract]
    [Entity("C_JCLQ_MatchResult",Type = EntityType.Table)]
    public class C_JCLQ_MatchResult: ISportResult
    { 
        public C_JCLQ_MatchResult()
        {
        
        }
            /// <summary>
            // 比赛Id
            ///</summary>
            [ProtoMember(1)]
            [Field("MatchId", IsIdenty = false, IsPrimaryKey = true)]
            public string MatchId{ get; set; }
            /// <summary>
            // 比赛日期
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchData")]
            public string MatchData{ get; set; }
            /// <summary>
            // 比赛编号
            ///</summary>
            [ProtoMember(3)]
            [Field("MatchNumber")]
            public string MatchNumber{ get; set; }
            /// <summary>
            // 主队得分
            ///</summary>
            [ProtoMember(4)]
            [Field("HomeScore")]
            public int HomeScore{ get; set; }
            /// <summary>
            // 客队得分
            ///</summary>
            [ProtoMember(5)]
            [Field("GuestScore")]
            public int GuestScore{ get; set; }
            /// <summary>
            // 比赛状态
            ///</summary>
            [ProtoMember(6)]
            [Field("MatchState")]
            public string MatchState{ get; set; }
            /// <summary>
            // 胜负结果
            ///</summary>
            [ProtoMember(7)]
            [Field("SF_Result")]
            public string SF_Result{ get; set; }
            /// <summary>
            // 胜负 SP
            ///</summary>
            [ProtoMember(8)]
            [Field("SF_SP")]
            public decimal SF_SP{ get; set; }
            /// <summary>
            // 让分胜负结果
            ///</summary>
            [ProtoMember(9)]
            [Field("RFSF_Result")]
            public string RFSF_Result{ get; set; }
            /// <summary>
            // 让分胜负 SP
            ///</summary>
            [ProtoMember(10)]
            [Field("RFSF_SP")]
            public decimal RFSF_SP{ get; set; }
            /// <summary>
            // 胜分差结果
            ///</summary>
            [ProtoMember(11)]
            [Field("SFC_Result")]
            public string SFC_Result{ get; set; }
            /// <summary>
            // 胜分差
            ///</summary>
            [ProtoMember(12)]
            [Field("SFC_SP")]
            public decimal SFC_SP{ get; set; }
            /// <summary>
            // 大小分结果
            ///</summary>
            [ProtoMember(13)]
            [Field("DXF_Result")]
            public string DXF_Result{ get; set; }
            /// <summary>
            // 大小分 SP
            ///</summary>
            [ProtoMember(14)]
            [Field("DXF_SP")]
            public decimal DXF_SP{ get; set; }
            /// <summary>
            // 让分胜负走势 如 让分主胜|-7.5;让分主胜|-8.5;
            ///</summary>
            [ProtoMember(15)]
            [Field("RFSF_Trend")]
            public string RFSF_Trend{ get; set; }
            /// <summary>
            // 大小分走势 如 小|152.5;小|151.5;小|150.5;
            ///</summary>
            [ProtoMember(16)]
            [Field("DXF_Trend")]
            public string DXF_Trend{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(17)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }

        public virtual string GetMatchId(string gameCode)
        {
            return MatchId;
        }
        public virtual string GetFullMatchScore(string gameCode)
        {
            return HomeScore + ":" + GuestScore;
        }
        public virtual string GetMatchResult(string gameCode, string gameType, decimal offset = -1)
        {
            switch (gameType)
            {
                case "SF":
                    return SF_Result;
                case "RFSF":
                    if (offset != -1)
                    {
                        var host1 = (decimal)HomeScore;
                        var guest1 = (decimal)GuestScore;
                        if (host1 + offset > guest1)
                        {
                            return "3";
                        }
                        else if (host1 + offset < guest1)
                        {
                            return "0";
                        }
                    }
                    return RFSF_Result;
                case "SFC":
                    return SFC_Result;
                case "DXF":
                    if (offset != -1)
                    {
                        var host2 = (decimal)HomeScore;
                        var guest2 = (decimal)GuestScore;
                        if (host2 + guest2 > offset)
                        {
                            return "3";
                        }
                        else if (host2 + guest2 < offset)
                        {
                            return "0";
                        }
                    }
                    return DXF_Result;
            }
            return string.Empty;
        }
    }
}