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
    [Entity("blast_bet_orderdetail", Type = EntityType.Table)]
    public class blast_bet_orderdetail
    {
        public blast_bet_orderdetail()
        {

        }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(1)]
        [Field("Id", IsIdenty = true, IsPrimaryKey = true)]
        public int Id { get; set; }

        [Field("SchemeId")]
        public string SchemeId { get; set; }


        [Field("userid")]
        public int userid { get; set; }

        [Field("GameType")]
        public string GameType { get; set; }
        /// <summary>
        /// 投注期号
        /// </summary>
        [Field("issueNo")]
        public string issueNo { get; set; }

        /// <summary>
        /// 投注号
        /// </summary>
        [Field("AnteCode")]
        public string AnteCode { get; set; }
        /// <summary>
        /// 赔率
        /// </summary>
        [Field("Odds")]
        public decimal Odds { get; set; }


        [Field("BonusStatus")]
        public int BonusStatus { get; set; }


        [Field("BonusAwardsMoney")]
        public decimal BonusAwardsMoney { get; set; }


        [Field("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }


        [Field("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }

        [Field("winNumber")]
        public string winNumber { get; set; }


        [Field("ProgressStatus")]
        public int ProgressStatus { get; set; }

        

        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }

    }
}