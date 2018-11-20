using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Auth.Domain.Entities;
using GameBiz.Domain.Entities;
using External.Core;

namespace External.Domain.Entities.Task
{
    /// <summary>
    /// 用户任务赠送记录表
    /// </summary>
    public class TaskList
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public virtual string TaskName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public virtual string Content { get; set; }
        /// <summary>
        /// 赠送成长值 
        /// </summary>
        public virtual decimal ValueGrowth { get; set; }
        /// <summary>
        /// 是否领取 
        /// </summary>
        public virtual bool IsGive { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public virtual TaskCategory TaskCategory { get; set; }
        /// <summary>
        /// VIP活动等级
        /// </summary>
        public virtual int VipLevel { get; set; }
        /// <summary>
        /// 创建时间戳
        /// </summary>
        public virtual string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 用户任务完成记录表
    /// </summary>
    public class UserTaskRecord
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 用户Id
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string OrderId { get; set; }
        /// <summary>
        /// 订单金额
        /// </summary>
        public virtual decimal OrderMoney { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public virtual string TaskName { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public virtual TaskCategory TaskCategory { get; set; }
        /// <summary>
        /// 创建时间戳
        /// </summary>
        public virtual string CurrentTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }
}
