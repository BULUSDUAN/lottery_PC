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
    [Entity("C_User_Balance_Report",Type = EntityType.Table)]
    public class C_User_Balance_Report
    { 
        public C_User_Balance_Report()
        {
        
        }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(1)]
            [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
            public int Id{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(2)]
            [Field("SaveDateTime")]
            public string SaveDateTime{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(3)]
            [Field("TotalFillMoneyBalance")]
            public decimal? TotalFillMoneyBalance{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(4)]
            [Field("TotalBonusBalance")]
            public decimal? TotalBonusBalance{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(5)]
            [Field("TotalCommissionBalance")]
            public decimal? TotalCommissionBalance{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(6)]
            [Field("TotalExpertsBalance")]
            public decimal? TotalExpertsBalance{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(7)]
            [Field("TotalFreezeBalance")]
            public decimal? TotalFreezeBalance{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(8)]
            [Field("TotalRedBagBalance")]
            public decimal? TotalRedBagBalance{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(9)]
            [Field("TotalUserGrowth")]
            public int? TotalUserGrowth{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(10)]
            [Field("TotalDouDou")]
            public int? TotalDouDou{ get; set; }
            /// <summary>
            // 
            ///</summary>
            [ProtoMember(11)]
            [Field("CreateTime")]
            public DateTime? CreateTime{ get; set; }
    }
}