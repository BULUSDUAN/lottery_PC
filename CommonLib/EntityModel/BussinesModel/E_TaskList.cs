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
    // 用户任务赠送记录表
    ///</summary>
    [ProtoContract]
    [Entity("E_TaskList",Type = EntityType.Table)]
    public class E_TaskList
    { 
        public E_TaskList()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 用户Id
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 订单号
            ///</summary>
            [ProtoMember(3)]
            [Field("OrderId")]
            public string OrderId{ get; set; }
            /// <summary>
            // 任务名称
            ///</summary>
            [ProtoMember(4)]
            [Field("TaskName")]
            public string TaskName{ get; set; }
            /// <summary>
            // 内容
            ///</summary>
            [ProtoMember(5)]
            [Field("Content")]
            public string Content{ get; set; }
            /// <summary>
            // 赠送成长值
            ///</summary>
            [ProtoMember(6)]
            [Field("ValueGrowth")]
            public decimal ValueGrowth{ get; set; }
            /// <summary>
            // 是否领取
            ///</summary>
            [ProtoMember(7)]
            [Field("IsGive")]
            public bool IsGive{ get; set; }
            /// <summary>
            // 任务类型
            ///</summary>
            [ProtoMember(8)]
            [Field("TaskCategory")]
            public int TaskCategory{ get; set; }
            /// <summary>
            // VIP活动等级
            ///</summary>
            [ProtoMember(9)]
            [Field("VipLevel")]
            public int VipLevel{ get; set; }
            /// <summary>
            // 创建时间戳
            ///</summary>
            [ProtoMember(10)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}