using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;
using GameBiz.Core;

namespace External.Core.Celebritys
{
    /// <summary>
    /// 名家
    /// </summary>
    [CommunicationObject]
    public class CelebritysInfo
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 关注人数
        /// </summary>
        public int Attention { get; set; }
        /// <summary>
        /// 粉丝人数
        /// </summary>
        public int Fans { get; set; }
        /// <summary>
        /// 名家类别
        /// </summary>
        public CelebrityType CelebrityType { get; set; }
        /// <summary>
        /// 是否认证为名家
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public bool IsEnable { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// 头像路径
        /// </summary>
        public string Picurl { get; set; }
        /// <summary>
        /// 名家路径
        /// </summary>
        public string WinnerUrl { get; set; }
    }

    [CommunicationObject]
    public class CelebritysInfoCollection
    {
        public CelebritysInfoCollection()
        {
            List = new List<CelebritysInfo>();
        }
        public int TotalCount { get; set; }
        public List<CelebritysInfo> List { get; set; }
    }

    /// <summary>
    /// 名家返点
    /// </summary>
    [CommunicationObject]
    public class CelebrityRebateInfo
    {
        /// <summary>
        /// 返点编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public string GameCode { get; set; }
        /// <summary>
        /// 彩种类型
        /// </summary>
        public string GameType { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public decimal Rebate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }

    [CommunicationObject]
    public class CelebrityRebateInfoCollection
    {
        public CelebrityRebateInfoCollection()
        {
            List = new List<CelebrityRebateInfo>();
        }
        public int TotalCount { get; set; }
        public List<CelebrityRebateInfo> List { get; set; }
    }

    /// <summary>
    /// 名家吐槽
    /// </summary>
    [CommunicationObject]
    public class CelebrityTsukkomiInfo
    {
        /// <summary>
        /// 吐槽编号
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// 名家编号
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 吐槽用户编号
        /// </summary>
        public string SendUserId { get; set; }
        /// <summary>
        /// 吐槽内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public DealWithType DealWithType { get; set; }
        /// <summary>
        /// 用户名称
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public string DisposeOpinion { get; set; }
    }

    [CommunicationObject]
    public class CelebrityTsukkomiInfoCollection
    {
        public CelebrityTsukkomiInfoCollection()
        {
            List = new List<CelebrityTsukkomiInfo>();
        }
        public int TotalCount { get; set; }
        public List<CelebrityTsukkomiInfo> List { get; set; }
    }

}
