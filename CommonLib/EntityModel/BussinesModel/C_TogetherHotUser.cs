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
    // 合买红人表
    ////</summary>
    [ProtoContract]
    [Entity("C_TogetherHotUser",Type = EntityType.Table)]
    public class C_TogetherHotUser
    { 
        public C_TogetherHotUser()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 用户ID
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 近期本周中奖
            ////</summary>
            [ProtoMember(3)]
            [Field("WeeksWinMoney")]
            public decimal? WeeksWinMoney{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}