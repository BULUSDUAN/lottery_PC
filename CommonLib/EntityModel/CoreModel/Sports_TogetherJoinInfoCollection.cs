using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{
   public class Sports_TogetherJoinInfoCollection:Page
    {
        public Sports_TogetherJoinInfoCollection()
        {

        }

        public List<Sports_TogetherJoinInfo> List { get; set; }
    }
    public class Sports_TogetherJoinInfo
    {
        public Sports_TogetherJoinInfo()
        { }
        public long JoinId { get; set; }
        public string UserId { get; set; }
        public string UserDisplayName { get; set; }
        public int HideDisplayNameCount { get; set; }
        public string SchemeId { get; set; }
        public int BuyCount { get; set; }
        public int RealBuyCount { get; set; }
        public bool IsSucess { get; set; }
        public decimal Price { get; set; }
        public decimal BonusMoney { get; set; }
        public TogetherJoinType JoinType { get; set; }
        public DateTime JoinDateTime { get; set; }
    }
}
