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
    [Entity("E_SMSSendLog_List",Type = EntityType.Table)]
    public class E_SMSSendLog_List
    { 
        public E_SMSSendLog_List()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 线索
            ////</summary>
            [ProtoMember(2)]
            [Field("KeyLine")]
            public string KeyLine{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(3)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 手机号
            ////</summary>
            [ProtoMember(4)]
            [Field("Mobile")]
            public string Mobile{ get; set; }
            //// <summary>
            // 内容
            ////</summary>
            [ProtoMember(5)]
            [Field("Content")]
            public string Content{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(6)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}