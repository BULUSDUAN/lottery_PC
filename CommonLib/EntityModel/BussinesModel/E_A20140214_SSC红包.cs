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
    [Entity("E_A20140214_SSC红包",Type = EntityType.Table)]
    public class E_A20140214_SSC红包
    { 
        public E_A20140214_SSC红包()
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
            // 方案编号
            ///</summary>
            [ProtoMember(3)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
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
            // 期号
            ///</summary>
            [ProtoMember(6)]
            [Field("IssuseNumber")]
            public string IssuseNumber{ get; set; }
            /// <summary>
            // 订单金额
            ///</summary>
            [ProtoMember(7)]
            [Field("OrderMoney")]
            public decimal OrderMoney{ get; set; }
            /// <summary>
            // 税后奖金
            ///</summary>
            [ProtoMember(8)]
            [Field("afterTaxBonusMoney")]
            public decimal afterTaxBonusMoney{ get; set; }
            /// <summary>
            // 加奖金额
            ///</summary>
            [ProtoMember(9)]
            [Field("AddMoney")]
            public decimal AddMoney{ get; set; }
            /// <summary>
            // 内容
            ///</summary>
            [ProtoMember(10)]
            [Field("Content")]
            public string Content{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}