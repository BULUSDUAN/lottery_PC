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
    // 单式上传的订单信息
    ///</summary>
    [ProtoContract]
    [Entity("T_SingleScheme_Order",Type = EntityType.Table)]
    public class T_SingleScheme_Order
    { 
        public T_SingleScheme_Order()
        {
        
        }
            /// <summary>
            // 订单编号
            ///</summary>
            [ProtoMember(1)]
            [Field("OrderId", IsIdenty = false, IsPrimaryKey = true)]
            public string OrderId{ get; set; }
            /// <summary>
            // 彩种编码
            ///</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 玩法
            ///</summary>
            [ProtoMember(3)]
            [Field("GameType")]
            public string GameType{ get; set; }
            /// <summary>
            // 串关方式
            ///</summary>
            [ProtoMember(4)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            /// <summary>
            // 期号
            ///</summary>
            [ProtoMember(5)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 选择的比赛编号
            ///</summary>
            [ProtoMember(6)]
            [Field("SelectMatchId")]
            public string SelectMatchId{ get; set; }
            /// <summary>
            // 允许投注的号，如胜平负 只能投 3 1 0
            ///</summary>
            [ProtoMember(7)]
            [Field("AllowCodes")]
            public string AllowCodes{ get; set; }
            /// <summary>
            // 是否包括场次编号
            ///</summary>
            [ProtoMember(8)]
            [Field("ContainsMatchId")]
            public bool ContainsMatchId{ get; set; }
            /// <summary>
            // 是否虚拟订单（不用出票到外部）
            ///</summary>
            [ProtoMember(9)]
            [Field("IsVirtualOrder")]
            public bool IsVirtualOrder{ get; set; }
            /// <summary>
            // 倍数
            ///</summary>
            [ProtoMember(10)]
            [Field("Amount")]
            public int Amount{ get; set; }
            /// <summary>
            // 总金额
            ///</summary>
            [ProtoMember(11)]
            [Field("TotalMoney")]
            public decimal TotalMoney{ get; set; }
            /// <summary>
            // 是否文件流
            ///</summary>
            [ProtoMember(12)]
            [Field("FileBuffer")]
            public string FileBuffer{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(13)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}