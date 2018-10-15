using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
namespace EntityModel
{
    /// <summary>
    // 澳彩代理
    ///</summary>
    [ProtoContract]
    [Entity("P_OCAgent",Type = EntityType.Table)]
    public class P_OCAgent
    { 
        public P_OCAgent()
        {
        
        }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(1)]
            [Field("UserId", IsIdenty = false, IsPrimaryKey = true)]
            public string UserId{ get; set; }
            /// <summary>
            // 代理类型
            ///</summary>
            [ProtoMember(2)]
            [Field("OCAgentCategory")]
            public int OCAgentCategory{ get; set; }
            /// <summary>
            // 上级用户编号
            ///</summary>
            [ProtoMember(3)]
            [Field("ParentUserId")]
            public string ParentUserId{ get; set; }
            /// <summary>
            // 店面编号
            ///</summary>
            [ProtoMember(4)]
            [Field("StoreId")]
            public string StoreId{ get; set; }
            /// <summary>
            // 父节点路径
            ///</summary>
            [ProtoMember(5)]
            [Field("ParentPath")]
            public string ParentPath{ get; set; }
            /// <summary>
            // 自定义域名
            ///</summary>
            [ProtoMember(6)]
            [Field("CustomerDomain")]
            public string CustomerDomain{ get; set; }
            /// <summary>
            // CPS模式
            ///</summary>
            [ProtoMember(7)]
            [Field("CPSMode")]
            public int CPSMode{ get; set; }
            /// <summary>
            // 渠道名称
            ///</summary>
            [ProtoMember(8)]
            [Field("ChannelName")]
            public string ChannelName{ get; set; }
            /// <summary>
            // 生成时间
            ///</summary>
            [ProtoMember(9)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}