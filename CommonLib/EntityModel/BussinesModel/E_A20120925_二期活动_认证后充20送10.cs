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
    [Entity("E_A20120925_二期活动_认证后充20送10",Type = EntityType.Table)]
    public class E_A20120925_二期活动_认证后充20送10
    { 
        public E_A20120925_二期活动_认证后充20送10()
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
            // 是否完成手机认证
            ////</summary>
            [ProtoMember(3)]
            [Field("IsComlateMobile")]
            public bool? IsComlateMobile{ get; set; }
            //// <summary>
            // 是否完成实名认证
            ////</summary>
            [ProtoMember(4)]
            [Field("IsComlateRealName")]
            public bool? IsComlateRealName{ get; set; }
            //// <summary>
            // 是否赠送金额
            ////</summary>
            [ProtoMember(5)]
            [Field("IsGiveMoney")]
            public bool? IsGiveMoney{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(6)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}