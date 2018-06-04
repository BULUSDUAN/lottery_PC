using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    //// <summary>
    // 用户资金冻结明细
    ////</summary>
    [ProtoContract]
    [Entity("C_User_Balance_FreezeList",Type = EntityType.Table)]
    public class C_User_Balance_FreezeList
    { 
        public C_User_Balance_FreezeList()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 订单编号
            ////</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            //// <summary>
            // 冻结金额
            ////</summary>
            [ProtoMember(4)]
            [Field("FreezeMoney")]
            public decimal? FreezeMoney{ get; set; }
            //// <summary>
            // 类型
            ////</summary>
            [ProtoMember(5)]
            [Field("Category")]
            public string Category{ get; set; }
            //// <summary>
            // 描述
            ////</summary>
            [ProtoMember(6)]
            [Field("Description")]
            public string Description{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(7)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}