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
    [Entity("T_JCLQ_Odds_RFSF",Type = EntityType.Table)]
    public class T_JCLQ_Odds_RFSF: JingCai_Odds
    { 
        public T_JCLQ_Odds_RFSF()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 比赛Id
            ///</summary>
            [ProtoMember(2)]
            [Field("MatchId")]
            public string MatchId{ get; set; }
            /// <summary>
            // 胜 平均赔率
            ///</summary>
            [ProtoMember(3)]
            [Field("WinOdds")]
            public decimal WinOdds{ get; set; }
            /// <summary>
            //  负 平均赔率
            ///</summary>
            [ProtoMember(4)]
            [Field("LoseOdds")]
            public decimal LoseOdds{ get; set; }
            /// <summary>
            // 让分数
            ///</summary>
            [ProtoMember(5)]
            [Field("RF")]
            public decimal RF{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }


        public override decimal GetOdds(string result)
        {
            switch (result)
            {
                case "3":
                    return WinOdds;
                case "0":
                    return LoseOdds;
                case "RF":
                    return RF;
                default:
                    throw new ArgumentException("获取让分胜负赔率不支持的结果数据 - " + result);
            }
        }
        public override bool CheckIsValidate()
        {
            if (RF.ToString("N2").EndsWith(".00"))
            {
                return false;
            }
            return true;
        }
        public override void SetOdds(I_JingCai_Odds odds)
        {
            WinOdds = odds.GetOdds("3");
            LoseOdds = odds.GetOdds("0");
            RF = odds.GetOdds("RF");
        }
        public override bool Equals(I_JingCai_Odds odds)
        {
            return WinOdds.Equals(odds.GetOdds("3"))
                && LoseOdds.Equals(odds.GetOdds("0"))
                && RF.Equals(odds.GetOdds("RF"));
        }
        public override string GetOddsString()
        {
            return "3|" + WinOdds.ToString("F2") + ",0|" + LoseOdds.ToString("F2") + ",RF|" + RF.ToString("F2");
        }
    }
}