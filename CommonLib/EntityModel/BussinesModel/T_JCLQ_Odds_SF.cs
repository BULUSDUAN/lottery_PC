using EntityModel.CoreModel;
using EntityModel.Ticket;
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
    [Entity("T_JCLQ_Odds_SF",Type = EntityType.Table)]
    public class T_JCLQ_Odds_SF: EntityModel.CoreModel.JingCai_Odds
    { 
        public T_JCLQ_Odds_SF()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public override long Id{ get; set; }
            /// <summary>
            // 比赛Id
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchId")]
            public override string MatchId{ get; set; }
            /// <summary>
            // 胜 平均赔率
            ///</summary>
            [ProtoMember(3)]
            [Field("WinOdds")]
            public decimal WinOdds{ get; set; }
            /// <summary>
            // 负 平均赔率
            ///</summary>
            [ProtoMember(4)]
            [Field("LoseOdds")]
            public decimal LoseOdds{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public override DateTime CreateTime{ get; set; }
        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinOdds;
                case "0":
                    return LoseOdds;
                default:
                    throw new ArgumentException("获取胜负赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            WinOdds = odds.GetOdds("3");
            LoseOdds = odds.GetOdds("0");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return WinOdds.Equals(odds.GetOdds("3"))
                && LoseOdds.Equals(odds.GetOdds("0"));
        }
        public override string GetOddsString()
        {
            return "3|" + WinOdds.ToString("F2") + ",0|" + LoseOdds.ToString("F2");
        }
    }
}