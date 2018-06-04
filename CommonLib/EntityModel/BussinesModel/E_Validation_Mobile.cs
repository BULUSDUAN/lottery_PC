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
    [Entity("E_Validation_Mobile",Type = EntityType.Table)]
    public class E_Validation_Mobile
    { 
        public E_Validation_Mobile()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 手机号码
            ////</summary>
            [ProtoMember(2)]
            [Field("Mobile")]
            public string Mobile{ get; set; }
            //// <summary>
            // 类别
            ////</summary>
            [ProtoMember(3)]
            [Field("Category")]
            public string Category{ get; set; }
            //// <summary>
            // 验证码
            ////</summary>
            [ProtoMember(4)]
            [Field("ValidateCode")]
            public string ValidateCode{ get; set; }
            //// <summary>
            // 发送时间
            ////</summary>
            [ProtoMember(5)]
            [Field("SendTimes")]
            public int? SendTimes{ get; set; }
            //// <summary>
            // 重试时间
            ////</summary>
            [ProtoMember(6)]
            [Field("RetryTimes")]
            public int? RetryTimes{ get; set; }
            //// <summary>
            // 更新时间
            ////</summary>
            [ProtoMember(7)]
            [Field("UpdateTime")]
            public DateTime? UpdateTime{ get; set; }
    }
}