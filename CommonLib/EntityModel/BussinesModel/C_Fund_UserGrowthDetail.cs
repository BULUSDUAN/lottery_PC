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
    // 用户成长值明细
    ///</summary>
    [ProtoContract]
    [Entity("C_Fund_UserGrowthDetail",Type = EntityType.Table)]
    public class C_Fund_UserGrowthDetail
    { 
        public C_Fund_UserGrowthDetail()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 订单号
            ///</summary>
            [ProtoMember(2)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 收支类型
            ///</summary>
            [ProtoMember(4)]
            [Field("PayType")]
            public int PayType{ get; set; }
            /// <summary>
            // 分类编号
            ///</summary>
            [ProtoMember(5)]
            [Field("Category")]
            public string Category{ get; set; }
            /// <summary>
            // 描述
            ///</summary>
            [ProtoMember(6)]
            [Field("Summary")]
            public string Summary{ get; set; }
            /// <summary>
            // 收入金额
            ///</summary>
            [ProtoMember(7)]
            [Field("PayMoney")]
            public int PayMoney{ get; set; }
            /// <summary>
            // 交易前余额
            ///</summary>
            [ProtoMember(8)]
            [Field("BeforeBalance")]
            public int BeforeBalance{ get; set; }
            /// <summary>
            // 交易后余额
            ///</summary>
            [ProtoMember(9)]
            [Field("AfterBalance")]
            public int AfterBalance{ get; set; }
            /// <summary>
            // 创建日期
            ///</summary>
            [ProtoMember(10)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}