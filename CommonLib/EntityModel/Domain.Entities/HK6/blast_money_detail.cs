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
    [Entity("blast_money_detail", Type = EntityType.Table)]
    public class blast_money_detail
    {
        public blast_money_detail()
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
        [Field("userId")]
        public string userId { get; set; }

        [Field("user_diaplayName")]
        public string user_diaplayName { get; set; }

        [Field("orderId")]
        public string orderId { get; set; }


        [Field("moneyType")]
        public int moneyType { get; set; }


        [Field("remark")]
        public string remark { get; set; }


        [Field("isAuto")]
        public int isAuto { get; set; }

        [Field("totalMoney")]
        public decimal totalMoney { get; set; }

        [Field("beforeMoney")]
        public decimal beforeMoney { get; set; }
        [Field("afterMoney")]
        public decimal afterMoney { get; set; }

        [Field("category")]
        public string category { get; set; }
        

        [Field("update_time")]
        public DateTime update_time { get; set; }



        [Field("create_time")]
        public DateTime create_time { get; set; }
      


    }
}