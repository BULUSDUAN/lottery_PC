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
        [Field("id", IsIdenty = true, IsPrimaryKey = true)]
        public int id { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(2)]
        [Field("SchemeId")]
        public string SchemeId { get; set; }

        [Field("userId")]
        public string userId { get; set; }


        /// <summary>
        // 
        ///</summary>
        [ProtoMember(4)]
        [Field("GameType")]
        public string GameType { get; set; }
        /// <summary>
        // 期号
        ///</summary>
        [ProtoMember(5)]
        [Field("playId")]
        public int playId { get; set; }
        /// <summary>
        // 期号
        ///</summary>
        [ProtoMember(6)]
        [Field("issueNo")]
        public string issueNo { get; set; }

        [Field("anteCodeNum")]
        public int anteCodeNum { get; set; }
        [Field("BeiSu")]
        public int BeiSu { get; set; } = 1;

        /// <summary>
        // 投注号
        ///</summary>
      
        [Field("AnteCodes")]
        public string AnteCodes { get; set; }
        /// <summary>
        // 赔率
        ///</summary>
        [ProtoMember(8)]
        [Field("Odds")]
        public decimal Odds { get; set; }
        /// <summary>
        // BonusStatus 改为2中奖,3不中奖为期号过,4和局
        ///</summary>
        [ProtoMember(9)]
        [Field("BonusStatus")]
        public int BonusStatus { get; set; }

        /// <summary>
        // 中奖金额
        ///</summary>
        [ProtoMember(11)]
        [Field("BonusAwardsMoney")]
        public decimal BonusAwardsMoney { get; set; }
        /// <summary>
        // 税前奖金
        ///</summary>
        [ProtoMember(12)]
        [Field("PreTaxBonusMoney")]
        public decimal PreTaxBonusMoney { get; set; }

        /// <summary>
        // 税后奖金
        ///</summary>
        [ProtoMember(14)]
        [Field("AfterTaxBonusMoney")]
        public decimal AfterTaxBonusMoney { get; set; }
        /// <summary>
        // 开奖号
        ///</summary>
        [ProtoMember(15)]
        [Field("winNumber")]
        public string winNumber { get; set; }

        [Field("winNumberDesc")]
        public string winNumberDesc { get; set; }
        /// <summary>
        // 进行状态
        ///</summary>
        [ProtoMember(16)]
        [Field("ProgressStatus")]
        public int ProgressStatus { get; set; }
        /// <summary>
        // 
        ///</summary>
        [ProtoMember(17)]
        [Field("CreateTime")]
        public DateTime CreateTime { get; set; }


        [Field("updateTime")]
        public DateTime updateTime { get; set; }
        /// <summary>
        /// 投注单价
        /// </summary>
        [Field("unitPrice")]
        public decimal unitPrice { get; set; }

        [Field("issueDate")]
        public string issueDate { get; set; }

  

        /// <summary>
        /// 每个码对应的赔率
        /// </summary>
        public string OddsArr { get; set; }

    }
}