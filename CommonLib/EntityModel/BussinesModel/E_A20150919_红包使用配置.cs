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
    [Entity("E_A20150919_红包使用配置",Type = EntityType.Table)]
    public class E_A20150919_红包使用配置
    { 
        public E_A20150919_红包使用配置()
        {
        
        }
            /// <summary>
            // 主键
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 彩种代码
            ///</summary>
            [ProtoMember(2)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            /// <summary>
            // 使用比率
            ///</summary>
            [ProtoMember(3)]
            [Field("UsePercent")]
            public decimal UsePercent{ get; set; }
            /// <summary>
            // 创建时间
            ///</summary>
            [ProtoMember(4)]
            [Field("CreateTime")]
            public DateTime CreateTime{ get; set; }
    }
}