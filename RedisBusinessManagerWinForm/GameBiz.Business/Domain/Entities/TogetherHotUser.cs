using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameBiz.Domain.Entities
{
    /// <summary>
    /// 合买红人表
    /// </summary>
    public class TogetherHotUser
    {
        /// <summary>
        /// id
        /// </summary>
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户ID
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 近期本周中奖
        /// </summary>
        public virtual decimal WeeksWinMoney { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    ///// <summary>
    ///// 合买红人订单表
    ///// </summary>
    //public class TogetherHotOrder
    //{
    //    /// <summary>
    //    /// id
    //    /// </summary>
    //    public virtual long Id { get; set; }
    //    /// <summary>
    //    /// 用户ID
    //    /// </summary>
    //    public virtual string UserId { get; set; }
    //    /// <summary>
    //    /// 彩种
    //    /// </summary>
    //    public virtual string GameCode { get; set; }
    //    /// <summary>
    //    /// 玩法
    //    /// </summary>
    //    public virtual string GameType { get; set; }
    //    /// <summary>
    //    /// 方案金额
    //    /// </summary>
    //    public virtual decimal TotalMoney { get; set; }
    //    /// <summary>
    //    /// 方案进度
    //    /// </summary>
    //    public virtual decimal Progress { get; set; }
    //    /// <summary>
    //    /// 前台是否显示
    //    /// </summary>
    //    public virtual bool IsShow { get; set; }
    //    /// <summary>
    //    /// 创建时间
    //    /// </summary>
    //    public virtual DateTime CreateTime { get; set; }
    //}
}
