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
    // 插件类
    ////</summary>
    [ProtoContract]
    [Entity("C_Activity_PluginClass",Type = EntityType.Table)]
    public class C_Activity_PluginClass
    { 
        public C_Activity_PluginClass()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            //// <summary>
            // 类名
            ////</summary>
            [ProtoMember(2)]
            [Field("ClassName")]
            public string ClassName{ get; set; }
            //// <summary>
            // 接口名
            ////</summary>
            [ProtoMember(3)]
            [Field("InterfaceName")]
            public string InterfaceName{ get; set; }
            //// <summary>
            // 组件文件名
            ////</summary>
            [ProtoMember(4)]
            [Field("AssemblyFileName")]
            public string AssemblyFileName{ get; set; }
            //// <summary>
            // 是否启用
            ////</summary>
            [ProtoMember(5)]
            [Field("IsEnable")]
            public bool? IsEnable{ get; set; }
            //// <summary>
            // 排序索引
            ////</summary>
            [ProtoMember(6)]
            [Field("OrderIndex")]
            public int? OrderIndex{ get; set; }
            //// <summary>
            // 开始时间
            ////</summary>
            [ProtoMember(7)]
            [Field("StartTime")]
            public DateTime? StartTime{ get; set; }
            //// <summary>
            // 结束时间
            ////</summary>
            [ProtoMember(8)]
            [Field("EndTime")]
            public DateTime? EndTime{ get; set; }
    }
}