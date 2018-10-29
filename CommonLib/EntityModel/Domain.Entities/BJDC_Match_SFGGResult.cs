using KaSon.FrameWork.Services.Attribute;
using KaSon.FrameWork.Services.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.Domain.Entities
{
   // [EntityMappingTable("C_BJDC_Match_SFGGResult")]
    [Entity("C_BJDC_Match_SFGGResult", Type = EntityType.Table)]
    public class BJDC_Match_SFGGResult
    {
     

        [Field("Id", IsPrimaryKey =true)]
        public string Id { get; set; }
        [Field("IssuseNumber")]
        public string IssuseNumber { get; set; }
        [Field("MatchOrderId")]
        public int MatchOrderId { get; set; }
        [Field("HomeFull_Result")]
        public string HomeFull_Result { get; set; }
        [Field("GuestFull_Result")]
        public string GuestFull_Result { get; set; }
        [Field("SF_Result")]
        public string SF_Result { get; set; }
        [Field("SF_SP")]
        public decimal SF_SP { get; set; }
        [Field("MatchState")]
        public string MatchState { get; set; }
        [Field("CreateTime")]
        public string CreateTime { get; set; }
    }
}
