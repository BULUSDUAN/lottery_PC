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
    [Entity("P_RebateDetail",Type = EntityType.Table)]
    public class P_RebateDetail
    { 
        public P_RebateDetail()
        {
        
        }
            //// <summary>
            // 主键
            ////</summary>
            [ProtoMember(1)]
            [Field("ID", IsIdenty = true, IsPrimaryKey = true)]
            public int ID{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(2)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(3)]
            [Field("SchemeId")]
            public string SchemeId{ get; set; }
            //// <summary>
            // 方案类型
            ////</summary>
            [ProtoMember(4)]
            [Field("SchemeType")]
            public int? SchemeType{ get; set; }
            //// <summary>
            // 用户编号
            ////</summary>
            [ProtoMember(5)]
            [Field("UserId")]
            public string UserId{ get; set; }
            //// <summary>
            // 彩种编码
            ////</summary>
            [ProtoMember(6)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(7)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 返利
            ////</summary>
            [ProtoMember(8)]
            [Field("Point")]
            public decimal? Point{ get; set; }
            //// <summary>
            // 返利金额
            ////</summary>
            [ProtoMember(9)]
            [Field("PayinMoney")]
            public decimal? PayinMoney{ get; set; }
            //// <summary>
            // 备注
            ////</summary>
            [ProtoMember(10)]
            [Field("Remark")]
            public string Remark{ get; set; }
    }
}