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
    // 用户积分余额
    ////</summary>
    [ProtoContract]
    [Entity("C_User_IntegralBalance",Type = EntityType.Table)]
    public class C_User_IntegralBalance
    { 
        public C_User_IntegralBalance()
        {
        
        }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            //// <summary>
            // 当前积分余额
            ////</summary>
            [ProtoMember(2)]
            [Field("CurrIntegralBalance")]
            public int? CurrIntegralBalance{ get; set; }
            //// <summary>
            // 用户积分余额
            ////</summary>
            [ProtoMember(3)]
            [Field("UseIntegralBalance")]
            public int? UseIntegralBalance{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}