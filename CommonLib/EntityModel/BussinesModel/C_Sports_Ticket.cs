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
    // 体彩票据
    ///</summary>
    [ProtoContract]
    [Entity("C_Sports_Ticket",Type = EntityType.Table)]
    public class C_Sports_Ticket
    { 
        public C_Sports_Ticket()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 方案编号
            ///</summary>
            [ProtoMember(2)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            /// <summary>
            // 彩票号
            ///</summary>
            [ProtoMember(3)]
            [Field("TicketId")]
            public string TicketId{ get; set; }
            /// <summary>
            // 彩种代码
            ///</summary>
            [ProtoMember(4)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(5)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 串关方式
            ///</summary>
            [ProtoMember(6)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            /// <summary>
            // 比赛编号列表
            ///</summary>
            [ProtoMember(7)]
            [Field("MatchIdList")]
            public string MatchIdList{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(8)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 注数
            ///</summary>
            [ProtoMember(9)]
            [Field("BetUnits")]
            public int BetUnits{ get; set; }
            /// <summary>
            // 倍数
            ///</summary>
            [ProtoMember(10)]
            [Field("Amount")]
            public int Amount{ get; set; }
            /// <summary>
            // 票金额
            ///</summary>
            [ProtoMember(11)]
            [Field("BetMoney")]
            public decimal BetMoney{ get; set; }
            /// <summary>
            // 票内容
            ///</summary>
            [ProtoMember(12)]
            [Field("BetContent")]
            public string BetContent{ get; set; }
            /// <summary>
            // 赔率
            ///</summary>
            [ProtoMember(13)]
            [Field("LocOdds")]
            public string LocOdds{ get; set; }
            /// <summary>
            // 出票状态
            ///</summary>
            [ProtoMember(14)]
            [Field("TicketStatus")]
            public int TicketStatus{ get; set; }
            /// <summary>
            // 出票记录
            ///</summary>
            [ProtoMember(15)]
            [Field("TicketLog")]
            public string TicketLog{ get; set; }
            /// <summary>
            // 合买伙伴编号
            ///</summary>
            [ProtoMember(16)]
            [Field("PartnerId")]
            public string PartnerId{ get; set; }
            /// <summary>
            // 彩票平台序号
            ///</summary>
            [ProtoMember(17)]
            [Field("Palmid")]
            public string Palmid{ get; set; }
            /// <summary>
            // 票号1
            ///</summary>
            [ProtoMember(18)]
            [Field("PrintNumber1")]
            public string PrintNumber1{ get; set; }
            /// <summary>
            // 票号2
            ///</summary>
            [ProtoMember(19)]
            [Field("PrintNumber2")]
            public string PrintNumber2{ get; set; }
            /// <summary>
            // 票号3
            ///</summary>
            [ProtoMember(20)]
            [Field("PrintNumber3")]
            public string PrintNumber3{ get; set; }
            /// <summary>
            // 条形码
            ///</summary>
            [ProtoMember(21)]
            [Field("BarCode")]
            public string BarCode{ get; set; }
            /// <summary>
            // 打印赔率信息
            ///</summary>
            [ProtoMember(22)]
            [Field("PrintOdd")]
            public string PrintOdd{ get; set; }
            /// <summary>
            // 打印没有赔率信息
            ///</summary>
            [ProtoMember(23)]
            [Field("PrintUnOdd")]
            public string PrintUnOdd{ get; set; }
            /// <summary>
            // 状态
            ///</summary>
            [ProtoMember(24)]
            [Field("BonusStatus")]
            public int BonusStatus{ get; set; }
            /// <summary>
            // 税前奖金
            ///</summary>
            [ProtoMember(25)]
            [Field("PreTaxBonusMoney")]
            public decimal PreTaxBonusMoney{ get; set; }
            /// <summary>
            // 税后奖金
            ///</summary>
            [ProtoMember(26)]
            [Field("AfterTaxBonusMoney")]
            public decimal AfterTaxBonusMoney{ get; set; }
            /// <summary>
            // 打印时间
            ///</summary>
            [ProtoMember(27)]
            [Field("PrintDateTime")]
            public DateTime? PrintDateTime{ get; set; }
            /// <summary>
            // 出票接口
            ///</summary>
            [ProtoMember(28)]
            [Field("Gateway")]
            public string Gateway{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(29)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
            /// <summary>
            // 是否追加
            ///</summary>
            [ProtoMember(30)]
            [Field("IsAppend")]
            public bool IsAppend{ get; set; }
            /// <summary>
            // 中奖时间
            ///</summary>
            [ProtoMember(31)]
            [Field("PrizeDateTime")]
            public DateTime? PrizeDateTime{ get; set; }
    }
}