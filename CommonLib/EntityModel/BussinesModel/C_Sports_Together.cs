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
    [Entity("C_Sports_Together",Type = EntityType.Table)]
    public class C_Sports_Together
    { 
        public C_Sports_Together()
        {
        
        }
            //// <summary>
            // 方案编号
            ////</summary>
            [ProtoMember(1)]
            [Field("SchemeId", IsIdenty = false, IsPrimaryKey = true)]
            public string SchemeId{ get; set; }
            //// <summary>
            // 标题
            ////</summary>
            [ProtoMember(2)]
            [Field("Title")]
            public string Title{ get; set; }
            //// <summary>
            // 描述
            ////</summary>
            [ProtoMember(3)]
            [Field("Description")]
            public string Description{ get; set; }
            //// <summary>
            // 方案保密性
            ////</summary>
            [ProtoMember(4)]
            [Field("Security")]
            public int? Security{ get; set; }
            //// <summary>
            // 参与密码
            ////</summary>
            [ProtoMember(5)]
            [Field("JoinPwd")]
            public string JoinPwd{ get; set; }
            //// <summary>
            // 总金额
            ////</summary>
            [ProtoMember(6)]
            [Field("TotalMoney")]
            public decimal? TotalMoney{ get; set; }
            //// <summary>
            // 总份数
            ////</summary>
            [ProtoMember(7)]
            [Field("TotalCount")]
            public int? TotalCount{ get; set; }
            //// <summary>
            // 每份单价
            ////</summary>
            [ProtoMember(8)]
            [Field("Price")]
            public decimal? Price{ get; set; }
            //// <summary>
            // 售出份数
            ////</summary>
            [ProtoMember(9)]
            [Field("SoldCount")]
            public int? SoldCount{ get; set; }
            //// <summary>
            // 参与人数
            ////</summary>
            [ProtoMember(10)]
            [Field("JoinUserCount")]
            public int? JoinUserCount{ get; set; }
            //// <summary>
            // 中奖提成 0-10
            ////</summary>
            [ProtoMember(11)]
            [Field("BonusDeduct")]
            public int? BonusDeduct{ get; set; }
            //// <summary>
            // 方案提成
            ////</summary>
            [ProtoMember(12)]
            [Field("SchemeDeduct")]
            public decimal? SchemeDeduct{ get; set; }
            //// <summary>
            // 发起人认购份数
            ////</summary>
            [ProtoMember(13)]
            [Field("Subscription")]
            public int? Subscription{ get; set; }
            //// <summary>
            // 发起人保底份数
            ////</summary>
            [ProtoMember(14)]
            [Field("Guarantees")]
            public int? Guarantees{ get; set; }
            //// <summary>
            // 是否已退还用户保底
            ////</summary>
            [ProtoMember(15)]
            [Field("IsPayBackGuarantees")]
            public bool? IsPayBackGuarantees{ get; set; }
            //// <summary>
            // 系统保底份数
            ////</summary>
            [ProtoMember(16)]
            [Field("SystemGuarantees")]
            public int? SystemGuarantees{ get; set; }
            //// <summary>
            // 方案进度状态
            ////</summary>
            [ProtoMember(17)]
            [Field("ProgressStatus")]
            public int? ProgressStatus{ get; set; }
            //// <summary>
            // 方案进度百分比
            ////</summary>
            [ProtoMember(18)]
            [Field("Progress")]
            public decimal? Progress{ get; set; }
            //// <summary>
            // 是否置顶
            ////</summary>
            [ProtoMember(19)]
            [Field("IsTop")]
            public bool? IsTop{ get; set; }
            //// <summary>
            // 是否已上传号码
            ////</summary>
            [ProtoMember(20)]
            [Field("IsUploadAnteCode")]
            public bool? IsUploadAnteCode{ get; set; }
            //// <summary>
            // 停止时间
            ////</summary>
            [ProtoMember(21)]
            [Field("StopTime")]
            public DateTime? StopTime{ get; set; }
            //// <summary>
            // 彩种代码
            ////</summary>
            [ProtoMember(22)]
            [Field("GameCode")]
            public string GameCode{ get; set; }
            //// <summary>
            // 玩法
            ////</summary>
            [ProtoMember(23)]
            [Field("GameType")]
            public string GameType{ get; set; }
            //// <summary>
            // 串关方式
            ////</summary>
            [ProtoMember(24)]
            [Field("PlayType")]
            public string PlayType{ get; set; }
            //// <summary>
            // 投注场次
            ////</summary>
            [ProtoMember(25)]
            [Field("TotalMatchCount")]
            public int? TotalMatchCount{ get; set; }
            //// <summary>
            // 创建者
            ////</summary>
            [ProtoMember(26)]
            [Field("CreateUserId")]
            public string CreateUserId{ get; set; }
            //// <summary>
            // 代理商编号
            ////</summary>
            [ProtoMember(27)]
            [Field("AgentId")]
            public string AgentId{ get; set; }
            //// <summary>
            // 方案来源
            ////</summary>
            [ProtoMember(28)]
            [Field("SchemeSource")]
            public int? SchemeSource{ get; set; }
            //// <summary>
            // 创建时间或期号
            ////</summary>
            [ProtoMember(29)]
            [Field("CreateTimeOrIssuseNumber")]
            public string CreateTimeOrIssuseNumber{ get; set; }
            //// <summary>
            // 方案投注类别
            ////</summary>
            [ProtoMember(30)]
            [Field("SchemeBettingCategory")]
            public int? SchemeBettingCategory{ get; set; }
            //// <summary>
            // 创建时间
            ////</summary>
            [ProtoMember(31)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}