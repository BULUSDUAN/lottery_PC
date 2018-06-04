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
    // 专家修改资料历史
    ////</summary>
    [ProtoContract]
    [Entity("E_ExperterUpdateHitstroy",Type = EntityType.Table)]
    public class E_ExperterUpdateHitstroy
    { 
        public E_ExperterUpdateHitstroy()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 专家编号
            ////</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 专家描述
            ////</summary>
            [ProtoMember(3)]
            [Field("ExperterSummary")]
            public string ExperterSummary{ get; set; }
            //// <summary>
            // 专家头像
            ////</summary>
            [ProtoMember(4)]
            [Field("ExperterHeadImage")]
            public string ExperterHeadImage{ get; set; }
            //// <summary>
            // 擅长彩种
            ////</summary>
            [ProtoMember(5)]
            [Field("AdeptGameCode")]
            public string AdeptGameCode{ get; set; }
            //// <summary>
            // 处理类别
            ////</summary>
            [ProtoMember(6)]
            [Field("DealWithType")]
            public int? DealWithType{ get; set; }
            //// <summary>
            // 处理意见
            ////</summary>
            [ProtoMember(7)]
            [Field("DisposeOpinion")]
            public string DisposeOpinion{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}