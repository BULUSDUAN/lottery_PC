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
    // 
    ///</summary>
    [ProtoContract]
    [Entity("E_A20141203_GiveLottery",Type = EntityType.Table)]
    public class E_A20141203_GiveLottery
    { 
        public E_A20141203_GiveLottery()
        {
        
        }
            /// <summary>
            // 方案编号
            ///</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            /// <summary>
            // 用户编号
            ///</summary>
            [ProtoMember(2)]
            [Field("UserId")]
            public string UserId{ get; set; }
            /// <summary>
            // 投注号码
            ///</summary>
            [ProtoMember(3)]
            [Field("AnteCode")]
            public string AnteCode{ get; set; }
            /// <summary>
            // 活动类型
            ///</summary>
            [ProtoMember(4)]
            [Field("ActivityType")]
            public int ActivityType{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(5)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}