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
    // 
    ////</summary>
    [ProtoContract]
    [Entity("E_A20130808_实名手机认证送3元",Type = EntityType.Table)]
    public class E_A20130808_实名手机认证送3元
    { 
        public E_A20130808_实名手机认证送3元()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户金额
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 真实姓名
            ////</summary>
            [ProtoMember(3)]
            [Field("RealName")]
            public string RealName{ get; set; }
            //// <summary>
            // 手机
            ////</summary>
            [ProtoMember(4)]
            [Field("Mobile")]
            public string Mobile{ get; set; }
            //// <summary>
            // 赠送金额
            ////</summary>
            [ProtoMember(5)]
            [Field("GiveMoney")]
            public decimal? GiveMoney{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(6)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}