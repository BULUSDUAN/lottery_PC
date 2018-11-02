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
    [Entity("T_JCLQ_Odds_DXF",Type = EntityType.Table)]
    public class T_JCLQ_Odds_DXF : EntityModel.CoreModel.JingCai_Odds
    { 
        public T_JCLQ_Odds_DXF()
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
            [Field("DaOdds")]
            public decimal DaOdds{ get; set; }
            /// <summary>
            // 负 平均赔率
            ///</summary>
            [ProtoMember(4)]
            [Field("XiaoOdds")]
            public decimal XiaoOdds{ get; set; }
            /// <summary>
            // 预设总分
            ///</summary>
            [ProtoMember(5)]
            [Field("YSZF")]
            public decimal YSZF{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public override DateTime CreateTime{ get; set; }

        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return DaOdds;
                case "0":
                    return XiaoOdds;
                case "YSZF":
                    return YSZF;
                default:
                    throw new ArgumentException("获取大小分赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            if (YSZF.ToString("N2").EndsWith(".00"))
            {
                return false;
            }
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            DaOdds = odds.GetOdds("3");
            XiaoOdds = odds.GetOdds("0");
            YSZF = odds.GetOdds("YSZF");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return DaOdds.Equals(odds.GetOdds("3"))
                && XiaoOdds.Equals(odds.GetOdds("0"))
                && YSZF.Equals(odds.GetOdds("YSZF"));
        }
        public override string GetOddsString()
        {
            return "3|" + DaOdds.ToString("F2") + ",0|" + XiaoOdds.ToString("F2") + ",YSZF|" + YSZF.ToString("F2");
        }
    }
}