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
    // 用户战绩
    ///</summary>
    [ProtoContract]
    [Entity("C_User_Beedings",Type = EntityType.Table)]
    public class C_User_Beedings
    { 
        public C_User_Beedings()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 彩种代码
            ///</summary>
            [ProtoMember(3)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(4)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 金星个数
            ///</summary>
            [ProtoMember(5)]
            [Field("GoldStarCount")]
            public int GoldStarCount{ get; set; }
            /// <summary>
            // 金钻个数
            ///</summary>
            [ProtoMember(6)]
            [Field("GoldDiamondsCount")]
            public int GoldDiamondsCount{ get; set; }
            /// <summary>
            // 金杯个数
            ///</summary>
            [ProtoMember(7)]
            [Field("GoldCupCount")]
            public int GoldCupCount{ get; set; }
            /// <summary>
            // 金冠个数
            ///</summary>
            [ProtoMember(8)]
            [Field("GoldCrownCount")]
            public int GoldCrownCount{ get; set; }
            /// <summary>
            // 银星个数
            ///</summary>
            [ProtoMember(9)]
            [Field("SilverStarCount")]
            public int SilverStarCount{ get; set; }
            /// <summary>
            // 银钻个数
            ///</summary>
            [ProtoMember(10)]
            [Field("SilverDiamondsCount")]
            public int SilverDiamondsCount{ get; set; }
            /// <summary>
            // 银杯个数
            ///</summary>
            [ProtoMember(11)]
            [Field("SilverCupCount")]
            public int SilverCupCount{ get; set; }
            /// <summary>
            // 银冠个数
            ///</summary>
            [ProtoMember(12)]
            [Field("SilverCrownCount")]
            public int SilverCrownCount{ get; set; }
            /// <summary>
            // 被订制跟单人数
            ///</summary>
            [ProtoMember(13)]
            [Field("BeFollowerUserCount")]
            public int BeFollowerUserCount{ get; set; }
            /// <summary>
            // 已被跟单总金额
            ///</summary>
            [ProtoMember(14)]
            [Field("BeFollowedTotalMoney")]
            public decimal BeFollowedTotalMoney{ get; set; }
            /// <summary>
            // 总订单数
            ///</summary>
            [ProtoMember(15)]
            [Field("TotalOrderCount")]
            public int TotalOrderCount{ get; set; }
            /// <summary>
            // 总投注金额
            ///</summary>
            [ProtoMember(16)]
            [Field("TotalBetMoney")]
            public decimal TotalBetMoney{ get; set; }
            /// <summary>
            // 总中奖次数
            ///</summary>
            [ProtoMember(17)]
            [Field("TotalBonusTimes")]
            public int TotalBonusTimes{ get; set; }
            /// <summary>
            // 总中奖金额
            ///</summary>
            [ProtoMember(18)]
            [Field("TotalBonusMoney")]
            public decimal TotalBonusMoney{ get; set; }
            /// <summary>
            // 战绩更新时间
            ///</summary>
            [ProtoMember(19)]
            [Field("UpdateTime")]
            public DateTime UpdateTime{ get; set; }
    }
}