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
    // 用户积分明细
    ////</summary>
    [ProtoContract]
    [Entity("C_User_IntegralDetail",Type = EntityType.Table)]
    public class C_User_IntegralDetail
    { 
        public C_User_IntegralDetail()
        {
        
        }
            //// <summary>
            // 积分明细编号
            ////</summary>
            [ProtoMember(1)]
            [Field("IntegralDetailId", IsIdenty = true, IsPrimaryKey = true)]
            public int IntegralDetailId{ get; set; }
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
            // 备注
            ////</summary>
            [ProtoMember(4)]
            [Field("Summary")]
            public string Summary{ get; set; }
            //// <summary>
            // 积分
            ////</summary>
            [ProtoMember(5)]
            [Field("PayIntegral")]
            public int? PayIntegral{ get; set; }
            //// <summary>
            // 旧积分
            ////</summary>
            [ProtoMember(6)]
            [Field("BeforeIntegral")]
            public int? BeforeIntegral{ get; set; }
            //// <summary>
            // 新积分
            ////</summary>
            [ProtoMember(7)]
            [Field("AfterIntegral")]
            public int? AfterIntegral{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}