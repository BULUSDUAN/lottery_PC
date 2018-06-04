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
    [Entity("E_Validation_Mobile_Log",Type = EntityType.Table)]
    public class E_Validation_Mobile_Log
    { 
        public E_Validation_Mobile_Log()
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
            // 数据库验证码
            ////</summary>
            [ProtoMember(3)]
            [Field("DB_ValidateCode")]
            public string DB_ValidateCode{ get; set; }
            //// <summary>
            // 用户验证码
            ////</summary>
            [ProtoMember(4)]
            [Field("User_ValidateCode")]
            public string User_ValidateCode{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}