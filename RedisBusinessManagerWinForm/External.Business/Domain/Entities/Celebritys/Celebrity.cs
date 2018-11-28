using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using External.Core;
using GameBiz.Core;

namespace External.Domain.Entities.Celebritys
{
    /// <summary>
    /// 名家
    /// </summary>
    public class Celebrity
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 名家类别
        /// </summary>
        public virtual CelebrityType CelebrityType { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public virtual DealWithType DealWithType { get; set; }
        /// <summary>
        /// 是否启用
        /// </summary>
        public virtual bool IsEnable { get; set; }
        /// <summary>
        /// 描述
        /// </summary>
        public virtual string Description { get; set; }
        /// <summary>
        /// 头像路径
        /// </summary>
        public virtual string Picurl { get; set; }
        /// <summary>
        /// 名家路径
        /// </summary>
        public virtual string WinnerUrl { get; set; }
    }

    /// <summary>
    /// 名家返点
    /// </summary>
    public class CelebrityRebate
    {
        /// <summary>
        /// 返点编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 彩种编码
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 彩种类型
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public virtual decimal Rebate { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 名家吐槽
    /// </summary>
    public class CelebrityTsukkomi
    {
        /// <summary>
        /// 吐槽编号
        /// </summary>
        public virtual int Id { get; set; }
        /// <summary>
        /// 名家编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 吐槽用户编号
        /// </summary>
        public virtual string SendUserId { get; set; }
        /// <summary>
        /// 吐槽内容
        /// </summary>
        public virtual string Content { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 处理类别
        /// </summary>
        public virtual DealWithType DealWithType { get; set; }
        /// <summary>
        /// 处理意见
        /// </summary>
        public virtual string DisposeOpinion { get; set; }
    }
}
