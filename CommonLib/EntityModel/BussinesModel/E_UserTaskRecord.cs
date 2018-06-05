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
    // 用户任务完成记录表
    ///</summary>
    [ProtoContract]
    [Entity("E_UserTaskRecord",Type = EntityType.Table)]
    public class E_UserTaskRecord
    { 
        public E_UserTaskRecord()
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
            // 订单金额
            ///</summary>
            [ProtoMember(4)]
            [Field("OrderMoney")]
            public decimal OrderMoney{ get; set; }
            /// <summary>
            // 任务名称
            ///</summary>
            [ProtoMember(5)]
            [Field("TaskName")]
            public string TaskName{ get; set; }
            /// <summary>
            // 任务类型
            ///</summary>
            [ProtoMember(6)]
            [Field("TaskCategory")]
            public int TaskCategory{ get; set; }
            /// <summary>
            // 创建时间戳
            ///</summary>
            [ProtoMember(7)]
            [Field("CurrentTime")]
            public string CurrentTime{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(8)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}