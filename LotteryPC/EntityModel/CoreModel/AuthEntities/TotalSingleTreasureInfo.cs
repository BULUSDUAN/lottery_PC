using EntityModel.Enum;
using System;
using System.Collections.Generic;
using System.Text;

namespace EntityModel.CoreModel
{

        public class BDFXCommisionInfo
        {
            /// <summary>
            /// 用户编号
            /// </summary>
            public string UserId { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }
            /// <summary>
            /// 中奖提成
            /// </summary>
            public decimal Commission { get; set; }
        }
        /// <summary>
        /// 牛人排行
        /// </summary>
     
        public class BDFXNRRankListInfo
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public decimal CurrProfitRate { get; set; }
            public int RankNumber { get; set; }
        }

     
        public class BDFXNRRankList_Collection
        {
            public BDFXNRRankList_Collection()
            {
                RanList = new List<BDFXNRRankListInfo>();
            }
            public int TotalCount { get; set; }
            public List<BDFXNRRankListInfo> RanList { get; set; }
        }

      
        public class SingleTreasureAttentionInfo
        {
            /// <summary>
            /// 主键
            /// </summary>
            public int Id { get; set; }
            /// <summary>
            /// 被关注用户编号
            /// </summary>
            public string BeConcernedUserId { get; set; }
            /// <summary>
            /// 关注者用户编号
            /// </summary>
            public string ConcernedUserId { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
        }
    
        public class SingleTreasureAttention_Collection : List<SingleTreasureAttentionInfo>
        {

        }

        /// <summary>
        /// 网页宝单或大单推荐专家
        /// </summary>
       
        public class WebUserSchemeShareExpertInfo
        {
            public string UserId { get; set; }
            public string UserName { get; set; }
            public int HideDisplayNameCount { get; set; }
            public decimal TotalProfit { get; set; }
            public int TotalFansCount { get; set; }
            /// <summary>
            /// 万元户等级
            /// </summary>
            public string MaxLevelName { get; set; }
            /// <summary>
            /// 用户自定义头像
            /// </summary>
            public string UserCustomerImgUrl { get; set; }
        }
     
        public class WebUserSchemeShareExpert_Collection
        {
            public WebUserSchemeShareExpert_Collection()
            {
                UserSchemeShareExpertList = new List<WebUserSchemeShareExpertInfo>();
            }
            public int TotalCount { get; set; }
            public List<WebUserSchemeShareExpertInfo> UserSchemeShareExpertList { get; set; }
        }

        /// <summary>
        /// 宝单、大单推荐专家
        /// </summary>
     
        public class UserSchemeShareExpertInfo
        {
            /// <summary>
            /// 主键编号
            /// </summary>
            public Int64 Id { get; set; }
            /// <summary>
            /// 用户编号
            /// </summary>
            public string UserId { get; set; }
            /// <summary>
            /// 专家类别：分为宝单专家和大单专家
            /// </summary>
            public CopyOrderSource ExpertType { get; set; }
            /// <summary>
            /// 是否启用
            /// </summary>
            public bool IsEnable { get; set; }
            /// <summary>
            /// 显示排序号
            /// </summary>
            public int ShowSort { get; set; }
            /// <summary>
            /// 创建时间
            /// </summary>
            public DateTime CreateTime { get; set; }
            /// <summary>
            /// 用户名
            /// </summary>
            public string UserName { get; set; }
        }
        /// <summary>
        /// 宝单、大单推荐专家
        /// </summary>
      
        public class UserSchemeShareExpert_Collection
        {
            public UserSchemeShareExpert_Collection()
            {
                SchemeShareExpertList = new List<UserSchemeShareExpertInfo>();
            }
            public int TotalCount { get; set; }
            public List<UserSchemeShareExpertInfo> SchemeShareExpertList { get; set; }
        }
    
}
