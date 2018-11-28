using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Common.Communication;

namespace External.Core.Task
{
    [CommunicationObject]
    public class TaskListInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public string OrderId { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 赠送成长值 
        /// </summary>
        public decimal ValueGrowth { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskCategory TaskCategory { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class TaskListInfoCollection
    {
        public TaskListInfoCollection()
        {
            List = new List<TaskListInfo>();
        }
        public List<TaskListInfo> List { get; set; }
        public int TotalCount { get; set; }
    }

    /// <summary>
    /// 热门累计任务
    /// </summary>
    [CommunicationObject]
    public class TaskHotCumulativeInfo
    {
        /// <summary>
        /// 中奖次数
        /// </summary>
        public int Count { get; set; }
        /// <summary>
        /// 中奖金额(只有累计中奖金额1000才使用这个字段)
        /// </summary>
        public decimal WinMonery { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 任务类型
        /// </summary>
        public TaskCategory TaskCategory { get; set; }
    }

    [CommunicationObject]
    public class TaskHotCumulativeInfoCollection
    {
        public TaskHotCumulativeInfoCollection()
        {
            List = new List<TaskHotCumulativeInfo>();
        }
        public List<TaskHotCumulativeInfo> List { get; set; }
    }


    /// <summary>
    /// 最新会员得到成长值动态
    /// </summary>
    [CommunicationObject]
    public class TaskHotTodayInfo
    {
        /// <summary>
        /// 用户Id
        /// </summary>
        public string UserId { get; set; }
        /// <summary>
        /// 显示名
        /// </summary>
        public string DisplayName { get; set; }
        /// <summary>
        /// 任务名称
        /// </summary>
        public string TaskName { get; set; }
        /// <summary>
        /// 赠送成长值 
        /// </summary>
        public decimal ValueGrowth { get; set; }
    }

    [CommunicationObject]
    public class TaskHotTodayInfooCollection
    {
        public TaskHotTodayInfooCollection()
        {
            List = new List<TaskHotTodayInfo>();
        }
        public List<TaskHotTodayInfo> List { get; set; }
    }

    /// <summary>
    /// 活动列表
    /// </summary>
    [CommunicationObject]
    public class ActivityListInfo
    {
        /// <summary>
        /// 活动Id
        /// </summary>
        public int ActivityIndex { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public string ActiveName { get; set; }
        /// <summary>
        /// 活动标题
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// 活动描述
        /// </summary>
        public string Summary { get; set; }
        /// <summary>
        /// 活动地址
        /// </summary>
        public string LinkUrl { get; set; }
        /// <summary>
        /// 图片地址
        /// </summary>
        public string ImageUrl { get; set; }
        /// <summary>
        /// 是否显示
        /// </summary>
        public bool IsShow { get; set; }
        /// <summary>
        /// 活动开始时间
        /// </summary>
        public DateTime BeginTime { get; set; }
        /// <summary>
        /// 活动结束时间
        /// </summary>
        public DateTime EndTime { get; set; }
        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime CreateTime { get; set; }
    }
    [CommunicationObject]
    public class ActivityListInfoCollection
    {
        public ActivityListInfoCollection()
        {
            List = new List<ActivityListInfo>();
        }
        public List<ActivityListInfo> List { get; set; }
        public int TotalCount { get; set; }
    }



}
