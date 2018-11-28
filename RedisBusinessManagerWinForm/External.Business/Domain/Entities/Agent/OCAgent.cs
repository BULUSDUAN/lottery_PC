using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GameBiz.Core;
using Common;

namespace External.Domain.Entities.Agent
{
    /// <summary>
    /// 澳彩代理
    /// </summary>
    public class OCAgent
    {
        /// <summary>
        /// 用户编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 店面编号
        /// </summary>
        public virtual string StoreId { get; set; }
        /// <summary>
        /// 代理类型
        /// </summary>
        public virtual OCAgentCategory OCAgentCategory { get; set; }
        /// <summary>
        /// 上级用户编号
        /// </summary>
        public virtual string ParentUserId { get; set; }
        /// <summary>
        /// 自定义域名
        /// </summary>
        public virtual string CustomerDomain { get; set; }
        /// <summary>
        /// CPS模式
        /// </summary>
        public virtual CPSMode CPSMode { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
        /// <summary>
        /// 父节点路径
        /// </summary>
        public virtual string ParentPath { get; set; }
        /// <summary>
        /// 渠道名称
        /// </summary>
        public virtual string ChannelName { get; set; }
    }

    /// <summary>
    /// 澳彩代理返点
    /// </summary>
    public class OCAgentRebate
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 代理编号
        /// </summary>
        public virtual string UserId { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 自身返点
        /// </summary>
        public virtual decimal Rebate { get; set; }
        /// <summary>
        /// 返点类型:0:串关返点；1:单关返点；
        /// </summary>
        public virtual int RebateType { get; set; }
        /// <summary>
        /// 下级用户默认返点
        /// </summary>
        public virtual decimal SubUserRebate { get; set; }
        /// <summary>
        /// CPS模式
        /// </summary>
        public virtual CPSMode CPSMode { get; set; }
        /// <summary>
        /// 生成时间
        /// </summary>
        public virtual DateTime CreateTime { get; set; }
    }

    /// <summary>
    /// 代理返点明细
    /// </summary>
    public class OCAgentPayDetail
    {
        public virtual long Id { get; set; }
        /// <summary>
        /// 订单号
        /// </summary>
        public virtual string SchemeId { get; set; }
        /// <summary>
        /// 订单用户（投注订单的用户）
        /// </summary>
        public virtual string OrderUser { get; set; }
        /// <summary>
        /// 收入用户编号
        /// </summary>
        public virtual string PayInUserId { get; set; }
        /// <summary>
        /// 方案类型
        /// </summary>
        public virtual SchemeType SchemeType { get; set; }
        /// <summary>
        /// 彩种
        /// </summary>
        public virtual string GameCode { get; set; }
        /// <summary>
        /// 玩法
        /// </summary>
        public virtual string GameType { get; set; }
        /// <summary>
        /// 返点
        /// </summary>
        public virtual decimal Rebate { get; set; }
        /// <summary>
        /// 订单总金额  或 订单盈利金额
        /// </summary>
        public virtual decimal OrderTotalMoney { get; set; }
        /// <summary>
        /// 返利金额
        /// </summary>
        public virtual decimal PayMoney { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public virtual string Remark { get; set; }
        /// <summary>
        /// CPS模式
        /// </summary>
        public virtual CPSMode CPSMode { get; set; }
        /// <summary>
        /// 处理人（用于提现返点，结算分红）
        /// </summary>
        public virtual string HandlPeople { get; set; }
        public virtual DateTime CreateTime { get; set; }
    }

}
